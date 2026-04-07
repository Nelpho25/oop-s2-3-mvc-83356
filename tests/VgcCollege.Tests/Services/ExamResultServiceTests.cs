using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.Services;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Services;

public class ExamResultServiceTests
{
    private ApplicationDbContext CreateContextWithReleasedExamResultData(string dbName)
    {
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);

        var branch = new Branch { Name = "Dublin", Address = "Dublin, Ireland" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course
        {
            Name = "Advanced C#",
            BranchId = branch.Id,
            StartDate = DateTime.Now.AddMonths(-1),
            EndDate = DateTime.Now.AddMonths(5)
        };
        context.Courses.Add(course);
        context.SaveChanges();

        var student = new StudentProfile
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            StudentNumber = "VGC-2024-001",
            IdentityUserId = "user-1"
        };
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam",
            ExamDate = DateTime.Now.AddDays(-7),
            MaxScore = 100,
            ResultsReleased = true
        };
        context.Exams.Add(exam);
        context.SaveChanges();

        var examResult = new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = student.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(examResult);
        context.SaveChanges();

        return context;
    }

    private ApplicationDbContext CreateContextWithUnreleasedExamResultData(string dbName)
    {
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);

        var branch = new Branch { Name = "Dublin", Address = "Dublin, Ireland" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course
        {
            Name = "Advanced C#",
            BranchId = branch.Id,
            StartDate = DateTime.Now.AddMonths(-1),
            EndDate = DateTime.Now.AddMonths(5)
        };
        context.Courses.Add(course);
        context.SaveChanges();

        var student = new StudentProfile
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            StudentNumber = "VGC-2024-001",
            IdentityUserId = "user-1"
        };
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam",
            ExamDate = DateTime.Now.AddDays(-7),
            MaxScore = 100,
            ResultsReleased = false
        };
        context.Exams.Add(exam);
        context.SaveChanges();

        var examResult = new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = student.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(examResult);
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetVisibleResultsForStudentAsync_WhenResultsReleased_ReturnsResults()
    {
        // Arrange
        var context = CreateContextWithReleasedExamResultData("ExamResults_Released_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();

        // Act
        var visibleResults = await service.GetVisibleResultsForStudentAsync(student.Id);

        // Assert
        visibleResults.Should().HaveCount(1);
        visibleResults[0].Score.Should().Be(85);
        visibleResults[0].StudentProfileId.Should().Be(student.Id);
    }

    [Fact]
    public async Task GetVisibleResultsForStudentAsync_WhenResultsNotReleased_ReturnsEmpty()
    {
        // Arrange
        var context = CreateContextWithUnreleasedExamResultData("ExamResults_Unreleased_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();

        // Act
        var visibleResults = await service.GetVisibleResultsForStudentAsync(student.Id);

        // Assert
        visibleResults.Should().BeEmpty();
    }

    [Fact]
    public async Task CanStudentViewResultAsync_WhenResultsReleased_ReturnsTrue()
    {
        // Arrange
        var context = CreateContextWithReleasedExamResultData("CanView_Released_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var result = context.ExamResults.First();

        // Act
        var canView = await service.CanStudentViewResultAsync(student.Id, result.Id);

        // Assert
        canView.Should().BeTrue();
    }

    [Fact]
    public async Task CanStudentViewResultAsync_WhenResultsNotReleased_ReturnsFalse()
    {
        // Arrange
        var context = CreateContextWithUnreleasedExamResultData("CanView_Unreleased_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var result = context.ExamResults.First();

        // Act
        var canView = await service.CanStudentViewResultAsync(student.Id, result.Id);

        // Assert
        canView.Should().BeFalse();
    }

    [Fact]
    public async Task CanStudentViewResultAsync_WrongStudent_ReturnsFalse()
    {
        // Arrange
        var context = CreateContextWithReleasedExamResultData("CanView_WrongStudent_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var result = context.ExamResults.First();

        // Act - Try to access with different student
        var canView = await service.CanStudentViewResultAsync(999, result.Id);

        // Assert
        canView.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllResultsAsync_ReturnsAllResults()
    {
        // Arrange
        var context = CreateContextWithReleasedExamResultData("GetAll_Results_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        // Act
        var allResults = await service.GetAllResultsAsync();

        // Assert
        allResults.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetResultsByExamAsync_WhenReleased_ReturnsResults()
    {
        // Arrange
        var context = CreateContextWithReleasedExamResultData("GetByExam_Released_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var exam = context.Exams.First();

        // Act
        var results = await service.GetResultsByExamAsync(exam.Id, visibleOnly: true);

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetResultsByExamAsync_WhenNotReleased_ReturnsEmpty()
    {
        // Arrange
        var context = CreateContextWithUnreleasedExamResultData("GetByExam_Unreleased_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var exam = context.Exams.First();

        // Act
        var results = await service.GetResultsByExamAsync(exam.Id, visibleOnly: true);

        // Assert
        results.Should().BeEmpty();
    }
}
