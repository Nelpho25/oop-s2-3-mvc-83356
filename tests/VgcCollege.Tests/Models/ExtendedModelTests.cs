using Xunit;
using FluentAssertions;
using oop_s2_1_mvc_83356.Models;

namespace VgcCollege.Tests.Models;

public class FacultyCourseAssignmentTests
{
    [Fact]
    public void CreateFacultyCourseAssignment_WithValidData_ShouldSucceed()
    {
        var assignment = new FacultyCourseAssignment
        {
            Id = 1,
            FacultyProfileId = 1,
            CourseId = 1
        };

        assignment.Should().NotBeNull();
        assignment.FacultyProfileId.Should().Be(1);
        assignment.CourseId.Should().Be(1);
    }

    [Fact]
    public void FacultyCourseAssignment_DefaultValues()
    {
        var assignment = new FacultyCourseAssignment();
        assignment.FacultyProfileId.Should().Be(0);
        assignment.CourseId.Should().Be(0);
    }
}

public class ApplicationUserExtendedTests
{
    [Fact]
    public void ApplicationUser_StoresDisplayName()
    {
        var user = new ApplicationUser { DisplayName = "John Doe" };
        user.DisplayName.Should().Be("John Doe");
    }

    [Fact]
    public void ApplicationUser_CanHaveAdminProfile()
    {
        var user = new ApplicationUser { Id = "user1" };
        var adminProfile = new AdminProfile
        {
            IdentityUserId = user.Id,
            FirstName = "John"
        };
        user.AdminProfile = adminProfile;

        user.AdminProfile.Should().NotBeNull();
        user.AdminProfile.IdentityUserId.Should().Be(user.Id);
    }
}

public class ExamExtendedTests
{
    [Fact]
    public void Exam_StoresExamDate()
    {
        var examDate = DateTime.UtcNow.AddDays(30);
        var exam = new Exam { ExamDate = examDate };
        exam.ExamDate.Should().Be(examDate);
    }

    [Fact]
    public void Exam_TracksResultsReleasedStatus()
    {
        var exam = new Exam { ResultsReleased = false };
        exam.ResultsReleased = true;
        exam.ResultsReleased.Should().BeTrue();
    }

    [Fact]
    public void Exam_DefaultResultsNotReleased()
    {
        var exam = new Exam();
        exam.ResultsReleased.Should().BeFalse();
    }
}

public class AttendanceRecordExtendedTests
{
    [Fact]
    public void AttendanceRecord_StoresSessionDate()
    {
        var sessionDate = DateTime.UtcNow;
        var record = new AttendanceRecord { SessionDate = sessionDate };
        record.SessionDate.Should().Be(sessionDate);
    }

    [Fact]
    public void AttendanceRecord_StoresWeekNumber()
    {
        var record = new AttendanceRecord { WeekNumber = 5 };
        record.WeekNumber.Should().Be(5);
    }

    [Fact]
    public void AttendanceRecord_TracksPresenceStatus()
    {
        var record = new AttendanceRecord { IsPresent = true };
        record.IsPresent.Should().BeTrue();
    }
}

public class CourseEnrolmentExtendedTests
{
    [Fact]
    public void CourseEnrolment_StoresEnrolDate()
    {
        var enrolDate = DateTime.UtcNow;
        var enrolment = new CourseEnrolment { EnrolDate = enrolDate };
        enrolment.EnrolDate.Should().Be(enrolDate);
    }

    [Fact]
    public void CourseEnrolment_TrackesStatus()
    {
        var enrolment = new CourseEnrolment { Status = CourseEnrolmentStatus.Active };
        enrolment.Status.Should().Be(CourseEnrolmentStatus.Active);
    }

    [Fact]
    public void CourseEnrolment_CanBeWithdrawn()
    {
        var enrolment = new CourseEnrolment { Status = CourseEnrolmentStatus.Active };
        enrolment.Status = CourseEnrolmentStatus.Withdrawn;
        enrolment.Status.Should().Be(CourseEnrolmentStatus.Withdrawn);
    }

    [Fact]
    public void CourseEnrolment_CanBeCompleted()
    {
        var enrolment = new CourseEnrolment { Status = CourseEnrolmentStatus.Active };
        enrolment.Status = CourseEnrolmentStatus.Completed;
        enrolment.Status.Should().Be(CourseEnrolmentStatus.Completed);
    }
}

public class ExamResultExtendedTests
{
    [Fact]
    public void ExamResult_StoresScore()
    {
        var score = 45m;
        var result = new ExamResult { Score = score };
        result.Score.Should().Be(score);
    }

    [Fact]
    public void ExamResult_StoresGrade()
    {
        var grade = "A";
        var result = new ExamResult { Grade = grade };
        result.Grade.Should().Be(grade);
    }

    [Fact]
    public void ExamResult_CanHaveNullScore()
    {
        var result = new ExamResult { Score = null };
        result.Score.Should().BeNull();
    }
}

public class AssignmentResultExtendedTests
{
    [Fact]
    public void AssignmentResult_StoresScore()
    {
        var score = 45m;
        var result = new AssignmentResult { Score = score };
        result.Score.Should().Be(score);
    }

    [Fact]
    public void AssignmentResult_StoresSubmittedAt()
    {
        var submittedAt = DateTime.UtcNow;
        var result = new AssignmentResult { SubmittedAt = submittedAt };
        result.SubmittedAt.Should().Be(submittedAt);
    }

    [Fact]
    public void AssignmentResult_StoresFeedback()
    {
        var feedback = "Good work!";
        var result = new AssignmentResult { Feedback = feedback };
        result.Feedback.Should().Be(feedback);
    }
}
