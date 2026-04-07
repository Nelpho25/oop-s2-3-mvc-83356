using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Services;

/// <summary>
/// Service for attendance tracking and calculations
/// </summary>
public interface IAttendanceService
{
    /// <summary>
    /// Calculate attendance rate for an enrollment
    /// </summary>
    Task<decimal> CalculateAttendanceRateAsync(int enrolmentId);

    /// <summary>
    /// Record attendance for a session
    /// </summary>
    Task RecordAttendanceAsync(int enrolmentId, DateTime sessionDate, bool isPresent);

    /// <summary>
    /// Get attendance records for an enrollment
    /// </summary>
    Task<List<AttendanceRecord>> GetAttendanceRecordsAsync(int enrolmentId);

    /// <summary>
    /// Validate session date is within course period
    /// </summary>
    Task<bool> IsValidSessionDateAsync(int courseId, DateTime sessionDate);
}
