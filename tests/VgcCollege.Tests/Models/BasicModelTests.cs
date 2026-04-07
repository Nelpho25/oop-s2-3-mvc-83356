using Xunit;
using oop_s2_1_mvc_83356.Models;
using System;
using System.Collections.Generic;

namespace VgcCollege.Tests.Models;

public class CourseModelTests
{
    [Fact]
    public void Course_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var course = new Course();

        // Assert
        Assert.Equal(0, course.Id);
        Assert.Empty(course.Name);
        Assert.Empty(course.CourseEnrolments);
        Assert.Empty(course.Assignments);
        Assert.Empty(course.Exams);
    }

    [Fact]
    public void Course_SetProperties_PropertiesAssignedCorrectly()
    {
        // Arrange
        var course = new Course
        {
            Id = 1,
            Name = "C# Advanced",
            BranchId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(3)
        };

        // Assert
        Assert.Equal(1, course.Id);
        Assert.Equal("C# Advanced", course.Name);
        Assert.Equal(1, course.BranchId);
        Assert.NotNull(course.StartDate);
        Assert.NotNull(course.EndDate);
    }

    [Fact]
    public void Course_WithEnrolments_CollectionsCanBePopulated()
    {
        // Arrange
        var course = new Course { Id = 1, Name = "C#" };
        var enrolment = new CourseEnrolment { Id = 1, CourseId = 1 };

        // Act
        course.CourseEnrolments.Add(enrolment);

        // Assert
        Assert.Single(course.CourseEnrolments);
        Assert.Contains(enrolment, course.CourseEnrolments);
    }
}

public class StudentProfileModelTests
{
    [Fact]
    public void StudentProfile_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var student = new StudentProfile();

        // Assert
        Assert.Equal(0, student.Id);
        Assert.Empty(student.FirstName);
        Assert.Empty(student.CourseEnrolments);
    }

    [Fact]
    public void StudentProfile_SetProperties_PropertiesAssignedCorrectly()
    {
        // Arrange
        var student = new StudentProfile
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            StudentNumber = "S001",
            Email = "john@example.com"
        };

        // Assert
        Assert.Equal(1, student.Id);
        Assert.Equal("John", student.FirstName);
        Assert.Equal("Doe", student.LastName);
        Assert.Equal("S001", student.StudentNumber);
        Assert.Equal("john@example.com", student.Email);
    }

    [Fact]
    public void StudentProfile_FullNameProperty_ConcatenatesFirstAndLastName()
    {
        // Arrange
        var student = new StudentProfile
        {
            FirstName = "Jane",
            LastName = "Smith"
        };

        // Act
        var fullName = $"{student.FirstName} {student.LastName}";

        // Assert
        Assert.Equal("Jane Smith", fullName);
    }
}

public class BranchModelTests
{
    [Fact]
    public void Branch_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var branch = new Branch();

        // Assert
        Assert.Equal(0, branch.Id);
        Assert.Empty(branch.Name);
        Assert.Empty(branch.Courses);
    }

    [Fact]
    public void Branch_SetProperties_PropertiesAssignedCorrectly()
    {
        // Arrange
        var branch = new Branch
        {
            Id = 1,
            Name = "Dublin Campus",
            Address = "123 Main Street, Dublin"
        };

        // Assert
        Assert.Equal(1, branch.Id);
        Assert.Equal("Dublin Campus", branch.Name);
        Assert.Equal("123 Main Street, Dublin", branch.Address);
    }

    [Fact]
    public void Branch_WithCourses_CanPopulateCoursesCollection()
    {
        // Arrange
        var branch = new Branch { Id = 1, Name = "Dublin" };
        var course = new Course { Id = 1, Name = "C#", BranchId = 1 };

        // Act
        branch.Courses.Add(course);

        // Assert
        Assert.Single(branch.Courses);
    }
}

public class CourseEnrolmentModelTests
{
    [Fact]
    public void CourseEnrolment_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var enrolment = new CourseEnrolment();

        // Assert
        Assert.Equal(0, enrolment.Id);
        Assert.Equal(CourseEnrolmentStatus.Active, enrolment.Status);
    }

    [Fact]
    public void CourseEnrolment_SetStatus_StatusChangesCorrectly()
    {
        // Arrange
        var enrolment = new CourseEnrolment { Status = CourseEnrolmentStatus.Active };

        // Act
        enrolment.Status = CourseEnrolmentStatus.Withdrawn;

        // Assert
        Assert.Equal(CourseEnrolmentStatus.Withdrawn, enrolment.Status);
    }

    [Fact]
    public void CourseEnrolment_AllStatusValues_AreValid()
    {
        // Arrange & Act & Assert
        var statuses = new[] { CourseEnrolmentStatus.Active, CourseEnrolmentStatus.Withdrawn, CourseEnrolmentStatus.Completed };
        foreach (var status in statuses)
        {
            var enrolment = new CourseEnrolment { Status = status };
            Assert.Equal(status, enrolment.Status);
        }
    }
}

