using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : BaseController
{
    public AdminController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var stats = new AdminDashboardViewModel
        {
            TotalBranches = await _context.Branches.CountAsync(),
            TotalCourses = await _context.Courses.CountAsync(),
            TotalStudents = await _context.StudentProfiles.CountAsync(),
            TotalFaculty = await _context.FacultyProfiles.CountAsync(),
            TotalEnrolments = await _context.CourseEnrolments.CountAsync(),
            ActiveEnrolments = await _context.CourseEnrolments
                .CountAsync(e => e.Status == Models.CourseEnrolmentStatus.Active)
        };

        return View(stats);
    }
}

public class AdminDashboardViewModel
{
    public int TotalBranches { get; set; }
    public int TotalCourses { get; set; }
    public int TotalStudents { get; set; }
    public int TotalFaculty { get; set; }
    public int TotalEnrolments { get; set; }
    public int ActiveEnrolments { get; set; }
}
