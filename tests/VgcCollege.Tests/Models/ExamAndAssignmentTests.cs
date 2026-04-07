using oop_s2_1_mvc_83356.Models;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Models;

public class AssignmentTests
{
    [Fact]
    public void CreateAssignment_WithValidData_ShouldSucceed()
    {
        // Arrange
        var assignment = new Assignment
        {
            CourseId = 1,
            Title = "Midterm Project",
            MaxScore = 100m,
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        // Act & Assert
        Assert.NotNull(assignment);
        Assert.Equal("Midterm Project", assignment.Title);
        Assert.Equal(100m, assignment.MaxScore);
    }

    [Fact]
    public void SaveAssignment_ToDatabase_ShouldPersist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("AssignmentTest");

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        var course = new Course { Name = "CS101", BranchId = 1 };

        context.Branches.Add(branch);
        context.SaveChanges();

        course.BranchId = branch.Id;
        context.Courses.Add(course);
        context.SaveChanges();

        var assignment = new Assignment
        {
            CourseId = course.Id,
            Title = "Lab Assignment 1",
            MaxScore = 50m,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        context.Assignments.Add(assignment);
        context.SaveChanges();

        // Assert
        var savedAssignment = context.Assignments.FirstOrDefault(a => a.Title == "Lab Assignment 1");
        Assert.NotNull(savedAssignment);
        Assert.Equal(50m, savedAssignment.MaxScore);
    }
}

public class ExamTests
{
    [Fact]
    public void CreateExam_WithValidData_ShouldSucceed()
    {
        // Arrange
        var exam = new Exam
        {
            CourseId = 1,
            Title = "Final Examination",
            ExamDate = DateTime.UtcNow.AddDays(45),
            MaxScore = 100m,
            ResultsReleased = false
        };

        // Act & Assert
        Assert.NotNull(exam);
        Assert.Equal("Final Examination", exam.Title);
        Assert.False(exam.ResultsReleased);
    }

    [Fact]
    public void SaveExam_ToDatabase_ShouldPersist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("ExamTest");

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        var course = new Course { Name = "CS101", BranchId = 1 };

        context.Branches.Add(branch);
        context.SaveChanges();

        course.BranchId = branch.Id;
        context.Courses.Add(course);
        context.SaveChanges();

        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam - CS101",
            ExamDate = DateTime.UtcNow.AddDays(60),
            MaxScore = 100m,
            ResultsReleased = false
        };

        // Act
        context.Exams.Add(exam);
        context.SaveChanges();

        // Assert
        var savedExam = context.Exams.FirstOrDefault(e => e.Title == "Final Exam - CS101");
        Assert.NotNull(savedExam);
        Assert.Equal(course.Id, savedExam.CourseId);
    }

    [Fact]
    public void ExamResults_CanBeReleased()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("ExamReleaseTest");

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        var course = new Course { Name = "CS101", BranchId = 1 };

        context.Branches.Add(branch);
        context.SaveChanges();

        course.BranchId = branch.Id;
        context.Courses.Add(course);
        context.SaveChanges();

        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam",
            MaxScore = 100m,
            ResultsReleased = false
        };

        context.Exams.Add(exam);
        context.SaveChanges();

        // Act
        exam.ResultsReleased = true;
        context.SaveChanges();

        // Assert
        var updatedExam = context.Exams.First(e => e.Id == exam.Id);
        Assert.True(updatedExam.ResultsReleased);
    }
}

public class ExamResultTests
{
    [Fact]
    public void CreateExamResult_WithValidData_ShouldSucceed()
    {
        // Arrange
        var result = new ExamResult
        {
            ExamId = 1,
            StudentProfileId = 1,
            Score = 85m,
            Grade = "B"
        };

        // Act & Assert
        Assert.NotNull(result);
        Assert.Equal(85m, result.Score);
        Assert.Equal("B", result.Grade);
    }

    [Fact]
    public void SaveExamResult_ToDatabase_ShouldPersist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("ExamResultTest");

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        var course = new Course { Name = "CS101", BranchId = 1 };
        var student = new StudentProfile
        {
            IdentityUserId = "user-123",
            FirstName = "Alice",
            LastName = "Brown",
            StudentNumber = "VGC-2026-A001",
            Email = "alice@vgc.ie"
        };

        context.Branches.Add(branch);
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        course.BranchId = branch.Id;
        context.Courses.Add(course);
        context.SaveChanges();

        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam",
            MaxScore = 100m,
            ResultsReleased = true
        };

        context.Exams.Add(exam);
        context.SaveChanges();

        var result = new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = student.Id,
            Score = 92m,
            Grade = "A"
        };

        // Act
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Assert
        var savedResult = context.ExamResults.FirstOrDefault(r => 
            r.ExamId == exam.Id && r.StudentProfileId == student.Id);
        Assert.NotNull(savedResult);
        Assert.Equal(92m, savedResult.Score);
        Assert.Equal("A", savedResult.Grade);
    }
}
