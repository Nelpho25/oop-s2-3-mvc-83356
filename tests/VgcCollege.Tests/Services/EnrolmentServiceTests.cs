using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using oop_s2_1_mvc_83356.Services;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Services;

public class EnrolmentServiceTests
{
    [Fact]
    public async Task EnrollStudentAsync_WithValidData_CreatesEnrolment()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        // Act
        var result = await service.EnrollStudentAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result!.StudentProfileId.Should().Be(1);
        result.CourseId.Should().Be(1);
        result.Status.Should().Be(CourseEnrolmentStatus.Active);
    }

    [Fact]
    public async Task EnrollStudentAsync_WhenAlreadyEnrolled_ReturnsNull()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        // First enrollment
        await service.EnrollStudentAsync(1, 1);

        // Act - Try to enroll again
        var result = await service.EnrollStudentAsync(1, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EnrollStudentAsync_WithInvalidStudent_ReturnsNull()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        // Act
        var result = await service.EnrollStudentAsync(999, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EnrollStudentAsync_WithInvalidCourse_ReturnsNull()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        // Act
        var result = await service.EnrollStudentAsync(1, 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task WithdrawStudentAsync_WithValidEnrolment_WithdrawsSuccessfully()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var enrolment = await service.EnrollStudentAsync(1, 1);
        var enrolmentId = enrolment!.Id;

        // Act
        await service.WithdrawStudentAsync(enrolmentId);

        // Assert
        var retrieved = await context.CourseEnrolments.FindAsync(enrolmentId);
        retrieved!.Status.Should().Be(CourseEnrolmentStatus.Withdrawn);
    }

    [Fact]
    public async Task IsStudentEnrolledAsync_WhenEnrolled_ReturnsTrue()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        await service.EnrollStudentAsync(1, 1);

        // Act
        var isEnrolled = await service.IsStudentEnrolledAsync(1, 1);

        // Assert
        isEnrolled.Should().BeTrue();
    }

    [Fact]
    public async Task IsStudentEnrolledAsync_WhenNotEnrolled_ReturnsFalse()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        // Act
        var isEnrolled = await service.IsStudentEnrolledAsync(1, 1);

        // Assert
        isEnrolled.Should().BeFalse();
    }

    [Fact]
    public async Task GetEnrolmentsByStudentAsync_ReturnsAllStudentEnrolments()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        // Add second course
        var course2 = new Course
        {
            Name = "Data Structures",
            BranchId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(4)
        };
        context.Courses.Add(course2);
        context.SaveChanges();

        // Enroll student in both courses
        await service.EnrollStudentAsync(1, 1);
        await service.EnrollStudentAsync(1, course2.Id);

        // Act
        var enrolments = await service.GetEnrolmentsByStudentAsync(1);

        // Assert
        enrolments.Should().HaveCount(2);
        enrolments.Should().AllSatisfy(e => e.StudentProfileId.Should().Be(1));
    }

    [Fact]
    public async Task GetEnrolmentsByCourseAsync_ReturnsAllCourseEnrolments()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryDbContext();
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        // Add second student
        var student2 = new StudentProfile
        {
            StudentNumber = "STU002",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@university.edu"
        };
        context.StudentProfiles.Add(student2);
        context.SaveChanges();

        // Enroll both students in same course
        await service.EnrollStudentAsync(1, 1);
        await service.EnrollStudentAsync(student2.Id, 1);

        // Act
        var enrolments = await service.GetEnrolmentsByCourseAsync(1);

        // Assert
        enrolments.Should().HaveCount(2);
        enrolments.Should().AllSatisfy(e => e.CourseId.Should().Be(1));
    }
}
