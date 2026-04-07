using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public class StudentProfile
{
    public int Id { get; set; }

    [ForeignKey(nameof(ApplicationUser))]
    public string IdentityUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    // Navigation properties
    public ApplicationUser ApplicationUser { get; set; } = null!;
    public ICollection<CourseEnrolment> CourseEnrolments { get; set; } = new List<CourseEnrolment>();
    public ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}
