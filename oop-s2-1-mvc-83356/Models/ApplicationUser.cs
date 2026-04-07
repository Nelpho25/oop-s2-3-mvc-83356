using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public StudentProfile? StudentProfile { get; set; }
    public FacultyProfile? FacultyProfile { get; set; }
    public AdminProfile? AdminProfile { get; set; }
}
