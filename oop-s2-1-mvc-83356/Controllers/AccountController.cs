using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.Controllers;

[Route("Account")]
public class AccountController : BaseController
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        ApplicationDbContext context,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger)
        : base(context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return await RedirectBasedOnRole();
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {model.Email} logged in at {DateTime.UtcNow}");
                return RedirectToLocal(returnUrl) ?? await RedirectBasedOnRole();
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning($"User account locked out: {model.Email}");
                return RedirectToAction(nameof(Lockout));
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpGet("Lockout")]
    [AllowAnonymous]
    public IActionResult Lockout()
    {
        return View();
    }

    [HttpGet("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation($"User {User.Identity?.Name} logged out at {DateTime.UtcNow}");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> LogoutPost()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation($"User {User.Identity?.Name} logged out at {DateTime.UtcNow}");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var profileViewModel = new ProfileViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToList()
        };

        return View(profileViewModel);
    }

    [HttpGet("AccessDenied")]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet("Register")]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign Student role by default
                await _userManager.AddToRoleAsync(user, "Student");

                _logger.LogInformation($"User {model.Email} registered successfully");

                // Create student profile
                var studentProfile = new StudentProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    StudentNumber = GenerateStudentNumber()
                };

                _context.StudentProfiles.Add(studentProfile);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    private IActionResult? RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return null;
    }

    private async Task<IActionResult> RedirectBasedOnRole()
    {
        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "Admin");
        else if (User.IsInRole("Faculty"))
            return RedirectToAction("Index", "Faculty");
        else if (User.IsInRole("Student"))
            return RedirectToAction("Dashboard", "StudentDashboard");
        else
            return RedirectToAction("Index", "Home");
    }

    private string GenerateStudentNumber()
    {
        return $"VGC-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N")[..4].ToUpper()}";
    }
}

public class LoginViewModel
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Display Name")]
    public string DisplayName => $"{FirstName} {LastName}";
}

public class ProfileViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}
