using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public class ExamResult
{
    public int Id { get; set; }

    [ForeignKey(nameof(Exam))]
    public int ExamId { get; set; }

    [ForeignKey(nameof(StudentProfile))]
    public int StudentProfileId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Score must be greater than or equal to 0")]
    public decimal? Score { get; set; }

    [MaxLength(10)]
    public string? Grade { get; set; }

    // Navigation properties
    public Exam Exam { get; set; } = null!;
    public StudentProfile StudentProfile { get; set; } = null!;
}
