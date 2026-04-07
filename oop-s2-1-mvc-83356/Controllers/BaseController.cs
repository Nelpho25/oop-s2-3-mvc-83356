using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using System.Security.Claims;

namespace oop_s2_1_mvc_83356.Controllers;

/// <summary>
/// Base controller providing common functionality for authorization and data access.
/// </summary>
public class BaseController : Controller
{
    protected readonly ApplicationDbContext _context;

    public BaseController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the current user's identity ID from the HttpContext.
    /// </summary>
    /// <returns>The user's identity ID, or null if not authenticated.</returns>
    protected string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the StudentProfile ID for the current user.
    /// </summary>
    /// <returns>The student profile ID, or null if the user is not a student.</returns>
    protected async Task<int?> GetStudentProfileIdAsync()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return null;

        var studentProfile = await _context.StudentProfiles
            .Where(s => s.IdentityUserId == userId)
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

        return studentProfile == 0 ? null : studentProfile;
    }

    /// <summary>
    /// Gets the FacultyProfile ID for the current user.
    /// </summary>
    /// <returns>The faculty profile ID, or null if the user is not faculty.</returns>
    protected async Task<int?> GetFacultyProfileIdAsync()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return null;

        var facultyProfile = await _context.FacultyProfiles
            .Where(f => f.IdentityUserId == userId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();

        return facultyProfile == 0 ? null : facultyProfile;
    }

    /// <summary>
    /// Gets all course IDs assigned to the current faculty member.
    /// </summary>
    /// <returns>A list of course IDs.</returns>
    protected async Task<List<int>> GetFacultyCourseIdsAsync()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return new List<int>();

        var courseIds = await _context.FacultyCourseAssignments
            .Where(f => f.FacultyProfile.IdentityUserId == userId)
            .Select(f => f.CourseId)
            .ToListAsync();

        return courseIds;
    }

    /// <summary>
    /// Checks if the current user has access to a specific course (Admin or Faculty assigned to course).
    /// </summary>
    protected async Task<bool> HasAccessToCourseAsync(int courseId)
    {
        if (User.IsInRole("Admin"))
            return true;

        var facultyCourseIds = await GetFacultyCourseIdsAsync();
        return facultyCourseIds.Contains(courseId);
    }

    /// <summary>
    /// Checks if the current student is enrolled in a specific course.
    /// </summary>
    protected async Task<bool> IsEnrolledInCourseAsync(int courseId)
    {
        var studentProfileId = await GetStudentProfileIdAsync();
        if (!studentProfileId.HasValue)
            return false;

        var isEnrolled = await _context.CourseEnrolments
            .AnyAsync(e => e.CourseId == courseId && e.StudentProfileId == studentProfileId.Value);

        return isEnrolled;
    }
}
