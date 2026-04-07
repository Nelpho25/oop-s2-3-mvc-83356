using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.ViewModels;

public class CourseIndexViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string BranchName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int EnrolledCount { get; set; }
}

public class CourseDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string BranchName { get; set; }
    public int BranchId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public List<FacultyAssignmentViewModel> AssignedFaculty { get; set; } = new();
    public List<CourseEnrolmentStudentViewModel> EnrolledStudents { get; set; } = new();
    public List<AssignmentBasicViewModel> Assignments { get; set; } = new();
    public List<ExamBasicViewModel> Exams { get; set; } = new();

    // For assigning faculty
    public int? SelectedFacultyId { get; set; }
    public List<FacultyBasicViewModel> AvailableFaculty { get; set; } = new();
}

public class CourseCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Course name must be between 2 and 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Branch is required")]
    public int BranchId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.DateTime)]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [DataType(DataType.DateTime)]
    public DateTime? EndDate { get; set; }

    public List<BranchBasicViewModel> Branches { get; set; } = new();
    public string PageTitle => Id == 0 ? "Create Course" : "Edit Course";
}

public class BranchBasicViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class FacultyAssignmentViewModel
{
    public int FacultyProfileId { get; set; }
    public string FacultyName { get; set; }
    public string Email { get; set; }
    public bool IsTutor { get; set; }
}

public class FacultyBasicViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public class CourseEnrolmentStudentViewModel
{
    public int EnrolmentId { get; set; }
    public int StudentProfileId { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public class AssignmentBasicViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal MaxScore { get; set; }
    public DateTime? DueDate { get; set; }
}

public class ExamBasicViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime ExamDate { get; set; }
    public decimal MaxScore { get; set; }
    public bool ResultsReleased { get; set; }
}
