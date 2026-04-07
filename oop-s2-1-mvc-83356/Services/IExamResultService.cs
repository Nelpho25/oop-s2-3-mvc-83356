using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Services;

/// <summary>
/// Service for exam result visibility and access control
/// </summary>
public interface IExamResultService
{
    /// <summary>
    /// Get visible exam results for a student (only if released)
    /// </summary>
    Task<List<ExamResult>> GetVisibleResultsForStudentAsync(int studentId);

    /// <summary>
    /// Get all exam results (admin only - no filtering)
    /// </summary>
    Task<List<ExamResult>> GetAllResultsAsync();

    /// <summary>
    /// Check if student can view specific exam result
    /// </summary>
    Task<bool> CanStudentViewResultAsync(int studentId, int examResultId);

    /// <summary>
    /// Get results by exam (visible only if released)
    /// </summary>
    Task<List<ExamResult>> GetResultsByExamAsync(int examId, bool visibleOnly = true);
}
