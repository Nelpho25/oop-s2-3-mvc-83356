using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Services;

public class ExamResultService : IExamResultService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExamResultService> _logger;

    public ExamResultService(ApplicationDbContext context, ILogger<ExamResultService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ExamResult>> GetVisibleResultsForStudentAsync(int studentId)
    {
        try
        {
            var results = await _context.ExamResults
                .Where(er => er.StudentProfileId == studentId && er.Exam.ResultsReleased)
                .Include(er => er.Exam)
                .Include(er => er.StudentProfile)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} visible exam results for student {StudentId}", results.Count, studentId);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving visible results for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<List<ExamResult>> GetAllResultsAsync()
    {
        return await _context.ExamResults
            .Include(er => er.Exam)
            .Include(er => er.StudentProfile)
            .ToListAsync();
    }

    public async Task<bool> CanStudentViewResultAsync(int studentId, int examResultId)
    {
        try
        {
            var result = await _context.ExamResults
                .Where(er => er.Id == examResultId && er.StudentProfileId == studentId)
                .Include(er => er.Exam)
                .FirstOrDefaultAsync();

            if (result == null)
                return false;

            // Student can view only if results are released
            return result.Exam.ResultsReleased;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking student {StudentId} access to exam result {ResultId}", studentId, examResultId);
            throw;
        }
    }

    public async Task<List<ExamResult>> GetResultsByExamAsync(int examId, bool visibleOnly = true)
    {
        var query = _context.ExamResults
            .Where(er => er.ExamId == examId)
            .Include(er => er.StudentProfile);

        if (visibleOnly)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam != null && !exam.ResultsReleased)
            {
                _logger.LogWarning("Exam {ExamId} results not released", examId);
                return new List<ExamResult>();
            }
        }

        return await query.ToListAsync();
    }
}
