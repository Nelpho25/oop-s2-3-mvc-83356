using System.ComponentModel.DataAnnotations;

namespace oop_s2_1_mvc_83356.ViewModels;

public class AttendanceIndexViewModel
{
    public int EnrolmentId { get; set; }
    public string StudentNumber { get; set; }
    public string StudentName { get; set; }
    public string CourseName { get; set; }
    
    public List<AttendanceWeekViewModel> Weeks { get; set; } = new();
    
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    
    public decimal AttendanceRate => TotalSessions > 0 ? ((decimal)PresentCount / TotalSessions) * 100 : 0;
}

public class AttendanceWeekViewModel
{
    public int WeekNumber { get; set; }
    public DateTime WeekStart { get; set; }
    public List<AttendanceSessionViewModel> Sessions { get; set; } = new();
}

public class AttendanceSessionViewModel
{
    public int RecordId { get; set; }
    public DateTime SessionDate { get; set; }
    public bool IsPresent { get; set; }
    public string DayOfWeek => SessionDate.ToString("dddd");
}

public class ExamIndexViewModel
{
    public int Id { get; set; }
    public string CourseName { get; set; }
    public string ExamName { get; set; }
    public DateTime ExamDate { get; set; }
    public decimal MaxScore { get; set; }
    public bool ResultsReleased { get; set; }
    public int SubmissionsCount { get; set; }
}

public class ExamReleaseToggleViewModel
{
    public int Id { get; set; }
    public string ExamName { get; set; }
    public bool ResultsReleased { get; set; }
}
