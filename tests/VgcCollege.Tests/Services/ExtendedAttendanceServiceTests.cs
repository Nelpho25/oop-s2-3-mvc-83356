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

public class ExtendedAttendanceServiceTests
{
    private readonly ILogger<AttendanceService> _loggerMock = new Mock<ILogger<AttendanceService>>().Object;

    [Fact]
    public async Task RecordAttendanceAsync_WithInvalidSessionDate_DoesNotRecord()
    {
        // Arrange
        var dbName = "Attend_InvalidDate_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var enrolment = await TestDbHelper.SeedEnrolmentAsync(context, student.Id, course.Id);

        var service = new AttendanceService(context, _loggerMock);
        var invalidDate = course.StartDate!.Value.AddDays(-30); // Before course starts

        // Act
        await service.RecordAttendanceAsync(enrolment.Id, invalidDate, true);

        // Assert
        var records = await service.GetAttendanceRecordsAsync(enrolment.Id);
        records.Should().BeEmpty();
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_With100PercentAttendance_Returns100()
    {
        // Arrange
        var dbName = "Attend_100Percent_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var enrolment = await TestDbHelper.SeedEnrolmentAsync(context, student.Id, course.Id);

        await TestDbHelper.SeedAttendanceRecordsAsync(context, enrolment.Id, count: 10, presentCount: 10);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var rate = await service.CalculateAttendanceRateAsync(enrolment.Id);

        // Assert
        rate.Should().Be(100);
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_With0PercentAttendance_Returns0()
    {
        // Arrange
        var dbName = "Attend_0Percent_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var enrolment = await TestDbHelper.SeedEnrolmentAsync(context, student.Id, course.Id);

        await TestDbHelper.SeedAttendanceRecordsAsync(context, enrolment.Id, count: 10, presentCount: 0);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var rate = await service.CalculateAttendanceRateAsync(enrolment.Id);

        // Assert
        rate.Should().Be(0);
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_WithMixedAttendance_CalculatesCorrectly()
    {
        // Arrange
        var dbName = "Attend_Mixed_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var enrolment = await TestDbHelper.SeedEnrolmentAsync(context, student.Id, course.Id);

        // Create 10 records, 7 present = 70%
        await TestDbHelper.SeedAttendanceRecordsAsync(context, enrolment.Id, count: 10, presentCount: 7);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var rate = await service.CalculateAttendanceRateAsync(enrolment.Id);

        // Assert
        rate.Should().Be(70);
    }

    [Fact]
    public async Task GetAttendanceRecordsAsync_ReturnsSortedByDate()
    {
        // Arrange
        var dbName = "Attend_SortedDate_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var enrolment = await TestDbHelper.SeedEnrolmentAsync(context, student.Id, course.Id);

        await TestDbHelper.SeedAttendanceRecordsAsync(context, enrolment.Id, count: 5, presentCount: 3);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var records = await service.GetAttendanceRecordsAsync(enrolment.Id);

        // Assert
        records.Should().BeInAscendingOrder(r => r.SessionDate);
    }

    [Fact]
    public async Task IsValidSessionDateAsync_AtCourseStart_ReturnsTrue()
    {
        // Arrange
        var dbName = "Attend_AtStart_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var service = new AttendanceService(context, _loggerMock);
        var sessionDate = course.StartDate!.Value;

        // Act
        var isValid = await service.IsValidSessionDateAsync(course.Id, sessionDate);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidSessionDateAsync_AtCourseEnd_ReturnsTrue()
    {
        // Arrange
        var dbName = "Attend_AtEnd_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var service = new AttendanceService(context, _loggerMock);
        var sessionDate = course.EndDate!.Value;

        // Act
        var isValid = await service.IsValidSessionDateAsync(course.Id, sessionDate);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidSessionDateAsync_WithNonExistentCourse_ReturnsFalse()
    {
        // Arrange
        var dbName = "Attend_NonExistentCourse_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var isValid = await service.IsValidSessionDateAsync(999, DateTime.Now);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task RecordAttendanceAsync_WithNonExistentEnrolment_DoesNotThrow()
    {
        // Arrange
        var dbName = "Attend_NonExistentEnrol_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var action = async () => await service.RecordAttendanceAsync(999, DateTime.Now, true);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_WithNonExistentEnrolment_ReturnsZero()
    {
        // Arrange
        var dbName = "Attend_NonExistentEnrolRate_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var rate = await service.CalculateAttendanceRateAsync(999);

        // Assert
        rate.Should().Be(0);
    }

    [Fact]
    public async Task GetAttendanceRecordsAsync_WithNoRecords_ReturnsEmpty()
    {
        // Arrange
        var dbName = "Attend_NoRecords_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var enrolment = await TestDbHelper.SeedEnrolmentAsync(context, student.Id, course.Id);

        var service = new AttendanceService(context, _loggerMock);

        // Act
        var records = await service.GetAttendanceRecordsAsync(enrolment.Id);

        // Assert
        records.Should().BeEmpty();
    }

    [Fact]
    public async Task RecordAttendanceAsync_Multiple_AllRecorded()
    {
        // Arrange
        var dbName = "Attend_Multiple_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);
        var enrolment = await TestDbHelper.SeedEnrolmentAsync(context, student.Id, course.Id);

        var service = new AttendanceService(context, _loggerMock);
        var startDate = DateTime.Now.AddDays(-5);

        // Act
        for (int i = 0; i < 5; i++)
        {
            await service.RecordAttendanceAsync(enrolment.Id, startDate.AddDays(i), i % 2 == 0);
        }

        // Assert
        var records = await service.GetAttendanceRecordsAsync(enrolment.Id);
        records.Should().HaveCount(5);
    }

    [Fact]
    public async Task IsValidSessionDateAsync_InMiddleOfCourse_ReturnsTrue()
    {
        // Arrange
        var dbName = "Attend_InMiddle_" + Guid.NewGuid();
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);
        var (branch, course, student) = await TestDbHelper.SeedBasicDataAsync(context);

        var service = new AttendanceService(context, _loggerMock);
        var duration = course.EndDate!.Value - course.StartDate!.Value;
        var midDate = course.StartDate.Value.Add(duration / 2);

        // Act
        var isValid = await service.IsValidSessionDateAsync(course.Id, midDate);

        // Assert
        isValid.Should().BeTrue();
    }
}
