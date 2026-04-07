using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public class Exam
{
    public int Id { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public DateTime? ExamDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Max score must be greater than or equal to 0")]
    public decimal MaxScore { get; set; }

    public bool ResultsReleased { get; set; } = false;

    // Navigation properties
    public Course Course { get; set; } = null!;
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}