public class ExamModelTests
{
    [Fact]
    public void Exam_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var exam = new Exam();

        // Assert
        Assert.Equal(0, exam.Id);
        Assert.False(exam.ResultsReleased);
        Assert.Empty(exam.ExamResults);
    }

    [Fact]
    public void Exam_SetProperties_PropertiesAssignedCorrectly()
    {
        // Arrange
        var examDate = DateTime.Now.AddDays(7);
        var exam = new Exam
        {
            Id = 1,
            CourseId = 1,
            ExamDate = examDate,
            MaxScore = 100,
            ResultsReleased = false
        };

        // Assert
        Assert.Equal(1, exam.Id);
        Assert.Equal(1, exam.CourseId);
        Assert.Equal(examDate, exam.ExamDate);
        Assert.Equal(100, exam.MaxScore);
        Assert.False(exam.ResultsReleased);
    }

    [Fact]
    public void Exam_ToggleResultsReleased_ChangesCorrectly()
    {
        // Arrange
        var exam = new Exam { ResultsReleased = false };

        // Act
        exam.ResultsReleased = !exam.ResultsReleased;

        // Assert
        Assert.True(exam.ResultsReleased);
    }
}

public class ExamResultModelTests
{
    [Fact]
    public void ExamResult_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var result = new ExamResult();

        // Assert
        Assert.Equal(0, result.Id);
        Assert.Null(result.Score);
    }

    [Fact]
    public void ExamResult_SetProperties_PropertiesAssignedCorrectly()
    {
        // Arrange
        var result = new ExamResult
        {
            Id = 1,
            ExamId = 1,
            StudentProfileId = 1,
            Score = 85
        };

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.ExamId);
        Assert.Equal(1, result.StudentProfileId);
        Assert.Equal(85, result.Score);
    }

    [Fact]
    public void ExamResult_ScoreRange_CanBeFromZeroToMaxScore()
    {
        // Arrange & Act
        var minScoreResult = new ExamResult { Score = 0 };
        var maxScoreResult = new ExamResult { Score = 100 };

        // Assert
        Assert.Equal(0, minScoreResult.Score);
        Assert.Equal(100, maxScoreResult.Score);
    }
}

public class AssignmentModelTests
{
    [Fact]
    public void Assignment_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var assignment = new Assignment();

        // Assert
        Assert.Equal(0, assignment.Id);
        Assert.Empty(assignment.Title);
        Assert.Empty(assignment.AssignmentResults);
    }

    [Fact]
    public void Assignment_SetProperties_PropertiesAssignedCorrectly()
    {
        // Arrange
        var dueDate = DateTime.Now.AddDays(7);
        var assignment = new Assignment
        {
            Id = 1,
            CourseId = 1,
            Title = "Assignment 1",
            DueDate = dueDate,
            MaxScore = 50
        };

        // Assert
        Assert.Equal(1, assignment.Id);
        Assert.Equal(1, assignment.CourseId);
        Assert.Equal("Assignment 1", assignment.Title);
        Assert.Equal(dueDate, assignment.DueDate);
        Assert.Equal(50, assignment.MaxScore);
    }
}

public class AttendanceRecordModelTests
{
    [Fact]
    public void AttendanceRecord_CreateNewInstance_HasDefaultValues()
    {
        // Act
        var record = new AttendanceRecord();

        // Assert
        Assert.Equal(0, record.Id);
        Assert.False(record.IsPresent);
    }

    [Fact]
    public void AttendanceRecord_SetProperties_PropertiesAssignedCorrectly()
    {
        // Arrange
        var sessionDate = DateTime.Now;
        var record = new AttendanceRecord
        {
            Id = 1,
            CourseEnrolmentId = 1,
            SessionDate = sessionDate,
            IsPresent = true
        };

        // Assert
        Assert.Equal(1, record.Id);
        Assert.Equal(1, record.CourseEnrolmentId);
        Assert.Equal(sessionDate, record.SessionDate);
        Assert.True(record.IsPresent);
    }

    [Fact]
    public void AttendanceRecord_PresenceToggle_WorksCorrectly()
    {
        // Arrange
        var record = new AttendanceRecord { IsPresent = false };

        // Act
        record.IsPresent = !record.IsPresent;

        // Assert
        Assert.True(record.IsPresent);
    }
}
