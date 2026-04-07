using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                // Get counts
                var totalStudents = await _context.StudentProfiles.CountAsync();
                var totalCourses = await _context.Courses.CountAsync();
                var totalBranches = await _context.Branches.CountAsync();
                var totalFaculty = await _context.FacultyProfiles.CountAsync();
                var totalEnrolments = await _context.CourseEnrolments.CountAsync();

                // Get recent courses with enrollments
                var recentCourses = await _context.Courses
                    .Include(c => c.Branch)
                    .Include(c => c.CourseEnrolments)
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        branchName = c.Branch.Name,
                        startDate = c.StartDate,
                        endDate = c.EndDate,
                        enrolledCount = c.CourseEnrolments.Count
                    })
                    .ToListAsync();

                // Get branches with course counts
                var branches = await _context.Branches
                    .Include(b => b.Courses)
                    .Select(b => new
                    {
                        id = b.Id,
                        name = b.Name,
                        address = b.Address,
                        coursesCount = b.Courses.Count
                    })
                    .ToListAsync();

                // Get top students by average grade
                var studentGrades = await _context.StudentProfiles
                    .Include(s => s.CourseEnrolments)
                    .ToListAsync();

                var topStudents = studentGrades
                    .Select(s => new
                    {
                        id = s.Id,
                        studentNumber = s.StudentNumber,
                        firstName = s.FirstName,
                        lastName = s.LastName,
                        averageGrade = _context.ExamResults
                            .Where(er => er.StudentProfileId == s.Id && er.Exam.ResultsReleased)
                            .Any() ? _context.ExamResults
                                .Where(er => er.StudentProfileId == s.Id && er.Exam.ResultsReleased)
                                .Average(er => er.Score) : 0m,
                        coursesCount = s.CourseEnrolments.Count
                    })
                    .OrderByDescending(x => x.averageGrade)
                    .Take(3)
                    .ToList();

                return Ok(new
                {
                    totalStudents,
                    totalCourses,
                    totalBranches,
                    totalFaculty,
                    totalEnrolments,
                    recentCourses,
                    branches,
                    topStudents
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
