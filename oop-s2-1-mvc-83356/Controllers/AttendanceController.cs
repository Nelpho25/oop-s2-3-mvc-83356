using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin,Faculty")]
[Route("Attendance")]
public class AttendanceController : BaseController
{
    public AttendanceController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var courses = new List<Course>();
        var userId = GetCurrentUserId();

        if (User.IsInRole("Admin"))
        {
            courses = await _context.Courses.Include(c => c.Branch).ToListAsync();
        }
        else if (User.IsInRole("Faculty"))
        {
            var facultyProfile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == userId);

            if (facultyProfile != null)
            {
                courses = await _context.FacultyCourseAssignments
                    .Where(fca => fca.FacultyProfileId == facultyProfile.Id)
                    .Include(fca => fca.Course)
                    .ThenInclude(c => c.Branch)
                    .Select(fca => fca.Course)
                    .ToListAsync();
            }
        }

        return View(courses);
    }

    [HttpGet("Course/{courseId}")]
    public async Task<IActionResult> Course(int courseId)
    {
        if (!await HasAccessToCourseAsync(courseId))
            return Forbid();

        var enrolments = await _context.CourseEnrolments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.StudentProfile)
            .Include(e => e.AttendanceRecords)
            .OrderBy(e => e.StudentProfile.LastName)
            .ThenBy(e => e.StudentProfile.FirstName)
            .ToListAsync();

        var attendanceViewModel = new AttendanceCourseViewModel
        {
            CourseId = courseId,
            Enrolments = enrolments
        };

        return View(attendanceViewModel);
    }

    [HttpGet("Enrolment/{enrolmentId}")]
    public async Task<IActionResult> Enrolment(int enrolmentId)
    {
        var enrolment = await _context.CourseEnrolments
            .Include(e => e.Course)
            .Include(e => e.StudentProfile)
            .Include(e => e.AttendanceRecords.OrderByDescending(ar => ar.SessionDate))
            .FirstOrDefaultAsync(e => e.Id == enrolmentId);

        if (enrolment == null)
            return NotFound();

        if (!await HasAccessToCourseAsync(enrolment.CourseId))
            return Forbid();

        var attendanceViewModel = new AttendanceEnrolmentViewModel
        {
            Enrolment = enrolment,
            AttendanceRate = CalculateAttendanceRate(enrolment.AttendanceRecords)
        };

        return View(attendanceViewModel);
    }

    [HttpPost("Record/{enrolmentId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Record(int enrolmentId, DateTime sessionDate, bool isPresent, string notes)
    {
        var enrolment = await _context.CourseEnrolments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == enrolmentId);

        if (enrolment == null)
            return NotFound();

        if (!await HasAccessToCourseAsync(enrolment.CourseId))
            return Forbid();

        // Check if record already exists
        var existingRecord = await _context.AttendanceRecords
            .FirstOrDefaultAsync(ar => ar.CourseEnrolmentId == enrolmentId && ar.SessionDate.Date == sessionDate.Date);

        if (existingRecord != null)
        {
            existingRecord.IsPresent = isPresent;
            existingRecord.Notes = notes;
            _context.AttendanceRecords.Update(existingRecord);
        }
        else
        {
            var record = new AttendanceRecord
            {
                CourseEnrolmentId = enrolmentId,
                SessionDate = sessionDate,
                WeekNumber = GetWeekNumber(sessionDate),
                IsPresent = isPresent,
                Notes = notes
            };

            _context.AttendanceRecords.Add(record);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Enrolment), new { enrolmentId });
    }

    private double CalculateAttendanceRate(ICollection<AttendanceRecord> records)
    {
        if (records.Count == 0)
            return 0;

        var presentCount = records.Count(r => r.IsPresent);
        return (presentCount / (double)records.Count) * 100;
    }

    private int GetWeekNumber(DateTime date)
    {
        var jan1 = new DateTime(date.Year, 1, 1);
        var weekNumber = (date - jan1).Days / 7 + 1;
        return weekNumber;
    }
}

public class AttendanceCourseViewModel
{
    public int CourseId { get; set; }
    public List<CourseEnrolment> Enrolments { get; set; } = new();
}

public class AttendanceEnrolmentViewModel
{
    public CourseEnrolment Enrolment { get; set; } = null!;
    public double AttendanceRate { get; set; }
}
