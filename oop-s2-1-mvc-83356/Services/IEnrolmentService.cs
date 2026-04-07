using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Services;

/// <summary>
/// Service for managing student course enrollments
/// </summary>
public interface IEnrolmentService
{
    /// <summary>
    /// Enroll a student in a course
    /// </summary>
    /// <returns>The created enrollment, or null if already enrolled</returns>
    Task<CourseEnrolment?> EnrollStudentAsync(int studentId, int courseId);

    /// <summary>
    /// Withdraw a student from a course
    /// </summary>
    Task WithdrawStudentAsync(int enrolmentId);

    /// <summary>
    /// Get all enrollments for a student
    /// </summary>
    Task<List<CourseEnrolment>> GetEnrolmentsByStudentAsync(int studentId);

    /// <summary>
    /// Get all enrollments for a course
    /// </summary>
    Task<List<CourseEnrolment>> GetEnrolmentsByCourseAsync(int courseId);

    /// <summary>
    /// Check if student is already enrolled
    /// </summary>
    Task<bool> IsStudentEnrolledAsync(int studentId, int courseId);
}
