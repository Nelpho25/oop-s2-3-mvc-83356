using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.Services;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Services;

public class AttendanceServiceTests
{
    [Fact]
    public async Task CalculateAttendanceRateAsync_WithRecords_CalculatesCorrectly()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        // Create enrollment first
        var enrolmentService = new EnrolmentService(context, new Mock<ILogger<EnrolmentService>>().Object);
        var enrolment = await enrolmentService.EnrollStudentAsync(1, 1);

        // Record attendance: 7 present, 3 absent = 70%
        var sessionDate = DateTime.Now.AddDays(-10);
        for (int i = 0; i < 7; i++)
        {
            await service.RecordAttendanceAsync(enrolment!.Id, sessionDate.AddDays(i), true);
        }
        for (int i = 0; i < 3; i++)
        {
            await service.RecordAttendanceAsync(enrolment!.Id, sessionDate.AddDays(7 + i), false);
        }

        // Act
        var rate = await service.CalculateAttendanceRateAsync(enrolment!.Id);

        // Assert
        rate.Should().Be(70);
    }

    [Fact]
    public async Task CalculateAttendanceRateAsync_WithNoRecords_ReturnsZero()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        // Act
        var rate = await service.CalculateAttendanceRateAsync(999); // Non-existent enrollment

        // Assert
        rate.Should().Be(0);
    }

    [Fact]
    public async Task RecordAttendanceAsync_WithValidData_RecordsSuccessfully()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        var enrolmentService = new EnrolmentService(context, new Mock<ILogger<EnrolmentService>>().Object);
        var enrolment = await enrolmentService.EnrollStudentAsync(1, 1);

        var sessionDate = DateTime.Now;

        // Act
        await service.RecordAttendanceAsync(enrolment!.Id, sessionDate, true);

        // Assert
        var records = await service.GetAttendanceRecordsAsync(enrolment.Id);
        records.Should().HaveCount(1);
        records[0].IsPresent.Should().BeTrue();
        records[0].SessionDate.Should().Be(sessionDate);
    }

    [Fact]
    public async Task IsValidSessionDateAsync_WithinCoursePeriod_ReturnsTrue()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        var course = context.Courses.First();
        var sessionDate = course.StartDate!.Value.AddDays(10); // Within period

        // Act
        var isValid = await service.IsValidSessionDateAsync(course.Id, sessionDate);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidSessionDateAsync_BeforeCourseStart_ReturnsFalse()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        var course = context.Courses.First();
        var sessionDate = course.StartDate!.Value.AddDays(-10); // Before start

        // Act
        var isValid = await service.IsValidSessionDateAsync(course.Id, sessionDate);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task IsValidSessionDateAsync_AfterCourseEnd_ReturnsFalse()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        var course = context.Courses.First();
        var sessionDate = course.EndDate!.Value.AddDays(10); // After end

        // Act
        var isValid = await service.IsValidSessionDateAsync(course.Id, sessionDate);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetAttendanceRecordsAsync_ReturnsAllRecords()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        var enrolmentService = new EnrolmentService(context, new Mock<ILogger<EnrolmentService>>().Object);
        var enrolment = await enrolmentService.EnrollStudentAsync(1, 1);

        // Record multiple attendance
        var sessionDate = DateTime.Now.AddDays(-5);
        for (int i = 0; i < 3; i++)
        {
            await service.RecordAttendanceAsync(enrolment!.Id, sessionDate.AddDays(i), true);
        }

        // Act
        var records = await service.GetAttendanceRecordsAsync(enrolment!.Id);

        // Assert
        records.Should().HaveCount(3);
        records.Should().BeInAscendingOrder(r => r.SessionDate);
    }

    [Theory]
    [InlineData(1, 100)]   // 1 out of 1 = 100%
    [InlineData(5, 100)]   // 5 out of 5 = 100%
    [InlineData(0, 100)]   // 0 out of 5 = 0%
    [InlineData(3, 60)]    // 3 out of 5 = 60%
    public async Task CalculateAttendanceRateAsync_WithTheory_CalculatesCorrectly(int presentDays, decimal expectedRate)
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<AttendanceService>>();
        var service = new AttendanceService(context, loggerMock.Object);

        var enrolmentService = new EnrolmentService(context, new Mock<ILogger<EnrolmentService>>().Object);
        var enrolment = await enrolmentService.EnrollStudentAsync(1, 1);

        var sessionDate = DateTime.Now.AddDays(-10);
        const int totalDays = 5;

        for (int i = 0; i < totalDays; i++)
        {
            bool isPresent = i < presentDays;
            await service.RecordAttendanceAsync(enrolment!.Id, sessionDate.AddDays(i), isPresent);
        }

        // Act
        var rate = await service.CalculateAttendanceRateAsync(enrolment!.Id);

        // Assert
        rate.Should().Be(expectedRate);
    }
}
