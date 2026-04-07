using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Services;

public class EnrolmentService : IEnrolmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EnrolmentService> _logger;

    public EnrolmentService(ApplicationDbContext context, ILogger<EnrolmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CourseEnrolment?> EnrollStudentAsync(int studentId, int courseId)
    {
        try
        {
            // Check if student exists
            var student = await _context.StudentProfiles.FindAsync(studentId);
            if (student == null)
            {
                _logger.LogWarning("Student {StudentId} not found for enrollment", studentId);
                return null;
            }

            // Check if course exists
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Course {CourseId} not found for enrollment", courseId);
                return null;
            }

            // Check if already enrolled
            if (await IsStudentEnrolledAsync(studentId, courseId))
            {
                _logger.LogWarning("Student {StudentId} already enrolled in course {CourseId}", studentId, courseId);
                return null;
            }

            // Create enrollment
            var enrolment = new CourseEnrolment
            {
                StudentProfileId = studentId,
                CourseId = courseId,
                EnrolDate = DateTime.UtcNow,
                Status = CourseEnrolmentStatus.Active
            };

            _context.CourseEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} enrolled in course {CourseId}", studentId, courseId);
            return enrolment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}", studentId, courseId);
            throw;
        }
    }

    public async Task WithdrawStudentAsync(int enrolmentId)
    {
        try
        {
            var enrolment = await _context.CourseEnrolments.FindAsync(enrolmentId);
            if (enrolment == null)
            {
                _logger.LogWarning("Enrolment {EnrolmentId} not found for withdrawal", enrolmentId);
                return;
            }

            enrolment.Status = CourseEnrolmentStatus.Withdrawn;
            _context.CourseEnrolments.Update(enrolment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Enrolment {EnrolmentId} withdrawn", enrolmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing enrolment {EnrolmentId}", enrolmentId);
            throw;
        }
    }

    public async Task<List<CourseEnrolment>> GetEnrolmentsByStudentAsync(int studentId)
    {
        return await _context.CourseEnrolments
            .Where(ce => ce.StudentProfileId == studentId)
            .Include(ce => ce.Course)
            .ToListAsync();
    }

    public async Task<List<CourseEnrolment>> GetEnrolmentsByCourseAsync(int courseId)
    {
        return await _context.CourseEnrolments
            .Where(ce => ce.CourseId == courseId)
            .Include(ce => ce.StudentProfile)
            .ToListAsync();
    }

    public async Task<bool> IsStudentEnrolledAsync(int studentId, int courseId)
    {
        return await _context.CourseEnrolments
            .AnyAsync(ce => ce.StudentProfileId == studentId && ce.CourseId == courseId);
    }
}
