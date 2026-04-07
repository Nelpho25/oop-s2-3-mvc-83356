using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_s2_1_mvc_83356.Models;

public class FacultyCourseAssignment
{
    public int Id { get; set; }

    [ForeignKey(nameof(FacultyProfile))]
    public int FacultyProfileId { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }

    public bool IsTutor { get; set; } = false;

    // Navigation properties
    public FacultyProfile FacultyProfile { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
