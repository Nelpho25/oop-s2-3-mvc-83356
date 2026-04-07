using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.ViewModels;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Courses")]
public class CourseController : BaseController
{
    private readonly ILogger<CourseController> _logger;

    public CourseController(ApplicationDbContext context, ILogger<CourseController> logger) : base(context)
    {
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var courses = await _context.Courses
            .Include(c => c.Branch)
            .Include(c => c.CourseEnrolments)
            .Select(c => new CourseIndexViewModel
            {
                Id = c.Id,
                Name = c.Name,
                BranchName = c.Branch.Name,
                StartDate = c.StartDate ?? DateTime.Now,
                EndDate = c.EndDate ?? DateTime.Now.AddMonths(3),
                EnrolledCount = c.CourseEnrolments.Count
            })
            .ToListAsync();

            _logger.LogInformation("Course index retrieved. Total courses: {CourseCount}", courses.Count);
            return View(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course index");
            TempData["Error"] = "An error occurred while retrieving courses.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("{id}/Details")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var course = await _context.Courses
            .Include(c => c.Branch)
            .Include(c => c.FacultyCourseAssignments)
                .ThenInclude(fca => fca.FacultyProfile)
            .Include(c => c.CourseEnrolments)
                .ThenInclude(ce => ce.StudentProfile)
            .Include(c => c.Assignments)
            .Include(c => c.Exams)
            .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                _logger.LogWarning("Course details requested for non-existent course {CourseId}", id);
                return NotFound();
            }

            var availableFaculty = await _context.FacultyProfiles
                .Where(f => !f.FacultyCourseAssignments.Any(fca => fca.CourseId == id))
                .Select(f => new FacultyBasicViewModel
                {
                    Id = f.Id,
                    FirstName = f.FirstName,
                    LastName = f.LastName,
                    Email = f.Email
                })
                .ToListAsync();

        var model = new CourseDetailsViewModel
        {
            Id = course.Id,
            Name = course.Name,
            BranchName = course.Branch.Name,
            BranchId = course.BranchId,
            StartDate = course.StartDate ?? DateTime.Now,
            EndDate = course.EndDate ?? DateTime.Now.AddMonths(3),
            AssignedFaculty = course.FacultyCourseAssignments.Select(fca => new FacultyAssignmentViewModel
            {
                FacultyProfileId = fca.FacultyProfileId,
                FacultyName = $"{fca.FacultyProfile.FirstName} {fca.FacultyProfile.LastName}",
                Email = fca.FacultyProfile.Email,
                IsTutor = fca.IsTutor
            }).ToList(),
            EnrolledStudents = course.CourseEnrolments.Select(ce => new CourseEnrolmentStudentViewModel
            {
                EnrolmentId = ce.Id,
                StudentProfileId = ce.StudentProfileId,
                StudentNumber = ce.StudentProfile.StudentNumber,
                FirstName = ce.StudentProfile.FirstName,
                LastName = ce.StudentProfile.LastName,
                Email = ce.StudentProfile.Email,
                Status = ce.Status.ToString()
            }).ToList(),
            Assignments = course.Assignments.Select(a => new AssignmentBasicViewModel
            {
                Id = a.Id,
                Name = a.Title,
                MaxScore = a.MaxScore,
                DueDate = a.DueDate ?? DateTime.Now
            }).ToList(),
            Exams = course.Exams.Select(e => new ExamBasicViewModel
            {
                Id = e.Id,
                Name = e.Title,
                ExamDate = e.ExamDate ?? DateTime.Now,
                MaxScore = e.MaxScore,
                ResultsReleased = e.ResultsReleased
            }).ToList(),
            AvailableFaculty = availableFaculty
        };

            _logger.LogInformation("Course {CourseId} details retrieved", id);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course details for {CourseId}", id);
            TempData["Error"] = "An error occurred while retrieving course details.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost("{id}/AssignFaculty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignFaculty(int id, int facultyId, bool isTutor = false)
    {
        try
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
            {
                _logger.LogWarning("Faculty assignment attempted for non-existent course {CourseId}", id);
                return NotFound();
            }

            var faculty = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.Id == facultyId);
            if (faculty == null)
            {
                _logger.LogWarning("Faculty assignment with non-existent faculty {FacultyId} for course {CourseId}", facultyId, id);
                return BadRequest("Faculty not found");
            }

            var existing = await _context.FacultyCourseAssignments
                .FirstOrDefaultAsync(fca => fca.CourseId == id && fca.FacultyProfileId == facultyId);

            if (existing != null)
            {
                _logger.LogWarning("Duplicate faculty assignment attempted for faculty {FacultyId} in course {CourseId}", facultyId, id);
                TempData["Error"] = "Faculty already assigned to this course.";
                return RedirectToAction("Details", new { id });
            }

                var assignment = new FacultyCourseAssignment
                {
                    CourseId = id,
                    FacultyProfileId = facultyId,
                    IsTutor = isTutor
                };

                _context.FacultyCourseAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Faculty {FacultyId} assigned to course {CourseId} by {User}", facultyId, id, User.Identity?.Name);
                TempData["Success"] = $"Faculty assigned successfully.";
                return RedirectToAction("Details", new { id });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error assigning faculty {FacultyId} to course {CourseId}", facultyId, id);
                TempData["Error"] = "An error occurred while assigning faculty. Please try again.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error assigning faculty {FacultyId} to course {CourseId}", facultyId, id);
                return RedirectToAction("Error", "Home");
            }
        }

    [HttpPost("{courseId}/RemoveFaculty/{facultyId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFaculty(int courseId, int facultyId)
    {
        var assignment = await _context.FacultyCourseAssignments
            .FirstOrDefaultAsync(fca => fca.CourseId == courseId && fca.FacultyProfileId == facultyId);

        if (assignment == null)
            return NotFound();

        _context.FacultyCourseAssignments.Remove(assignment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Faculty removed from course.";
        return RedirectToAction("Details", new { id = courseId });
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var branches = await _context.Branches
            .Select(b => new BranchBasicViewModel { Id = b.Id, Name = b.Name })
            .ToListAsync();

        var model = new CourseCreateEditViewModel
        {
            Branches = branches,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(3)
        };

        return View("CreateEdit", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(CourseCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Branches = await _context.Branches
                .Select(b => new BranchBasicViewModel { Id = b.Id, Name = b.Name })
                .ToListAsync();
            return View("CreateEdit", model);
        }

        var course = new Course
        {
            Name = model.Name,
            BranchId = model.BranchId,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Course '{course.Name}' created successfully.";
        return RedirectToAction("Index");
    }

    [HttpGet("{id}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        var branches = await _context.Branches
            .Select(b => new BranchBasicViewModel { Id = b.Id, Name = b.Name })
            .ToListAsync();

        var model = new CourseCreateEditViewModel
        {
            Id = course.Id,
            Name = course.Name,
            BranchId = course.BranchId,
            StartDate = course.StartDate ?? DateTime.Now,
            EndDate = course.EndDate ?? DateTime.Now.AddMonths(3),
            Branches = branches
        };

        return View("CreateEdit", model);
    }

    [HttpPost("{id}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int id, CourseCreateEditViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            model.Branches = await _context.Branches
                .Select(b => new BranchBasicViewModel { Id = b.Id, Name = b.Name })
                .ToListAsync();
            return View("CreateEdit", model);
        }

        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        course.Name = model.Name;
        course.BranchId = model.BranchId;
        course.StartDate = model.StartDate;
        course.EndDate = model.EndDate;

        _context.Courses.Update(course);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Course '{course.Name}' updated successfully.";
        return RedirectToAction("Index");
    }

    [HttpGet("{id}/Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses
            .Include(c => c.CourseEnrolments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        if (course.CourseEnrolments.Any())
        {
            TempData["Error"] = "Cannot delete course with enrolled students.";
            return RedirectToAction("Details", new { id });
        }

        var branches = await _context.Branches
            .Select(b => new BranchBasicViewModel { Id = b.Id, Name = b.Name })
            .ToListAsync();

        var model = new CourseCreateEditViewModel
        {
            Id = course.Id,
            Name = course.Name,
            BranchId = course.BranchId,
            StartDate = course.StartDate ?? DateTime.Now,
            EndDate = course.EndDate ?? DateTime.Now.AddMonths(3),
            Branches = branches
        };

        return View(model);
    }

    [HttpPost("{id}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses
            .Include(c => c.CourseEnrolments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        if (course.CourseEnrolments.Any())
        {
            TempData["Error"] = "Cannot delete course with enrolled students.";
            return RedirectToAction("Details", new { id });
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Course '{course.Name}' deleted successfully.";
        return RedirectToAction("Index");
    }
}
