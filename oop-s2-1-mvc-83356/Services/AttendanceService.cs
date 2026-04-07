using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AttendanceService> _logger;

    public AttendanceService(ApplicationDbContext context, ILogger<AttendanceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<decimal> CalculateAttendanceRateAsync(int enrolmentId)
    {
        try
        {
            var records = await _context.AttendanceRecords
                .Where(ar => ar.CourseEnrolmentId == enrolmentId)
                .ToListAsync();

            if (records.Count == 0)
            {
                _logger.LogWarning("No attendance records found for enrolment {EnrolmentId}", enrolmentId);
                return 0;
            }

            var presentCount = records.Count(r => r.IsPresent);
            var rate = (decimal)presentCount / records.Count * 100;

            _logger.LogInformation("Attendance rate for enrolment {EnrolmentId}: {Rate}%", enrolmentId, rate);
            return rate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating attendance for enrolment {EnrolmentId}", enrolmentId);
            throw;
        }
    }

    public async Task RecordAttendanceAsync(int enrolmentId, DateTime sessionDate, bool isPresent)
    {
        try
        {
            // Validate session date
            var enrolment = await _context.CourseEnrolments
                .Include(ce => ce.Course)
                .FirstOrDefaultAsync(ce => ce.Id == enrolmentId);

            if (enrolment == null)
            {
                _logger.LogWarning("Enrolment {EnrolmentId} not found", enrolmentId);
                return;
            }

            if (!await IsValidSessionDateAsync(enrolment.CourseId, sessionDate))
            {
                _logger.LogWarning("Invalid session date {SessionDate} for course {CourseId}", sessionDate, enrolment.CourseId);
                return;
            }

            var record = new AttendanceRecord
            {
                CourseEnrolmentId = enrolmentId,
                SessionDate = sessionDate,
                IsPresent = isPresent
            };

            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Attendance recorded for enrolment {EnrolmentId} on {SessionDate}", enrolmentId, sessionDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording attendance for enrolment {EnrolmentId}", enrolmentId);
            throw;
        }
    }

    public async Task<List<AttendanceRecord>> GetAttendanceRecordsAsync(int enrolmentId)
    {
        return await _context.AttendanceRecords
            .Where(ar => ar.CourseEnrolmentId == enrolmentId)
            .OrderBy(ar => ar.SessionDate)
            .ToListAsync();
    }

    public async Task<bool> IsValidSessionDateAsync(int courseId, DateTime sessionDate)
    {
        try
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            var startDate = course.StartDate ?? DateTime.MinValue;
            var endDate = course.EndDate ?? DateTime.MaxValue;

            return sessionDate >= startDate && sessionDate <= endDate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating session date for course {CourseId}", courseId);
            throw;
        }
    }
}
