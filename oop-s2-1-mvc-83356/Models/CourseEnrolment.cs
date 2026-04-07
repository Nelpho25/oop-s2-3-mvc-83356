using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public enum CourseEnrolmentStatus
{
    Active,
    Withdrawn,
    Completed
}

public class CourseEnrolment
{
    public int Id { get; set; }

    [ForeignKey(nameof(StudentProfile))]
    public int StudentProfileId { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }

    public DateTime EnrolDate { get; set; } = DateTime.UtcNow;

    [Required]
    public CourseEnrolmentStatus Status { get; set; } = CourseEnrolmentStatus.Active;

    // Navigation properties
    public StudentProfile StudentProfile { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}
