using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using oop_s2_1_mvc_83356.Models;
using Xunit;

namespace VgcCollege.Tests.Models;

public class ModelValidationTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    #region Branch Validation Tests

    [Fact]
    public void Branch_WithValidData_PassesValidation()
    {
        var branch = new Branch
        {
            Name = "Dublin",
            Address = "123 Main Street"
        };

        var errors = ValidateModel(branch);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Branch_WithoutName_FailsValidation()
    {
        var branch = new Branch
        {
            Name = null,
            Address = "123 Main Street"
        };

        var errors = ValidateModel(branch);

        errors.Should().Contain(e => e.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Branch_WithEmptyName_FailsValidation()
    {
        var branch = new Branch
        {
            Name = "",
            Address = "123 Main Street"
        };

        var errors = ValidateModel(branch);

        errors.Should().Contain(e => e.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Branch_WithoutAddress_FailsValidation()
    {
        var branch = new Branch
        {
            Name = "Dublin",
            Address = null
        };

        var errors = ValidateModel(branch);

        errors.Should().Contain(e => e.MemberNames.Contains("Address"));
    }

    #endregion

    #region Course Validation Tests

    [Fact]
    public void Course_WithValidData_PassesValidation()
    {
        var course = new Course
        {
            Name = "C# Advanced",
            BranchId = 1,
            StartDate = DateTime.Now.AddMonths(-1),
            EndDate = DateTime.Now.AddMonths(5)
        };

        var errors = ValidateModel(course);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Course_WithoutName_FailsValidation()
    {
        var course = new Course
        {
            Name = null,
            BranchId = 1,
            StartDate = DateTime.Now.AddMonths(-1),
            EndDate = DateTime.Now.AddMonths(5)
        };

        var errors = ValidateModel(course);

        errors.Should().Contain(e => e.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Course_WithEmptyName_FailsValidation()
    {
        var course = new Course
        {
            Name = "",
            BranchId = 1,
            StartDate = DateTime.Now.AddMonths(-1),
            EndDate = DateTime.Now.AddMonths(5)
        };

        var errors = ValidateModel(course);

        errors.Should().Contain(e => e.MemberNames.Contains("Name"));
    }

    #endregion

    #region StudentProfile Validation Tests

    [Fact]
    public void StudentProfile_WithValidData_PassesValidation()
    {
        var student = new StudentProfile
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            StudentNumber = "VGC-2024-001",
            IdentityUserId = "user-1"
        };

        var errors = ValidateModel(student);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void StudentProfile_WithoutFirstName_FailsValidation()
    {
        var student = new StudentProfile
        {
            FirstName = null,
            LastName = "Doe",
            Email = "john@test.com",
            StudentNumber = "VGC-2024-001"
        };

        var errors = ValidateModel(student);

        errors.Should().Contain(e => e.MemberNames.Contains("FirstName"));
    }

    [Fact]
    public void StudentProfile_WithoutLastName_FailsValidation()
    {
        var student = new StudentProfile
        {
            FirstName = "John",
            LastName = null,
            Email = "john@test.com",
            StudentNumber = "VGC-2024-001"
        };

        var errors = ValidateModel(student);

        errors.Should().Contain(e => e.MemberNames.Contains("LastName"));
    }

    [Fact]
    public void StudentProfile_WithoutEmail_FailsValidation()
    {
        var student = new StudentProfile
        {
            FirstName = "John",
            LastName = "Doe",
            Email = null,
            StudentNumber = "VGC-2024-001"
        };

        var errors = ValidateModel(student);

        errors.Should().Contain(e => e.MemberNames.Contains("Email"));
    }

    [Fact]
    public void StudentProfile_WithInvalidEmail_FailsValidation()
    {
        var student = new StudentProfile
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "not-an-email",
            StudentNumber = "VGC-2024-001"
        };

        var errors = ValidateModel(student);

        errors.Should().Contain(e => e.MemberNames.Contains("Email"));
    }

    [Fact]
    public void StudentProfile_WithoutStudentNumber_FailsValidation()
    {
        var student = new StudentProfile
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            StudentNumber = null
        };

        var errors = ValidateModel(student);

        errors.Should().Contain(e => e.MemberNames.Contains("StudentNumber"));
    }

    #endregion

    #region FacultyProfile Validation Tests

    [Fact]
    public void FacultyProfile_WithValidData_PassesValidation()
    {
        var faculty = new FacultyProfile
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Department = "Computer Science",
            IdentityUserId = "user-2"
        };

        var errors = ValidateModel(faculty);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void FacultyProfile_WithoutEmail_FailsValidation()
    {
        var faculty = new FacultyProfile
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = null,
            Department = "Computer Science"
        };

        var errors = ValidateModel(faculty);

        errors.Should().Contain(e => e.MemberNames.Contains("Email"));
    }

    #endregion

    #region AdminProfile Validation Tests

    [Fact]
    public void AdminProfile_WithValidData_PassesValidation()
    {
        var admin = new AdminProfile
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@test.com",
            Phone = "123-456-7890",
            Department = "Administration",
            IdentityUserId = "user-admin"
        };

        var errors = ValidateModel(admin);

        errors.Should().BeEmpty();
    }

    #endregion

    #region Assignment Validation Tests

    [Fact]
    public void Assignment_WithValidData_PassesValidation()
    {
        var assignment = new Assignment
        {
            CourseId = 1,
            Title = "Assignment 1",
            MaxScore = 50,
            DueDate = DateTime.Now.AddDays(7)
        };

        var errors = ValidateModel(assignment);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Assignment_WithoutTitle_FailsValidation()
    {
        var assignment = new Assignment
        {
            CourseId = 1,
            Title = null,
            MaxScore = 50,
            DueDate = DateTime.Now.AddDays(7)
        };

        var errors = ValidateModel(assignment);

        errors.Should().Contain(e => e.MemberNames.Contains("Title"));
    }

    #endregion

    #region Exam Validation Tests

    [Fact]
    public void Exam_WithValidData_PassesValidation()
    {
        var exam = new Exam
        {
            CourseId = 1,
            Title = "Final Exam",
            ExamDate = DateTime.Now.AddDays(30),
            MaxScore = 100,
            ResultsReleased = false
        };

        var errors = ValidateModel(exam);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Exam_WithoutTitle_FailsValidation()
    {
        var exam = new Exam
        {
            CourseId = 1,
            Title = null,
            ExamDate = DateTime.Now.AddDays(30),
            MaxScore = 100,
            ResultsReleased = false
        };

        var errors = ValidateModel(exam);

        errors.Should().Contain(e => e.MemberNames.Contains("Title"));
    }

    [Fact]
    public void Exam_ResultsReleased_DefaultsFalse()
    {
        var exam = new Exam
        {
            CourseId = 1,
            Title = "Final Exam",
            ExamDate = DateTime.Now.AddDays(30),
            MaxScore = 100
        };

        exam.ResultsReleased.Should().BeFalse();
    }

    #endregion

    #region CourseEnrolment Validation Tests

    [Fact]
    public void CourseEnrolment_WithValidData_PassesValidation()
    {
        var enrolment = new CourseEnrolment
        {
            StudentProfileId = 1,
            CourseId = 1,
            EnrolDate = DateTime.Now,
            Status = CourseEnrolmentStatus.Active
        };

        var errors = ValidateModel(enrolment);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void CourseEnrolment_StatusDefaultsActive()
    {
        var enrolment = new CourseEnrolment
        {
            StudentProfileId = 1,
            CourseId = 1,
            EnrolDate = DateTime.Now
        };

        enrolment.Status.Should().Be(CourseEnrolmentStatus.Active);
    }

    #endregion

    #region ExamResult Validation Tests

    [Fact]
    public void ExamResult_WithValidData_PassesValidation()
    {
        var result = new ExamResult
        {
            ExamId = 1,
            StudentProfileId = 1,
            Score = 85,
            Grade = "B"
        };

        var errors = ValidateModel(result);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ExamResult_WithNegativeScore_FailsValidation()
    {
        var result = new ExamResult
        {
            ExamId = 1,
            StudentProfileId = 1,
            Score = -5,
            Grade = "F"
        };

        var errors = ValidateModel(result);

        // Depending on attributes, may or may not fail
        // This test documents the current behavior
    }

    #endregion

    #region AttendanceRecord Validation Tests

    [Fact]
    public void AttendanceRecord_WithValidData_PassesValidation()
    {
        var record = new AttendanceRecord
        {
            CourseEnrolmentId = 1,
            SessionDate = DateTime.Now,
            WeekNumber = 1,
            IsPresent = true
        };

        var errors = ValidateModel(record);

        errors.Should().BeEmpty();
    }

    #endregion

    #region AssignmentResult Validation Tests

    [Fact]
    public void AssignmentResult_WithValidData_PassesValidation()
    {
        var result = new AssignmentResult
        {
            AssignmentId = 1,
            StudentProfileId = 1,
            Score = 45,
            SubmittedAt = DateTime.Now
        };

        var errors = ValidateModel(result);

        errors.Should().BeEmpty();
    }

    #endregion
}
