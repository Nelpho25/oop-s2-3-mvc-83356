using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public class Assignment
{
    public int Id { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Max score must be greater than or equal to 0")]
    public decimal MaxScore { get; set; }

    public DateTime? DueDate { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;
    public ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
}
