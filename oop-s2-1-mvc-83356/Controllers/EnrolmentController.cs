using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.ViewModels;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Enrolments")]
public class EnrolmentController : BaseController
{
    private readonly ILogger<EnrolmentController> _logger;

    public EnrolmentController(ApplicationDbContext context, ILogger<EnrolmentController> logger) : base(context)
    {
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(int? courseId, int? studentId)
    {
        try
        {
            var query = _context.CourseEnrolments
                .Include(ce => ce.Course)
                .Include(ce => ce.StudentProfile)
                .AsQueryable();

            if (courseId.HasValue)
                query = query.Where(ce => ce.CourseId == courseId);

            if (studentId.HasValue)
                query = query.Where(ce => ce.StudentProfileId == studentId);

            var enrolments = await query
                .Select(ce => new EnrolmentIndexViewModel
                {
                    Id = ce.Id,
                    StudentNumber = ce.StudentProfile.StudentNumber,
                    StudentName = $"{ce.StudentProfile.FirstName} {ce.StudentProfile.LastName}",
                    CourseName = ce.Course.Name,
                    Status = ce.Status.ToString(),
                    EnrolmentDate = ce.EnrolDate
                })
                .ToListAsync();

            _logger.LogInformation("Enrolment index retrieved. Total enrolments: {EnrolmentCount}, CourseId filter: {CourseId}, StudentId filter: {StudentId}", enrolments.Count, courseId, studentId);
            return View(enrolments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolment index");
            TempData["Error"] = "An error occurred while retrieving enrolments.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        try
        {
            var students = await _context.StudentProfiles
                .Select(s => new StudentBasicViewModel
                {
                    Id = s.Id,
                    StudentNumber = s.StudentNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                })
                .OrderBy(s => s.StudentNumber)
                .ToListAsync();

            var courses = await _context.Courses
                .Include(c => c.Branch)
                .Select(c => new CourseBasicViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            var model = new EnrolmentCreateEditViewModel
            {
                EnrolmentDate = DateTime.Now,
                Status = "Active",
                Students = students,
                Courses = courses
            };

            return View("CreateEdit", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrolment creation form");
            TempData["Error"] = "An error occurred while preparing the enrolment form.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(EnrolmentCreateEditViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.Students = await _context.StudentProfiles
                    .Select(s => new StudentBasicViewModel
                    {
                        Id = s.Id,
                        StudentNumber = s.StudentNumber,
                        FirstName = s.FirstName,
                        LastName = s.LastName
                    })
                    .OrderBy(s => s.StudentNumber)
                    .ToListAsync();

                model.Courses = await _context.Courses
                    .Select(c => new CourseBasicViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View("CreateEdit", model);
            }

            // Validate student exists
            var student = await _context.StudentProfiles.FindAsync(model.StudentProfileId);
            if (student == null)
            {
                _logger.LogWarning("Enrolment attempted for non-existent student {StudentId}", model.StudentProfileId);
                ModelState.AddModelError("StudentProfileId", "The selected student does not exist.");
                model.Students = await _context.StudentProfiles
                    .Select(s => new StudentBasicViewModel
                    {
                        Id = s.Id,
                        StudentNumber = s.StudentNumber,
                        FirstName = s.FirstName,
                        LastName = s.LastName
                    })
                    .OrderBy(s => s.StudentNumber)
                    .ToListAsync();

                model.Courses = await _context.Courses
                    .Select(c => new CourseBasicViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View("CreateEdit", model);
            }

            // Validate course exists and get course details
            var course = await _context.Courses.Include(c => c.Branch).FirstOrDefaultAsync(c => c.Id == model.CourseId);
            if (course == null)
            {
                _logger.LogWarning("Enrolment attempted for non-existent course {CourseId}", model.CourseId);
                ModelState.AddModelError("CourseId", "The selected course does not exist.");
                model.Students = await _context.StudentProfiles
                    .Select(s => new StudentBasicViewModel
                    {
                        Id = s.Id,
                        StudentNumber = s.StudentNumber,
                        FirstName = s.FirstName,
                        LastName = s.LastName
                    })
                    .OrderBy(s => s.StudentNumber)
                    .ToListAsync();

                model.Courses = await _context.Courses
                    .Select(c => new CourseBasicViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View("CreateEdit", model);
            }

            // Check for duplicate enrollment
            var existing = await _context.CourseEnrolments
                .FirstOrDefaultAsync(ce => ce.StudentProfileId == model.StudentProfileId && ce.CourseId == model.CourseId);

            if (existing != null)
            {
                _logger.LogWarning("Duplicate enrolment attempted for student {StudentId} in course {CourseId}", model.StudentProfileId, model.CourseId);
                ModelState.AddModelError("", $"Student {student.FirstName} {student.LastName} is already enrolled in {course.Name}.");
                model.Students = await _context.StudentProfiles
                    .Select(s => new StudentBasicViewModel
                    {
                        Id = s.Id,
                        StudentNumber = s.StudentNumber,
                        FirstName = s.FirstName,
                        LastName = s.LastName
                    })
                    .OrderBy(s => s.StudentNumber)
                    .ToListAsync();

                model.Courses = await _context.Courses
                    .Select(c => new CourseBasicViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View("CreateEdit", model);
            }

            // Check if enrolment date is valid (within course dates if specified)
            if (course.StartDate.HasValue && model.EnrolmentDate.HasValue && model.EnrolmentDate.Value < course.StartDate.Value)
            {
                _logger.LogWarning("Enrolment attempted with date before course start for student {StudentId} in course {CourseId}", model.StudentProfileId, model.CourseId);
                ModelState.AddModelError("EnrolmentDate", $"Enrolment date cannot be before course start date ({course.StartDate:MMM dd, yyyy}).");
                model.Students = await _context.StudentProfiles
                    .Select(s => new StudentBasicViewModel
                    {
                        Id = s.Id,
                        StudentNumber = s.StudentNumber,
                        FirstName = s.FirstName,
                        LastName = s.LastName
                    })
                    .OrderBy(s => s.StudentNumber)
                    .ToListAsync();

                model.Courses = await _context.Courses
                    .Select(c => new CourseBasicViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View("CreateEdit", model);
            }

            var statusEnum = Enum.Parse<CourseEnrolmentStatus>(model.Status);

            var enrolment = new CourseEnrolment
            {
                StudentProfileId = model.StudentProfileId,
                CourseId = model.CourseId,
                EnrolDate = model.EnrolmentDate ?? DateTime.UtcNow,
                Status = statusEnum
            };

            _context.CourseEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Enrolment created: Student {StudentId} ({StudentName}) enrolled in course {CourseId} ({CourseName}) by {User}", model.StudentProfileId, $"{student.FirstName} {student.LastName}", model.CourseId, course.Name, User.Identity?.Name);
            TempData["Success"] = $"Enrolment created successfully. Student {student.FirstName} {student.LastName} has been enrolled in {course.Name}.";
            return RedirectToAction("Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error creating enrolment for student {StudentId}", model.StudentProfileId);
            TempData["Error"] = "An error occurred while creating the enrolment. Please try again.";
            model.Students = await _context.StudentProfiles.Select(s => new StudentBasicViewModel { Id = s.Id, StudentNumber = s.StudentNumber, FirstName = s.FirstName, LastName = s.LastName }).OrderBy(s => s.StudentNumber).ToListAsync();
            model.Courses = await _context.Courses.Select(c => new CourseBasicViewModel { Id = c.Id, Name = c.Name, StartDate = c.StartDate, EndDate = c.EndDate }).OrderBy(c => c.Name).ToListAsync();
            return View("CreateEdit", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating enrolment");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("{id}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var enrolment = await _context.CourseEnrolments
            .Include(ce => ce.StudentProfile)
            .Include(ce => ce.Course)
            .FirstOrDefaultAsync(ce => ce.Id == id);

        if (enrolment == null)
            return NotFound();

        var students = await _context.StudentProfiles
            .Select(s => new StudentBasicViewModel
            {
                Id = s.Id,
                StudentNumber = s.StudentNumber,
                FirstName = s.FirstName,
                LastName = s.LastName
            })
            .ToListAsync();

        var courses = await _context.Courses
            .Select(c => new CourseBasicViewModel
            {
                Id = c.Id,
                Name = c.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            })
            .ToListAsync();

        var model = new EnrolmentCreateEditViewModel
        {
            Id = enrolment.Id,
            StudentProfileId = enrolment.StudentProfileId,
            CourseId = enrolment.CourseId,
            EnrolmentDate = enrolment.EnrolDate,
            Status = enrolment.Status.ToString(),
            Students = students,
            Courses = courses
        };

        return View("CreateEdit", model);
    }

    [HttpPost("{id}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int id, EnrolmentCreateEditViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            model.Students = await _context.StudentProfiles
                .Select(s => new StudentBasicViewModel
                {
                    Id = s.Id,
                    StudentNumber = s.StudentNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                })
                .ToListAsync();

            model.Courses = await _context.Courses
                .Select(c => new CourseBasicViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                })
                .ToListAsync();

            return View("CreateEdit", model);
        }

        var enrolment = await _context.CourseEnrolments.FirstOrDefaultAsync(ce => ce.Id == id);

        if (enrolment == null)
            return NotFound();

        var statusEnum = Enum.Parse<CourseEnrolmentStatus>(model.Status);

        enrolment.Status = statusEnum;
        enrolment.EnrolDate = model.EnrolmentDate ?? DateTime.UtcNow;

        _context.CourseEnrolments.Update(enrolment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Enrolment updated successfully.";
        return RedirectToAction("Index");
    }

    [HttpGet("{id}/Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var enrolment = await _context.CourseEnrolments
            .Include(ce => ce.StudentProfile)
            .Include(ce => ce.Course)
            .FirstOrDefaultAsync(ce => ce.Id == id);

        if (enrolment == null)
            return NotFound();

        return View(new EnrolmentIndexViewModel
        {
            Id = enrolment.Id,
            StudentNumber = enrolment.StudentProfile.StudentNumber,
            StudentName = $"{enrolment.StudentProfile.FirstName} {enrolment.StudentProfile.LastName}",
            CourseName = enrolment.Course.Name,
            Status = enrolment.Status.ToString(),
            EnrolmentDate = enrolment.EnrolDate
        });
    }

    [HttpPost("{id}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var enrolment = await _context.CourseEnrolments.FirstOrDefaultAsync(ce => ce.Id == id);

        if (enrolment == null)
            return NotFound();

        _context.CourseEnrolments.Remove(enrolment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Enrolment deleted successfully.";
        return RedirectToAction("Index");
    }
}
