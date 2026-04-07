using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public class AssignmentResult
{
    public int Id { get; set; }

    [ForeignKey(nameof(Assignment))]
    public int AssignmentId { get; set; }

    [ForeignKey(nameof(StudentProfile))]
    public int StudentProfileId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Score must be greater than or equal to 0")]
    public decimal? Score { get; set; }

    [MaxLength(1000)]
    public string? Feedback { get; set; }

    public DateTime? SubmittedAt { get; set; }

    // Navigation properties
    public Assignment Assignment { get; set; } = null!;
    public StudentProfile StudentProfile { get; set; } = null!;
}
