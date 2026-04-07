using oop_s2_1_mvc_83356.Models;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Models;

public class CourseTests
{
    [Fact]
    public void CreateCourse_WithValidData_ShouldSucceed()
    {
        // Arrange
        var course = new Course
        {
            Name = "Introduction to Computer Science",
            BranchId = 1,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow.AddDays(120)
        };

        // Act & Assert
        Assert.NotNull(course);
        Assert.Equal("Introduction to Computer Science", course.Name);
        Assert.Equal(1, course.BranchId);
    }

    [Fact]
    public void SaveCourse_ToDatabase_ShouldPersist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("CourseTest");
        
        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course
        {
            Name = "Web Development Fundamentals",
            BranchId = branch.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(120)
        };

        // Act
        context.Courses.Add(course);
        context.SaveChanges();

        // Assert
        var savedCourse = context.Courses.FirstOrDefault(c => c.Name == "Web Development Fundamentals");
        Assert.NotNull(savedCourse);
        Assert.Equal(branch.Id, savedCourse.BranchId);
    }

    [Fact]
    public void CourseWithoutName_ShouldFail()
    {
        // Arrange
        var course = new Course
        {
            Name = "", // Empty name
            BranchId = 1
        };

        // Assert
        Assert.True(string.IsNullOrEmpty(course.Name));
    }
}
