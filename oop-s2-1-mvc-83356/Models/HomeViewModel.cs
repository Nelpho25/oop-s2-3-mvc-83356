using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.ViewModels;

public class HomeViewModel
{
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalBranches { get; set; }
    public int TotalFaculty { get; set; }
    public int TotalEnrolments { get; set; }

    public List<RecentCourseDto> RecentCourses { get; set; } = new();
    public List<TopStudentDto> TopStudents { get; set; } = new();
    public List<BranchDto> Branches { get; set; } = new();

    public string WelcomeRole { get; set; } = string.Empty;
}

public class RecentCourseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int EnrolledStudentsCount { get; set; }
}

public class TopStudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public decimal AverageGrade { get; set; }
    public int CoursesCount { get; set; }
}

public class BranchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int CoursesCount { get; set; }
}
