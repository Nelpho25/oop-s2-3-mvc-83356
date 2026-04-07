using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin,Faculty")]
[Route("Gradebook")]
public class GradebookController : BaseController
{
    public GradebookController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("Courses")]
    public async Task<IActionResult> Courses()
    {
        List<Course> courses;

        if (User.IsInRole("Admin"))
        {
            courses = await _context.Courses.ToListAsync();
        }
        else
        {
            var courseIds = await GetFacultyCourseIdsAsync();
            courses = await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .ToListAsync();
        }

        return View(courses);
    }

    [HttpGet("Course/{courseId}")]
    public async Task<IActionResult> Course(int courseId)
    {
        if (!await HasAccessToCourseAsync(courseId))
            return Forbid();

        var course = await _context.Courses
            .Include(c => c.Assignments)
                .ThenInclude(a => a.AssignmentResults)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            return NotFound();

        var enrolments = await _context.CourseEnrolments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.StudentProfile)
            .ToListAsync();

        var gradebookViewModel = new GradebookViewModel
        {
            Course = course,
            Enrolments = enrolments
        };

        return View(gradebookViewModel);
    }

    [HttpGet("Assignment/{assignmentId}")]
    public async Task<IActionResult> Assignment(int assignmentId)
    {
        var assignment = await _context.Assignments
            .Include(a => a.Course)
            .Include(a => a.AssignmentResults)
                .ThenInclude(ar => ar.StudentProfile)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
            return NotFound();

        if (!await HasAccessToCourseAsync(assignment.CourseId))
            return Forbid();

        return View(assignment);
    }

    [HttpPost("UpdateScore/{resultId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateScore(int resultId, decimal score, string feedback)
    {
        var result = await _context.AssignmentResults
            .Include(ar => ar.Assignment)
            .FirstOrDefaultAsync(ar => ar.Id == resultId);

        if (result == null)
            return NotFound();

        if (!await HasAccessToCourseAsync(result.Assignment.CourseId))
            return Forbid();

        result.Score = score;
        result.Feedback = feedback;

        _context.AssignmentResults.Update(result);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Assignment), new { assignmentId = result.AssignmentId });
    }

    [HttpGet("ExamResults/{courseId}")]
    public async Task<IActionResult> ExamResults(int courseId)
    {
        if (!await HasAccessToCourseAsync(courseId))
            return Forbid();

        var exams = await _context.Exams
            .Where(e => e.CourseId == courseId)
            .Include(e => e.ExamResults)
                .ThenInclude(er => er.StudentProfile)
            .ToListAsync();

        var examResultsViewModel = new ExamResultsViewModel
        {
            CourseId = courseId,
            Exams = exams
        };

        return View(examResultsViewModel);
    }

    [HttpPost("ReleaseResults/{examId}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReleaseResults(int examId)
    {
        var exam = await _context.Exams
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null)
            return NotFound();

        exam.ResultsReleased = !exam.ResultsReleased;
        _context.Exams.Update(exam);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ExamResults), new { courseId = exam.CourseId });
    }
}

public class GradebookViewModel
{
    public Course Course { get; set; } = null!;
    public List<CourseEnrolment> Enrolments { get; set; } = new();
}

public class ExamResultsViewModel
{
    public int CourseId { get; set; }
    public List<Exam> Exams { get; set; } = new();
}
