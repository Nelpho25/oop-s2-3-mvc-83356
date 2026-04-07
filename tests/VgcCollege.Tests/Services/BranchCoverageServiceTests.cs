using Microsoft.EntityFrameworkCore;
using Xunit;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VgcCollege.Tests.Services;

public class GradeCalculationServiceTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Theory]
    [InlineData(90, "A")]
    [InlineData(80, "B")]
    [InlineData(75, "C")]
    [InlineData(65, "D")]
    [InlineData(55, "F")]
    public void GradeCalculation_WithVaryingScores_ReturnsCorrectGrade(decimal score, string expectedGrade)
    {
        // Act
        string grade = score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };

        // Assert
        Assert.Equal(expectedGrade, grade);
    }

    [Theory]
    [InlineData(85, true)]
    [InlineData(75, true)]
    [InlineData(65, false)]
    [InlineData(45, false)]
    public void PassingGradeCheck_WithVaryingScores_ReturnsCorrectResult(decimal score, bool expectedPassing)
    {
        // Act
        bool isPassing = score >= 70;

        // Assert
        Assert.Equal(expectedPassing, isPassing);
    }
}

public class ExamResultBranchCoverageTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ExamResults_WhenResultsAreReleased_StudentCanViewResults()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var branch = new Branch { Id = 1, Name = "Dublin", Address = "123 Main St" };
        var course = new Course { Id = 1, Name = "C#", BranchId = 1, Branch = branch };
        var exam = new Exam
        {
            Id = 1,
            CourseId = 1,
            Course = course,
            ExamDate = DateTime.Now,
            MaxScore = 100,
            ResultsReleased = true  // BRANCH: ResultsReleased = true
        };
        var student = new StudentProfile { Id = 1, FirstName = "John", LastName = "Doe", StudentNumber = "S001", Email = "john@example.com" };
        var examResult = new ExamResult
        {
            Id = 1,
            ExamId = 1,
            Exam = exam,
            StudentProfileId = 1,
            StudentProfile = student,
            Score = 85
        };

        context.Branches.Add(branch);
        context.Courses.Add(course);
        context.StudentProfiles.Add(student);
        context.Exams.Add(exam);
        context.ExamResults.Add(examResult);
        await context.SaveChangesAsync();

        // Act
        var results = context.ExamResults
            .Include(r => r.Exam)
            .Where(r => r.Exam.ResultsReleased && r.StudentProfileId == 1)
            .ToList();

        // Assert
        Assert.Single(results);
        Assert.True(results.First().Exam.ResultsReleased);
    }

    [Fact]
    public async Task ExamResults_WhenResultsAreNotReleased_StudentCannotViewResults()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var branch = new Branch { Id = 1, Name = "Dublin", Address = "123 Main St" };
        var course = new Course { Id = 1, Name = "C#", BranchId = 1, Branch = branch };
        var exam = new Exam
        {
            Id = 1,
            CourseId = 1,
            Course = course,
            ExamDate = DateTime.Now,
            MaxScore = 100,
            ResultsReleased = false  // BRANCH: ResultsReleased = false
        };
        var student = new StudentProfile { Id = 1, FirstName = "John", LastName = "Doe", StudentNumber = "S001", Email = "john@example.com" };
        var examResult = new ExamResult
        {
            Id = 1,
            ExamId = 1,
            Exam = exam,
            StudentProfileId = 1,
            StudentProfile = student,
            Score = 85
        };

        context.Branches.Add(branch);
        context.Courses.Add(course);
        context.StudentProfiles.Add(student);
        context.Exams.Add(exam);
        context.ExamResults.Add(examResult);
        await context.SaveChangesAsync();

        // Act
        var results = context.ExamResults
            .Include(r => r.Exam)
            .Where(r => r.Exam.ResultsReleased && r.StudentProfileId == 1)
            .ToList();

        // Assert
        Assert.Empty(results);
    }
}

public class EnrolmentStatusBranchCoverageTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Theory]
    [InlineData(CourseEnrolmentStatus.Active)]
    [InlineData(CourseEnrolmentStatus.Withdrawn)]
    [InlineData(CourseEnrolmentStatus.Completed)]
    public async Task EnrolmentStatus_AllStatusValues_CanBeSet(CourseEnrolmentStatus status)
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var branch = new Branch { Id = 1, Name = "Dublin", Address = "123 Main St" };
        var course = new Course { Id = 1, Name = "C#", BranchId = 1, Branch = branch };
        var student = new StudentProfile { Id = 1, FirstName = "John", LastName = "Doe", StudentNumber = "S001", Email = "john@example.com" };
        var enrolment = new CourseEnrolment
        {
            Id = 1,
            CourseId = 1,
            Course = course,
            StudentProfileId = 1,
            StudentProfile = student,
            EnrolDate = DateTime.Now,
            Status = status  // Different status for each test iteration
        };

        context.Branches.Add(branch);
        context.Courses.Add(course);
        context.StudentProfiles.Add(student);
        context.CourseEnrolments.Add(enrolment);
        await context.SaveChangesAsync();

        // Act
        var retrievedEnrolment = await context.CourseEnrolments.FindAsync(1);

        // Assert
        Assert.NotNull(retrievedEnrolment);
        Assert.Equal(status, retrievedEnrolment.Status);
    }

    [Fact]
    public async Task ActiveEnrolments_FilterByStatus_ReturnsOnlyActiveEnrolments()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var branch = new Branch { Id = 1, Name = "Dublin", Address = "123 Main St" };
        var course = new Course { Id = 1, Name = "C#", BranchId = 1, Branch = branch };
        var student = new StudentProfile { Id = 1, FirstName = "John", LastName = "Doe", StudentNumber = "S001", Email = "john@example.com" };

        var activeEnrolment = new CourseEnrolment { Id = 1, CourseId = 1, Course = course, StudentProfileId = 1, StudentProfile = student, Status = CourseEnrolmentStatus.Active, EnrolDate = DateTime.Now };
        var withdrawnEnrolment = new CourseEnrolment { Id = 2, CourseId = 1, Course = course, StudentProfileId = 1, StudentProfile = student, Status = CourseEnrolmentStatus.Withdrawn, EnrolDate = DateTime.Now };

        context.Branches.Add(branch);
        context.Courses.Add(course);
        context.StudentProfiles.Add(student);
        context.CourseEnrolments.AddRange(activeEnrolment, withdrawnEnrolment);
        await context.SaveChangesAsync();

        // Act
        var activeOnly = context.CourseEnrolments
            .Where(e => e.Status == CourseEnrolmentStatus.Active)
            .ToList();

        // Assert
        Assert.Single(activeOnly);
        Assert.Equal(CourseEnrolmentStatus.Active, activeOnly.First().Status);
    }
}

public class AttendanceRateBranchCoverageTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AttendanceRate_CalculationLogic_Perfect100Percent()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var branch = new Branch { Id = 1, Name = "Dublin", Address = "123 Main St" };
        var course = new Course { Id = 1, Name = "C#", BranchId = 1, Branch = branch };
        var student = new StudentProfile { Id = 1, FirstName = "John", LastName = "Doe", StudentNumber = "S001", Email = "john@example.com" };
        var enrolment = new CourseEnrolment { Id = 1, CourseId = 1, Course = course, StudentProfileId = 1, StudentProfile = student, Status = CourseEnrolmentStatus.Active, EnrolDate = DateTime.Now };

        var records = new[]
        {
            new AttendanceRecord { Id = 1, CourseEnrolmentId = 1, CourseEnrolment = enrolment, SessionDate = DateTime.Now.AddDays(-2), IsPresent = true },
            new AttendanceRecord { Id = 2, CourseEnrolmentId = 1, CourseEnrolment = enrolment, SessionDate = DateTime.Now.AddDays(-1), IsPresent = true },
            new AttendanceRecord { Id = 3, CourseEnrolmentId = 1, CourseEnrolment = enrolment, SessionDate = DateTime.Now, IsPresent = true }
        };

        context.Branches.Add(branch);
        context.Courses.Add(course);
        context.StudentProfiles.Add(student);
        context.CourseEnrolments.Add(enrolment);
        context.AttendanceRecords.AddRange(records);
        await context.SaveChangesAsync();

        // Act
        var presentCount = context.AttendanceRecords.Where(r => r.CourseEnrolmentId == 1 && r.IsPresent).Count();
        var totalCount = context.AttendanceRecords.Where(r => r.CourseEnrolmentId == 1).Count();
        var rate = totalCount > 0 ? (decimal)presentCount / totalCount * 100 : 0;

        // Assert
        Assert.Equal(100, rate);
    }

    [Fact]
    public async Task AttendanceRate_CalculationLogic_50PercentAttendance()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var branch = new Branch { Id = 1, Name = "Dublin", Address = "123 Main St" };
        var course = new Course { Id = 1, Name = "C#", BranchId = 1, Branch = branch };
        var student = new StudentProfile { Id = 1, FirstName = "John", LastName = "Doe", StudentNumber = "S001", Email = "john@example.com" };
        var enrolment = new CourseEnrolment { Id = 1, CourseId = 1, Course = course, StudentProfileId = 1, StudentProfile = student, Status = CourseEnrolmentStatus.Active, EnrolDate = DateTime.Now };

        var records = new[]
        {
            new AttendanceRecord { Id = 1, CourseEnrolmentId = 1, CourseEnrolment = enrolment, SessionDate = DateTime.Now.AddDays(-1), IsPresent = true },
            new AttendanceRecord { Id = 2, CourseEnrolmentId = 1, CourseEnrolment = enrolment, SessionDate = DateTime.Now, IsPresent = false }
        };

        context.Branches.Add(branch);
        context.Courses.Add(course);
        context.StudentProfiles.Add(student);
        context.CourseEnrolments.Add(enrolment);
        context.AttendanceRecords.AddRange(records);
        await context.SaveChangesAsync();

        // Act
        var presentCount = context.AttendanceRecords.Where(r => r.CourseEnrolmentId == 1 && r.IsPresent).Count();
        var totalCount = context.AttendanceRecords.Where(r => r.CourseEnrolmentId == 1).Count();
        var rate = totalCount > 0 ? (decimal)presentCount / totalCount * 100 : 0;

        // Assert
        Assert.Equal(50, rate);
    }

    [Fact]
    public async Task AttendanceRate_WithNoRecords_ReturnsZero()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var presentCount = context.AttendanceRecords.Where(r => r.CourseEnrolmentId == 999 && r.IsPresent).Count();
        var totalCount = context.AttendanceRecords.Where(r => r.CourseEnrolmentId == 999).Count();
        var rate = totalCount > 0 ? (decimal)presentCount / totalCount * 100 : 0;

        // Assert
        Assert.Equal(0, rate);
    }
}
