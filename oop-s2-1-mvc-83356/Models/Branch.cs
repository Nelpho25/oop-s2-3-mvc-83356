using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.Models;

public class Branch
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
