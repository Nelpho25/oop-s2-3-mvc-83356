using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Accounts")]
public class AccountManagementController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountManagementController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : base(context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.ToListAsync();
        var userViewModels = new List<UserManagementViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Unknown";

            userViewModels.Add(new UserManagementViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                UserRole = userRole,
                CreatedAt = user.CreatedAt,
                IsActive = !user.LockoutEnd.HasValue || user.LockoutEnd < DateTime.UtcNow
            });
        }

        return View(userViewModels.OrderByDescending(u => u.CreatedAt).ToList());
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var currentRole = roles.FirstOrDefault() ?? "Student";

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            CurrentRole = currentRole,
            AvailableRoles = new[] { "Admin", "Faculty", "Student" },
            CreatedAt = user.CreatedAt,
            IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow
        };

        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = new[] { "Admin", "Faculty", "Student" };
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Update display name
        user.DisplayName = model.DisplayName;
        await _userManager.UpdateAsync(user);

        // Handle role change
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(model.NewRole))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.NewRole);

            // Create or remove profile based on role
            await HandleProfileCreation(user, model.NewRole);
        }

        // Handle lock/unlock
        if (model.IsLocked && (!user.LockoutEnd.HasValue || user.LockoutEnd <= DateTime.UtcNow))
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
        }
        else if (!model.IsLocked && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var model = new DeleteUserViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            UserRole = roles.FirstOrDefault() ?? "Unknown"
        };

        return View(model);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Get the user's role to clean up associated profile
        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault();

        // Remove associated profile
        if (userRole == "Student")
        {
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(sp => sp.IdentityUserId == id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
            }
        }
        else if (userRole == "Faculty")
        {
            var facultyProfile = await _context.FacultyProfiles.FirstOrDefaultAsync(fp => fp.IdentityUserId == id);
            if (facultyProfile != null)
            {
                _context.FacultyProfiles.Remove(facultyProfile);
            }
        }
        else if (userRole == "Admin")
        {
            var adminProfile = await _context.AdminProfiles.FirstOrDefaultAsync(ap => ap.IdentityUserId == id);
            if (adminProfile != null)
            {
                _context.AdminProfiles.Remove(adminProfile);
            }
        }

        await _context.SaveChangesAsync();

        // Delete the user
        await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Index));
    }

    private async Task HandleProfileCreation(ApplicationUser user, string newRole)
    {
        var faker = new Bogus.Faker();

        if (newRole == "Student")
        {
            // Remove other profiles
            var facultyProfile = await _context.FacultyProfiles.FirstOrDefaultAsync(fp => fp.IdentityUserId == user.Id);
            if (facultyProfile != null)
                _context.FacultyProfiles.Remove(facultyProfile);

            var adminProfile = await _context.AdminProfiles.FirstOrDefaultAsync(ap => ap.IdentityUserId == user.Id);
            if (adminProfile != null)
                _context.AdminProfiles.Remove(adminProfile);

            // Create student profile if doesn't exist
            var existingStudent = await _context.StudentProfiles.FirstOrDefaultAsync(sp => sp.IdentityUserId == user.Id);
            if (existingStudent == null)
            {
                var names = user.DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var studentProfile = new StudentProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = names.Length > 0 ? names[0] : "Student",
                    LastName = names.Length > 1 ? names[1] : user.DisplayName,
                    StudentNumber = $"VGC-{DateTime.UtcNow.Year}-{user.Id.Substring(0, 8).ToUpper()}",
                    Email = user.Email!,
                    Phone = faker.Person.Phone,
                    Address = faker.Address.FullAddress(),
                    DateOfBirth = faker.Person.DateOfBirth
                };
                _context.StudentProfiles.Add(studentProfile);
            }
        }
        else if (newRole == "Faculty")
        {
            // Remove other profiles
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(sp => sp.IdentityUserId == user.Id);
            if (studentProfile != null)
                _context.StudentProfiles.Remove(studentProfile);

            var adminProfile = await _context.AdminProfiles.FirstOrDefaultAsync(ap => ap.IdentityUserId == user.Id);
            if (adminProfile != null)
                _context.AdminProfiles.Remove(adminProfile);

            // Create faculty profile if doesn't exist
            var existingFaculty = await _context.FacultyProfiles.FirstOrDefaultAsync(fp => fp.IdentityUserId == user.Id);
            if (existingFaculty == null)
            {
                var names = user.DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var facultyProfile = new FacultyProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = names.Length > 0 ? names[0] : "Faculty",
                    LastName = names.Length > 1 ? names[1] : user.DisplayName,
                    Email = user.Email!,
                    Phone = faker.Person.Phone,
                    Department = faker.Commerce.Department()
                };
                _context.FacultyProfiles.Add(facultyProfile);
            }
        }
        else if (newRole == "Admin")
        {
            // Remove other profiles
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(sp => sp.IdentityUserId == user.Id);
            if (studentProfile != null)
                _context.StudentProfiles.Remove(studentProfile);

            var facultyProfile = await _context.FacultyProfiles.FirstOrDefaultAsync(fp => fp.IdentityUserId == user.Id);
            if (facultyProfile != null)
                _context.FacultyProfiles.Remove(facultyProfile);

            // Create admin profile if doesn't exist
            var existingAdmin = await _context.AdminProfiles.FirstOrDefaultAsync(ap => ap.IdentityUserId == user.Id);
            if (existingAdmin == null)
            {
                var names = user.DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var adminProfile = new AdminProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = names.Length > 0 ? names[0] : "Admin",
                    LastName = names.Length > 1 ? names[1] : user.DisplayName,
                    Email = user.Email!,
                    Phone = faker.Person.Phone,
                    Department = "Administration"
                };
                _context.AdminProfiles.Add(adminProfile);
            }
        }

        await _context.SaveChangesAsync();
    }
}

public class UserManagementViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display Name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Display(Name = "Current Role")]
    public string CurrentRole { get; set; } = string.Empty;

    [Display(Name = "New Role")]
    [Required(ErrorMessage = "Please select a role")]
    public string NewRole { get; set; } = string.Empty;

    public string[] AvailableRoles { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Lock Account")]
    public bool IsLocked { get; set; }
}

public class DeleteUserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
}
