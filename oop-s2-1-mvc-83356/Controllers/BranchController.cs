using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.ViewModels;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Branches")]
public class BranchController : BaseController
{
    private readonly ILogger<BranchController> _logger;

    public BranchController(ApplicationDbContext context, ILogger<BranchController> logger) : base(context)
    {
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var branches = await _context.Branches
            .Include(b => b.Courses)
            .Select(b => new BranchIndexViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                CourseCount = b.Courses.Count
            })
            .ToListAsync();

            _logger.LogInformation("Branch index retrieved. Total branches: {BranchCount}", branches.Count);
            return View(branches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branch index");
            TempData["Error"] = "An error occurred while retrieving branches.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("{id}/Details")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var branch = await _context.Branches
                .Include(b => b.Courses)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                _logger.LogWarning("Branch details requested for non-existent branch {BranchId}", id);
                return NotFound();
            }

            var model = new BranchDetailsViewModel
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Courses = branch.Courses.Select(c => new CourseBasicViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    StartDate = c.StartDate ?? DateTime.Now,
                    EndDate = c.EndDate ?? DateTime.Now.AddMonths(3)
                }).ToList()
            };

            _logger.LogInformation("Branch {BranchId} details retrieved", id);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branch details for {BranchId}", id);
            TempData["Error"] = "An error occurred while retrieving branch details.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View("CreateEdit", new BranchCreateEditViewModel());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(BranchCreateEditViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View("CreateEdit", model);

            var branch = new Branch
            {
                Name = model.Name,
                Address = model.Address
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Branch {BranchName} created successfully by {User}", branch.Name, User.Identity?.Name);
            TempData["Success"] = $"Branch '{branch.Name}' created successfully.";
            return RedirectToAction("Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error creating branch");
            TempData["Error"] = "An error occurred while saving the branch. Please try again.";
            return View("CreateEdit", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating branch");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("{id}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                _logger.LogWarning("Branch edit requested for non-existent branch {BranchId}", id);
                return NotFound();
            }

            var model = new BranchCreateEditViewModel
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address
            };

            return View("CreateEdit", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branch for editing {BranchId}", id);
            TempData["Error"] = "An error occurred while retrieving the branch.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost("{id}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int id, BranchCreateEditViewModel model)
    {
        try
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("CreateEdit", model);

            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                _logger.LogWarning("Branch edit requested for non-existent branch {BranchId}", id);
                return NotFound();
            }

            branch.Name = model.Name;
            branch.Address = model.Address;

            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Branch {BranchId} updated successfully by {User}", id, User.Identity?.Name);
            TempData["Success"] = $"Branch '{branch.Name}' updated successfully.";
            return RedirectToAction("Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating branch {BranchId}", id);
            TempData["Error"] = "An error occurred while updating the branch. Please try again.";
            return View("CreateEdit", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating branch {BranchId}", id);
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("{id}/Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var branch = await _context.Branches
                .Include(b => b.Courses)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                _logger.LogWarning("Branch delete requested for non-existent branch {BranchId}", id);
                return NotFound();
            }

            if (branch.Courses.Any())
            {
                _logger.LogWarning("Delete attempted for branch {BranchId} with associated courses", id);
                TempData["Error"] = "Cannot delete branch with associated courses.";
                return RedirectToAction("Details", new { id });
            }

            var model = new BranchCreateEditViewModel
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branch for deletion {BranchId}", id);
            TempData["Error"] = "An error occurred while preparing branch deletion.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost("{id}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var branch = await _context.Branches
                .Include(b => b.Courses)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                _logger.LogWarning("Branch deletion requested for non-existent branch {BranchId}", id);
                return NotFound();
            }

            if (branch.Courses.Any())
            {
                _logger.LogWarning("Delete attempted for branch {BranchId} with {CourseCount} courses", id, branch.Courses.Count);
                TempData["Error"] = "Cannot delete branch with associated courses.";
                return RedirectToAction("Details", new { id });
            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Branch {BranchId} ({BranchName}) deleted successfully by {User}", id, branch.Name, User.Identity?.Name);
            TempData["Success"] = $"Branch '{branch.Name}' deleted successfully.";
            return RedirectToAction("Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting branch {BranchId}", id);
            TempData["Error"] = "An error occurred while deleting the branch. Please try again.";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting branch {BranchId}", id);
            return RedirectToAction("Error", "Home");
        }
    }
}
