using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.ViewModels;

public class StudentIndexViewModel
{
    public int Id { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int EnrolmentCount { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public class StudentDetailsViewModel
{
    public int Id { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }

    public List<EnrolmentStatusViewModel> Enrolments { get; set; } = new();
    public List<AssignmentResultViewModel> AssignmentResults { get; set; } = new();
    public List<ExamResultViewModel> ExamResults { get; set; } = new();
    public List<AttendanceHistoryViewModel> AttendanceHistory { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}";
}

public class StudentCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; }

    [StringLength(200)]
    public string Address { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime DateOfBirth { get; set; }

    public string PageTitle => Id == 0 ? "Create Student" : "Edit Student";
}

public class EnrolmentStatusViewModel
{
    public int EnrolmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string Status { get; set; }
    public DateTime EnrolmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
}

public class AssignmentResultViewModel
{
    public int ResultId { get; set; }
    public string CourseName { get; set; }
    public string AssignmentName { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string Feedback { get; set; }
    public DateTime SubmissionDate { get; set; }

    public decimal Percentage => MaxScore > 0 ? (Score / MaxScore) * 100 : 0;
}

public class ExamResultViewModel
{
    public int ResultId { get; set; }
    public string CourseName { get; set; }
    public string ExamName { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string Grade { get; set; }
    public DateTime ExamDate { get; set; }
    public bool ResultsReleased { get; set; }

    public decimal Percentage => MaxScore > 0 ? (Score / MaxScore) * 100 : 0;
}

public class AttendanceHistoryViewModel
{
    public string CourseName { get; set; }
    public List<AttendanceRecordViewModel> Records { get; set; } = new();
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }

    public decimal AttendanceRate => TotalSessions > 0 ? ((decimal)PresentCount / TotalSessions) * 100 : 0;
}

public class AttendanceRecordViewModel
{
    public int Id { get; set; }
    public DateTime SessionDate { get; set; }
    public bool IsPresent { get; set; }
}
