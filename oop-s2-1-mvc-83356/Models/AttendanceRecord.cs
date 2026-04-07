using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public class AttendanceRecord
{
    public int Id { get; set; }

    [ForeignKey(nameof(CourseEnrolment))]
    public int CourseEnrolmentId { get; set; }

    public DateTime SessionDate { get; set; }

    public int WeekNumber { get; set; }

    public bool IsPresent { get; set; } = false;

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation property
    public CourseEnrolment CourseEnrolment { get; set; } = null!;
}
