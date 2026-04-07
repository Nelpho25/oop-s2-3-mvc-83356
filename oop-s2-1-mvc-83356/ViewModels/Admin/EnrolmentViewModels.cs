using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.ViewModels;

public class EnrolmentIndexViewModel
{
    public int Id { get; set; }
    public string StudentNumber { get; set; }
    public string StudentName { get; set; }
    public string CourseName { get; set; }
    public string Status { get; set; }
    public DateTime EnrolmentDate { get; set; }
}

public class EnrolmentCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Student is required")]
    public int StudentProfileId { get; set; }

    [Required(ErrorMessage = "Course is required")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Enrolment date is required")]
    [DataType(DataType.DateTime)]
    public DateTime? EnrolmentDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; }

    public List<StudentBasicViewModel> Students { get; set; } = new();
    public List<CourseBasicViewModel> Courses { get; set; } = new();
    public string PageTitle => Id == 0 ? "Create Enrolment" : "Edit Enrolment";
}

public class StudentBasicViewModel
{
    public int Id { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName => $"{StudentNumber} - {FirstName} {LastName}";
}
