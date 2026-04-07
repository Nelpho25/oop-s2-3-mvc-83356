using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.Services;
using VgcCollege.Tests.Fixtures;
using VgcCollege.Tests.Helpers;
using Xunit;

namespace VgcCollege.Tests.Services;

public class ExtendedExamResultServiceTests
{
    private readonly ILogger<ExamResultService> _loggerMock = new Mock<ILogger<ExamResultService>>().Object;

    [Fact]
    public async Task GetVisibleResultsForStudentAsync_WhenMultipleExams_ReturnsOnlyVisible()
    {
        // Arrange
        var dbName = "ExamResult_MultipleVisible_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var exam1 = await TestDbHelper.SeedExamAsync(context, course.Id, released: true);
        var exam2 = await TestDbHelper.SeedExamAsync(context, course.Id, released: false);
        
        await TestDbHelper.SeedExamResultAsync(context, exam1.Id, student.Id, 85);
        await TestDbHelper.SeedExamResultAsync(context, exam2.Id, student.Id, 75);

        var service = new ExamResultService(context, _loggerMock);

        // Act
        var visibleResults = await service.GetVisibleResultsForStudentAsync(student.Id);

        // Assert
        visibleResults.Should().HaveCount(1);
        visibleResults[0].ExamId.Should().Be(exam1.Id);
    }

    [Fact]
    public async Task GetAllResultsAsync_ReturnsAllResultsRegardlessOfReleaseStatus()
    {
        // Arrange
        var dbName = "ExamResult_AllResults_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var exam1 = await TestDbHelper.SeedExamAsync(context, course.Id, released: true);
        var exam2 = await TestDbHelper.SeedExamAsync(context, course.Id, released: false);
        
        await TestDbHelper.SeedExamResultAsync(context, exam1.Id, student.Id, 85);
        await TestDbHelper.SeedExamResultAsync(context, exam2.Id, student.Id, 75);

        var service = new ExamResultService(context, _loggerMock);

        // Act
        var allResults = await service.GetAllResultsAsync();

        // Assert
        allResults.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetResultsByExamAsync_WithVisibleOnlyTrue_ReturnsOnlyReleasedResults()
    {
        // Arrange
        var dbName = "ExamResult_VisibleOnly_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var students = await TestDbHelper.SeedMultipleStudentsAsync(context, 2);
        var exam = await TestDbHelper.SeedExamAsync(context, course.Id, released: true);

        await TestDbHelper.SeedExamResultAsync(context, exam.Id, student.Id, 85);
        await TestDbHelper.SeedExamResultAsync(context, exam.Id, students[0].Id, 90);
        await TestDbHelper.SeedExamResultAsync(context, exam.Id, students[1].Id, 78);

        var service = new ExamResultService(context, _loggerMock);

        // Act
        var results = await service.GetResultsByExamAsync(exam.Id, visibleOnly: true);

        // Assert
        results.Should().HaveCount(3);
        results.Should().AllSatisfy(r => r.ExamId.Should().Be(exam.Id));
    }

    [Fact]
    public async Task GetResultsByExamAsync_WithNonExistentExam_ReturnsEmpty()
    {
        // Arrange
        var dbName = "ExamResult_NonExistentExam_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var service = new ExamResultService(context, _loggerMock);

        // Act
        var results = await service.GetResultsByExamAsync(999, visibleOnly: true);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task CanStudentViewResult_WithCorrectStudent_ReturnsTrue()
    {
        // Arrange
        var dbName = "ExamResult_CorrectStudent_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var exam = await TestDbHelper.SeedExamAsync(context, course.Id, released: true);
        var result = await TestDbHelper.SeedExamResultAsync(context, exam.Id, student.Id, 85);

        var service = new ExamResultService(context, _loggerMock);

        // Act
        var canView = await service.CanStudentViewResultAsync(student.Id, result.Id);

        // Assert
        canView.Should().BeTrue();
    }

    [Fact]
    public async Task CanStudentViewResult_WithNonExistentResult_ReturnsFalse()
    {
        // Arrange
        var dbName = "ExamResult_NonExistentResult_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var service = new ExamResultService(context, _loggerMock);

        // Act
        var canView = await service.CanStudentViewResultAsync(student.Id, 999);

        // Assert
        canView.Should().BeFalse();
    }

    [Fact]
    public async Task GetVisibleResultsForStudentAsync_WithNoResults_ReturnsEmpty()
    {
        // Arrange
        var dbName = "ExamResult_NoResults_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var service = new ExamResultService(context, _loggerMock);

        // Act
        var results = await service.GetVisibleResultsForStudentAsync(student.Id);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVisibleResultsForStudentAsync_WithNonExistentStudent_ReturnsEmpty()
    {
        // Arrange
        var dbName = "ExamResult_NonExistentStudent_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);

        var service = new ExamResultService(context, _loggerMock);

        // Act
        var results = await service.GetVisibleResultsForStudentAsync(999);

        // Assert
        results.Should().BeEmpty();
    }
}
