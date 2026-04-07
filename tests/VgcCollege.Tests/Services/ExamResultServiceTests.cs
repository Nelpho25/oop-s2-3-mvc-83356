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
    [Fact]
    public async Task GetVisibleResultsForStudentAsync_WhenResultsReleased_ReturnsResults()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        // Update exam to released
        var exam = context.Exams.First();
        exam.ResultsReleased = true;
        context.SaveChanges();

        // Add exam result for student
        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act
        var visibleResults = await service.GetVisibleResultsForStudentAsync(1);

        // Assert
        visibleResults.Should().HaveCount(1);
        visibleResults[0].Score.Should().Be(85);
        visibleResults[0].StudentProfileId.Should().Be(1);
    }

    [Fact]
    public async Task GetVisibleResultsForStudentAsync_WhenResultsNotReleased_ReturnsEmpty()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        // Exam has ResultsReleased = false by default in seed data

        // Add exam result for student
        var exam = context.Exams.First();
        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act
        var visibleResults = await service.GetVisibleResultsForStudentAsync(1);

        // Assert
        visibleResults.Should().BeEmpty();
    }

    [Fact]
    public async Task CanStudentViewResultAsync_WhenResultsReleased_ReturnsTrue()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        // Update exam to released
        var exam = context.Exams.First();
        exam.ResultsReleased = true;
        context.SaveChanges();

        // Add exam result
        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act
        var canView = await service.CanStudentViewResultAsync(1, result.Id);

        // Assert
        canView.Should().BeTrue();
    }

    [Fact]
    public async Task CanStudentViewResultAsync_WhenResultsNotReleased_ReturnsFalse()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        // Add exam result (results not released)
        var exam = context.Exams.First();
        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act
        var canView = await service.CanStudentViewResultAsync(1, result.Id);

        // Assert
        canView.Should().BeFalse();
    }

    [Fact]
    public async Task CanStudentViewResultAsync_WrongStudent_ReturnsFalse()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var exam = context.Exams.First();
        exam.ResultsReleased = true;
        context.SaveChanges();

        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act - Try to access with different student
        var canView = await service.CanStudentViewResultAsync(999, result.Id);

        // Assert
        canView.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllResultsAsync_ReturnsAllResults()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var exam = context.Exams.First();
        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act
        var allResults = await service.GetAllResultsAsync();

        // Assert
        allResults.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetResultsByExamAsync_WhenReleased_ReturnsResults()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var exam = context.Exams.First();
        exam.ResultsReleased = true;
        context.SaveChanges();

        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act
        var results = await service.GetResultsByExamAsync(exam.Id, visibleOnly: true);

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetResultsByExamAsync_WhenNotReleased_ReturnsEmpty()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<ExamResultService>>();
        var service = new ExamResultService(context, loggerMock.Object);

        var exam = context.Exams.First();
        var result = new ExamResult
        {
            StudentProfileId = 1,
            ExamId = exam.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Act
        var results = await service.GetResultsByExamAsync(exam.Id, visibleOnly: true);

        // Assert
        results.Should().BeEmpty();
    }
}
