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
    private ApplicationDbContext CreateContextWithBasicData(string dbName)
    {
        var context = TestDbContextFactory.CreateInMemoryDbContext(dbName);

        var branch = new Branch { Name = "Dublin", Address = "Dublin, Ireland" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course
        {
            Name = "C# Advanced",
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

        return context;
    }

    [Fact]
    public async Task EnrollStudentAsync_WithValidData_CreatesEnrolment()
    {
        // Arrange
        var context = CreateContextWithBasicData("Enrol_Create_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var course = context.Courses.First();

        // Act
        var result = await service.EnrollStudentAsync(student.Id, course.Id);

        // Assert
        result.Should().NotBeNull();
        result!.StudentProfileId.Should().Be(student.Id);
        result.CourseId.Should().Be(course.Id);
        result.Status.Should().Be(CourseEnrolmentStatus.Active);
    }

    [Fact]
    public async Task EnrollStudentAsync_WhenAlreadyEnrolled_ReturnsNull()
    {
        // Arrange
        var context = CreateContextWithBasicData("Enrol_Duplicate_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var course = context.Courses.First();

        // First enrollment
        await service.EnrollStudentAsync(student.Id, course.Id);

        // Act - Try to enroll again
        var result = await service.EnrollStudentAsync(student.Id, course.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EnrollStudentAsync_WithInvalidStudent_ReturnsNull()
    {
        // Arrange
        var context = CreateContextWithBasicData("Enrol_InvalidStudent_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var course = context.Courses.First();

        // Act
        var result = await service.EnrollStudentAsync(999, course.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EnrollStudentAsync_WithInvalidCourse_ReturnsNull()
    {
        // Arrange
        var context = CreateContextWithBasicData("Enrol_InvalidCourse_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();

        // Act
        var result = await service.EnrollStudentAsync(student.Id, 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task WithdrawStudentAsync_WithValidEnrolment_WithdrawsSuccessfully()
    {
        // Arrange
        var context = CreateContextWithBasicData("Withdraw_Valid_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var course = context.Courses.First();

        var enrolment = await service.EnrollStudentAsync(student.Id, course.Id);
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
        var context = CreateContextWithBasicData("IsEnrolled_True_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var course = context.Courses.First();

        await service.EnrollStudentAsync(student.Id, course.Id);

        // Act
        var isEnrolled = await service.IsStudentEnrolledAsync(student.Id, course.Id);

        // Assert
        isEnrolled.Should().BeTrue();
    }

    [Fact]
    public async Task IsStudentEnrolledAsync_WhenNotEnrolled_ReturnsFalse()
    {
        // Arrange
        var context = CreateContextWithBasicData("IsEnrolled_False_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var course = context.Courses.First();

        // Act
        var isEnrolled = await service.IsStudentEnrolledAsync(student.Id, course.Id);

        // Assert
        isEnrolled.Should().BeFalse();
    }

    [Fact]
    public async Task GetEnrolmentsByStudentAsync_ReturnsAllStudentEnrolments()
    {
        // Arrange
        var context = CreateContextWithBasicData("GetByStudent_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student = context.StudentProfiles.First();
        var course1 = context.Courses.First();

        // Add second course
        var branch = context.Branches.First();
        var course2 = new Course
        {
            Name = "Data Structures",
            BranchId = branch.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(4)
        };
        context.Courses.Add(course2);
        context.SaveChanges();

        // Enroll student in both courses
        await service.EnrollStudentAsync(student.Id, course1.Id);
        await service.EnrollStudentAsync(student.Id, course2.Id);

        // Act
        var enrolments = await service.GetEnrolmentsByStudentAsync(student.Id);

        // Assert
        enrolments.Should().HaveCount(2);
        enrolments.Should().AllSatisfy(e => e.StudentProfileId.Should().Be(student.Id));
    }

    [Fact]
    public async Task GetEnrolmentsByCourseAsync_ReturnsAllCourseEnrolments()
    {
        // Arrange
        var context = CreateContextWithBasicData("GetByCourse_" + Guid.NewGuid());
        var loggerMock = new Mock<ILogger<EnrolmentService>>();
        var service = new EnrolmentService(context, loggerMock.Object);

        var student1 = context.StudentProfiles.First();
        var course = context.Courses.First();

        // Add second student
        var student2 = new StudentProfile
        {
            StudentNumber = "VGC-2024-002",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@university.edu",
            IdentityUserId = "user-2"
        };
        context.StudentProfiles.Add(student2);
        context.SaveChanges();

        // Enroll both students in same course
        await service.EnrollStudentAsync(student1.Id, course.Id);
        await service.EnrollStudentAsync(student2.Id, course.Id);

        // Act
        var enrolments = await service.GetEnrolmentsByCourseAsync(course.Id);

        // Assert
        enrolments.Should().HaveCount(2);
        enrolments.Should().AllSatisfy(e => e.CourseId.Should().Be(course.Id));
    }
}
