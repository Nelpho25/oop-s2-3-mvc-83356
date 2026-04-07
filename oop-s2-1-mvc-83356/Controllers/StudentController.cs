using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.ViewModels;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Students")]
public class StudentController : BaseController
{
    private readonly ILogger<StudentController> _logger;

    public StudentController(ApplicationDbContext context, ILogger<StudentController> logger) : base(context)
    {
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var students = await _context.StudentProfiles
                .Include(s => s.CourseEnrolments)
                .Select(s => new StudentIndexViewModel
                {
                    Id = s.Id,
                    StudentNumber = s.StudentNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    EnrolmentCount = s.CourseEnrolments.Count
                })
                .ToListAsync();

            _logger.LogInformation("Student index retrieved. Total students: {StudentCount}", students.Count);
            return View(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student index");
            TempData["Error"] = "An error occurred while retrieving students.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("{id}/Details")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var student = await _context.StudentProfiles
                .Include(s => s.CourseEnrolments)
                    .ThenInclude(ce => ce.Course)
                .Include(s => s.AssignmentResults)
                    .ThenInclude(ar => ar.Assignment)
                        .ThenInclude(a => a.Course)
                .Include(s => s.ExamResults)
                    .ThenInclude(er => er.Exam)
                        .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                _logger.LogWarning("Student details requested for non-existent student {StudentId}", id);
                return NotFound();
            }

            // Get attendance records
            var enrolmentIds = student.CourseEnrolments.Select(ce => ce.Id).ToList();
            var attendanceRecords = await _context.AttendanceRecords
                .Where(ar => enrolmentIds.Contains(ar.CourseEnrolmentId))
                .Include(ar => ar.CourseEnrolment)
                    .ThenInclude(ce => ce.Course)
                .ToListAsync();

        var model = new StudentDetailsViewModel
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Phone = student.Phone,
            Address = student.Address,
            DateOfBirth = student.DateOfBirth ?? DateTime.Now.AddYears(-20),
            Enrolments = student.CourseEnrolments.Select(ce => new EnrolmentStatusViewModel
            {
                EnrolmentId = ce.Id,
                CourseId = ce.CourseId,
                CourseName = ce.Course.Name,
                Status = ce.Status.ToString(),
                EnrolmentDate = ce.EnrolDate,
                CompletionDate = null
            }).ToList(),
            AssignmentResults = student.AssignmentResults.Select(ar => new AssignmentResultViewModel
            {
                ResultId = ar.Id,
                CourseName = ar.Assignment.Course.Name,
                AssignmentName = ar.Assignment.Title,
                Score = ar.Score ?? 0,
                MaxScore = ar.Assignment.MaxScore,
                Feedback = ar.Feedback,
                SubmissionDate = ar.SubmittedAt ?? DateTime.Now
            }).ToList(),
            ExamResults = student.ExamResults.Select(er => new ExamResultViewModel
            {
                ResultId = er.Id,
                CourseName = er.Exam.Course.Name,
                ExamName = er.Exam.Title,
                Score = er.Score ?? 0,
                MaxScore = er.Exam.MaxScore,
                Grade = er.Grade,
                ExamDate = er.Exam.ExamDate ?? DateTime.Now,
                ResultsReleased = er.Exam.ResultsReleased
            }).ToList(),
            AttendanceHistory = attendanceRecords
                .GroupBy(ar => ar.CourseEnrolment.Course.Name)
                .Select(g => new AttendanceHistoryViewModel
                {
                    CourseName = g.Key,
                    TotalSessions = g.Count(),
                    PresentCount = g.Count(ar => ar.IsPresent),
                    Records = g.OrderBy(ar => ar.SessionDate)
                        .Select(ar => new AttendanceRecordViewModel
                        {
                            Id = ar.Id,
                            SessionDate = ar.SessionDate,
                            IsPresent = ar.IsPresent
                        }).ToList()
                }).ToList()
            };

            _logger.LogInformation("Student {StudentId} details retrieved", id);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student details for {StudentId}", id);
            TempData["Error"] = "An error occurred while retrieving student details.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View("CreateEdit", new StudentCreateEditViewModel
        {
            DateOfBirth = DateTime.Now.AddYears(-20)
        });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(StudentCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View("CreateEdit", model);

        var student = new StudentProfile
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Phone = model.Phone,
            Address = model.Address,
            DateOfBirth = model.DateOfBirth
        };

        _context.StudentProfiles.Add(student);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Student '{student.FirstName} {student.LastName}' created successfully.";
        return RedirectToAction("Index");
    }

    [HttpGet("{id}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return NotFound();

        var model = new StudentCreateEditViewModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Phone = student.Phone,
            Address = student.Address,
            DateOfBirth = student.DateOfBirth ?? DateTime.Now.AddYears(-20)
        };

        return View("CreateEdit", model);
    }

    [HttpPost("{id}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int id, StudentCreateEditViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View("CreateEdit", model);

        var student = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return NotFound();

        student.FirstName = model.FirstName;
        student.LastName = model.LastName;
        student.Phone = model.Phone;
        student.Address = model.Address;
        student.DateOfBirth = model.DateOfBirth;

        _context.StudentProfiles.Update(student);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Student '{student.FirstName} {student.LastName}' updated successfully.";
        return RedirectToAction("Index");
    }

    [HttpGet("{id}/Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.StudentProfiles
            .Include(s => s.CourseEnrolments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return NotFound();

        if (student.CourseEnrolments.Any())
        {
            TempData["Error"] = "Cannot delete student with active enrolments.";
            return RedirectToAction("Details", new { id });
        }

        var model = new StudentCreateEditViewModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Phone = student.Phone,
            Address = student.Address,
            DateOfBirth = student.DateOfBirth ?? DateTime.Now.AddYears(-20)
        };

        return View(model);
    }

    [HttpPost("{id}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.StudentProfiles
            .Include(s => s.CourseEnrolments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return NotFound();

        if (student.CourseEnrolments.Any())
        {
            TempData["Error"] = "Cannot delete student with active enrolments.";
            return RedirectToAction("Details", new { id });
        }

        _context.StudentProfiles.Remove(student);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Student '{student.FirstName} {student.LastName}' deleted successfully.";
        return RedirectToAction("Index");
    }
}
