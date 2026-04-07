using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Faculty")]
[Route("Faculty")]
public class FacultyController : BaseController
{
    public FacultyController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var facultyProfile = await _context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.IdentityUserId == userId);

        // If faculty profile doesn't exist, create a default one
        if (facultyProfile == null)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                facultyProfile = new FacultyProfile
                {
                    IdentityUserId = userId,
                    FirstName = user.DisplayName?.Split(' ')[0] ?? "Faculty",
                    LastName = user.DisplayName?.Split(' ').Length > 1 ? user.DisplayName!.Split(' ')[1] : "",
                    Email = user.Email ?? "",
                    Phone = "",
                    Department = "Department Not Specified"
                };

                _context.FacultyProfiles.Add(facultyProfile);
                await _context.SaveChangesAsync();
            }
        }

        var courses = await _context.FacultyCourseAssignments
            .Where(fca => fca.FacultyProfileId == facultyProfile.Id)
            .Include(fca => fca.Course)
            .Select(fca => fca.Course)
            .ToListAsync();

        var stats = new FacultyDashboardViewModel
        {
            FacultyName = $"{facultyProfile.FirstName} {facultyProfile.LastName}",
            Department = facultyProfile.Department,
            AssignedCourses = courses.Count,
            TutorCourses = await _context.FacultyCourseAssignments
                .CountAsync(fca => fca.FacultyProfileId == facultyProfile.Id && fca.IsTutor),
            TotalStudents = await _context.CourseEnrolments
                .Where(e => courses.Select(c => c.Id).Contains(e.CourseId))
                .Select(e => e.StudentProfileId)
                .Distinct()
                .CountAsync(),
            Courses = courses
        };

        return View(stats);
    }
}

public class FacultyDashboardViewModel
{
    public string FacultyName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public int AssignedCourses { get; set; }
    public int TutorCourses { get; set; }
    public int TotalStudents { get; set; }
    public List<Models.Course> Courses { get; set; } = new();
}
