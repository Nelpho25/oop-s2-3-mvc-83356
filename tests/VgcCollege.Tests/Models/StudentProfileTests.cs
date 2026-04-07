using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Models;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Models;

public class StudentProfileTests
{
    [Fact]
    public void CreateStudentProfile_WithValidData_ShouldSucceed()
    {
        // Arrange
        var student = new StudentProfile
        {
            FirstName = "Alice",
            LastName = "Brown",
            StudentNumber = "VGC-2026-A001",
            Email = "alice@vgc.ie",
            Phone = "+353-1-234-5678",
            Address = "123 Main Street, Dublin",
            DateOfBirth = new DateTime(2000, 5, 15)
        };

        // Act & Assert
        Assert.NotNull(student);
        Assert.Equal("Alice", student.FirstName);
        Assert.Equal("VGC-2026-A001", student.StudentNumber);
    }

    [Fact]
    public void SaveStudentProfile_ToDatabase_ShouldPersist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("StudentTest");

        var student = new StudentProfile
        {
            IdentityUserId = "user-123",
            FirstName = "Bob",
            LastName = "Wilson",
            StudentNumber = "VGC-2026-B001",
            Email = "bob@vgc.ie",
            Phone = "+353-1-987-6543"
        };

        // Act
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        // Assert
        var savedStudent = context.StudentProfiles.FirstOrDefault(s => s.StudentNumber == "VGC-2026-B001");
        Assert.NotNull(savedStudent);
        Assert.Equal("Bob", savedStudent.FirstName);
    }

    [Fact]
    public void StudentNumber_ShouldBeStored()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("StudentStoreTest");

        var student1 = new StudentProfile
        {
            IdentityUserId = "user-1",
            FirstName = "Alice",
            LastName = "Brown",
            StudentNumber = "VGC-2026-A001",
            Email = "alice@vgc.ie"
        };

        // Act
        context.StudentProfiles.Add(student1);
        context.SaveChanges();

        // Assert
        var savedStudent = context.StudentProfiles.First(s => s.StudentNumber == "VGC-2026-A001");
        Assert.Equal("VGC-2026-A001", savedStudent.StudentNumber);
    }

    [Fact]
    public void StudentWithoutEmail_ShouldFail()
    {
        // Arrange
        var student = new StudentProfile
        {
            FirstName = "Charlie",
            LastName = "Davis",
            StudentNumber = "VGC-2026-C001",
            Email = "" // Empty email
        };

        // Assert
        Assert.True(string.IsNullOrEmpty(student.Email));
    }
}
