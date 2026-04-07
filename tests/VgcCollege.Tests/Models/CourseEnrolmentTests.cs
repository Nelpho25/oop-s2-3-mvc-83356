using oop_s2_1_mvc_83356.Models;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Models;

public class CourseEnrolmentTests
{
    [Fact]
    public void CreateEnrolment_WithValidData_ShouldSucceed()
    {
        // Arrange
        var enrolment = new CourseEnrolment
        {
            StudentProfileId = 1,
            CourseId = 1,
            EnrolDate = DateTime.UtcNow,
            Status = CourseEnrolmentStatus.Active
        };

        // Act & Assert
        Assert.NotNull(enrolment);
        Assert.Equal(CourseEnrolmentStatus.Active, enrolment.Status);
    }

    [Fact]
    public void SaveEnrolment_ToDatabase_ShouldPersist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("EnrolmentTest");

        // Create dependent entities
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

        var enrolment = new CourseEnrolment
        {
            StudentProfileId = student.Id,
            CourseId = course.Id,
            EnrolDate = DateTime.UtcNow,
            Status = CourseEnrolmentStatus.Active
        };

        // Act
        context.CourseEnrolments.Add(enrolment);
        context.SaveChanges();

        // Assert
        var savedEnrolment = context.CourseEnrolments
            .FirstOrDefault(e => e.StudentProfileId == student.Id && e.CourseId == course.Id);
        Assert.NotNull(savedEnrolment);
        Assert.Equal(CourseEnrolmentStatus.Active, savedEnrolment.Status);
    }

    [Fact]
    public void EnrolmentStatus_CanBeChanged()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("EnrolmentStatusTest");

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

        var enrolment = new CourseEnrolment
        {
            StudentProfileId = student.Id,
            CourseId = course.Id,
            Status = CourseEnrolmentStatus.Active
        };

        context.CourseEnrolments.Add(enrolment);
        context.SaveChanges();

        // Act
        enrolment.Status = CourseEnrolmentStatus.Withdrawn;
        context.SaveChanges();

        // Assert
        var updatedEnrolment = context.CourseEnrolments.First(e => e.Id == enrolment.Id);
        Assert.Equal(CourseEnrolmentStatus.Withdrawn, updatedEnrolment.Status);
    }
}
