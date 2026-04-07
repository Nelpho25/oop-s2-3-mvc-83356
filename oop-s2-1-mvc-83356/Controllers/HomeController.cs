using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.ViewModels;

namespace oop_s2_1_mvc_83356.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Redirect authenticated users based on role
        if (User.Identity?.IsAuthenticated ?? false)
        {
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("MyProfile", "StudentProfile");
            }
            else if (User.IsInRole("Faculty"))
            {
                return RedirectToAction("MyCourses", "Course");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        // Build view model for unauthenticated or admin users
        var viewModel = new HomeViewModel();

        try
        {
            viewModel.TotalStudents = await _context.StudentProfiles.CountAsync();
            viewModel.TotalCourses = await _context.Courses.CountAsync();
            viewModel.TotalBranches = await _context.Branches.CountAsync();
            viewModel.TotalFaculty = await _context.FacultyProfiles.CountAsync();
            viewModel.TotalEnrolments = await _context.CourseEnrolments.CountAsync();

            // Get recent 5 courses
            viewModel.RecentCourses = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.CourseEnrolments)
                .OrderByDescending(c => c.Id)
                .Take(5)
                .Select(c => new RecentCourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    BranchName = c.Branch.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    EnrolledStudentsCount = c.CourseEnrolments.Count
                })
                .ToListAsync();

            // Get top 3 students by average grade
            var studentGrades = await _context.StudentProfiles
                .Include(s => s.CourseEnrolments)
                .ToListAsync();

            var topStudents = studentGrades
                .Select(s => new
                {
                    Student = s,
                    AverageGrade = _context.ExamResults
                        .Where(er => er.StudentProfileId == s.Id && er.Exam.ResultsReleased)
                        .Any() ? _context.ExamResults
                            .Where(er => er.StudentProfileId == s.Id && er.Exam.ResultsReleased)
                            .Average(er => er.Score) : 0m,
                    CoursesCount = s.CourseEnrolments.Count
                })
                .OrderByDescending(x => x.AverageGrade)
                .Take(3)
                .ToList();

            viewModel.TopStudents = topStudents.Select(sg => new TopStudentDto
            {
                Id = sg.Student.Id,
                FirstName = sg.Student.FirstName,
                LastName = sg.Student.LastName,
                StudentNumber = sg.Student.StudentNumber,
                AverageGrade = sg.AverageGrade > 0 ? Math.Round(sg.AverageGrade, 1) : 0,
                CoursesCount = sg.CoursesCount
            }).ToList();

            // Get branches with course counts
            viewModel.Branches = await _context.Branches
                .Include(b => b.Courses)
                .Select(b => new BranchDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Address = b.Address,
                    CoursesCount = b.Courses.Count
                })
                .ToListAsync();

            if (User.IsInRole("Admin"))
            {
                viewModel.WelcomeRole = "Admin";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page data");
        }

        return View(viewModel);
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        _logger.LogInformation("Error page accessed with status code: {StatusCode}", statusCode);

        return statusCode switch
        {
            404 => View("Error404", new ErrorViewModel { StatusCode = 404 }),
            403 => View("Error403", new ErrorViewModel { StatusCode = 403 }),
            _ => View("Error500", new ErrorViewModel { StatusCode = statusCode ?? 500 })
        };
    }
}

public class ErrorViewModel
{
    public int StatusCode { get; set; }
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
