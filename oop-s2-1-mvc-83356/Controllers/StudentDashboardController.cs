using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Student")]
[Route("Student")]
public class StudentDashboardController : BaseController
{
    public StudentDashboardController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var studentProfileId = await GetStudentProfileIdAsync();
        if (!studentProfileId.HasValue)
            return NotFound("Student profile not found.");

        var student = await _context.StudentProfiles
            .FirstOrDefaultAsync(s => s.Id == studentProfileId.Value);

        if (student == null)
            return NotFound("Student profile not found.");

        var enrolments = await _context.CourseEnrolments
            .Where(e => e.StudentProfileId == studentProfileId.Value)
            .Include(e => e.Course)
            .Include(e => e.AttendanceRecords)
            .ToListAsync();

        var stats = new StudentDashboardViewModel
        {
            StudentName = $"{student.FirstName} {student.LastName}",
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            EnroledCourses = enrolments.Count,
            ActiveEnrolments = enrolments.Count(e => e.Status == Models.CourseEnrolmentStatus.Active),
            CompletedEnrolments = enrolments.Count(e => e.Status == Models.CourseEnrolmentStatus.Completed),
            Enrolments = enrolments
        };

        return View(stats);
    }

    [HttpGet("Grades")]
    public async Task<IActionResult> Grades()
    {
        var studentProfileId = await GetStudentProfileIdAsync();
        if (!studentProfileId.HasValue)
            return NotFound("Student profile not found.");

        var examResults = await _context.ExamResults
            .Where(er => er.StudentProfileId == studentProfileId.Value)
            .Include(er => er.Exam)
            .ThenInclude(e => e.Course)
            .Where(er => er.Exam.ResultsReleased) // Only show released results
            .OrderByDescending(er => er.Exam.ExamDate)
            .ToListAsync();

        var assignmentResults = await _context.AssignmentResults
            .Where(ar => ar.StudentProfileId == studentProfileId.Value)
            .Include(ar => ar.Assignment)
            .ThenInclude(a => a.Course)
            .OrderByDescending(ar => ar.SubmittedAt)
            .ToListAsync();

        var gradesViewModel = new StudentGradesViewModel
        {
            ExamResults = examResults,
            AssignmentResults = assignmentResults
        };

        return View(gradesViewModel);
    }

    [HttpGet("Attendance")]
    public async Task<IActionResult> Attendance()
    {
        var studentProfileId = await GetStudentProfileIdAsync();
        if (!studentProfileId.HasValue)
            return NotFound("Student profile not found.");

        var attendance = await _context.AttendanceRecords
            .Where(ar => ar.CourseEnrolment.StudentProfileId == studentProfileId.Value)
            .Include(ar => ar.CourseEnrolment)
            .ThenInclude(ce => ce.Course)
            .OrderByDescending(ar => ar.SessionDate)
            .ToListAsync();

        return View(attendance);
    }
}

public class StudentDashboardViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int EnroledCourses { get; set; }
    public int ActiveEnrolments { get; set; }
    public int CompletedEnrolments { get; set; }
    public List<Models.CourseEnrolment> Enrolments { get; set; } = new();
}

public class StudentGradesViewModel
{
    public List<Models.ExamResult> ExamResults { get; set; } = new();
    public List<Models.AssignmentResult> AssignmentResults { get; set; } = new();
}
