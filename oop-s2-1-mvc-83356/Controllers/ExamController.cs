using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.ViewModels;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin,Faculty")]
[Route("Admin/Exams")]
public class ExamController : BaseController
{
    private readonly ILogger<ExamController> _logger;

    public ExamController(ApplicationDbContext context, ILogger<ExamController> logger) : base(context)
    {
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var exams = await _context.Exams
            .Include(e => e.Course)
            .Include(e => e.ExamResults)
            .Select(e => new ExamIndexViewModel
            {
                Id = e.Id,
                CourseName = e.Course.Name,
                ExamName = e.Title,
                ExamDate = e.ExamDate.HasValue ? e.ExamDate.Value : DateTime.Now,
                MaxScore = e.MaxScore,
                ResultsReleased = e.ResultsReleased,
                SubmissionsCount = e.ExamResults.Count
            })
            .OrderByDescending(e => e.ExamDate)
            .ToListAsync();

            _logger.LogInformation("Exam index retrieved. Total exams: {ExamCount}", exams.Count);
            return View(exams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exam index");
            TempData["Error"] = "An error occurred while retrieving exams.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("{id}/Results")]
    public async Task<IActionResult> Results(int id)
    {
        try
        {
            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.StudentProfile)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                _logger.LogWarning("Results requested for non-existent exam {ExamId}", id);
                return NotFound();
            }

            _logger.LogInformation("Exam results retrieved for exam {ExamId} ({ExamTitle}). Total results: {ResultCount}", id, exam.Title, exam.ExamResults.Count);
            return View(exam);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exam results for exam {ExamId}", id);
            TempData["Error"] = "An error occurred while retrieving exam results.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost("{id}/ToggleResultsRelease")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleResultsRelease(int id)
    {
        try
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                _logger.LogWarning("Toggle results release requested for non-existent exam {ExamId}", id);
                return NotFound();
            }

            exam.ResultsReleased = !exam.ResultsReleased;
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Exam {ExamId} ({ExamTitle}) results release toggled to {ResultsReleased} by {User}", id, exam.Title, exam.ResultsReleased, User.Identity?.Name);
            TempData["Success"] = $"Results for '{exam.Title}' are now {(exam.ResultsReleased ? "released" : "hidden")}.";
            return RedirectToAction("Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error toggling results release for exam {ExamId}", id);
            TempData["Error"] = "An error occurred while updating exam results release status. Please try again.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error toggling results release for exam {ExamId}", id);
            return RedirectToAction("Error", "Home");
        }
    }
}
