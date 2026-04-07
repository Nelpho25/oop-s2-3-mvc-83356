using FluentAssertions;
using oop_s2_1_mvc_83356.Models;
using Xunit;

namespace VgcCollege.Tests.Models;

public class AdminProfileTests
{
    [Fact]
    public void AdminProfile_CreateWithValidData_HasCorrectProperties()
    {
        // Arrange & Act
        var admin = new AdminProfile
        {
            Id = 1,
            IdentityUserId = "user-123",
            FirstName = "John",
            LastName = "Admin",
            Email = "admin@vgc.ie",
            Phone = "1234567890",
            Department = "Administration"
        };

        // Assert
        admin.Id.Should().Be(1);
        admin.IdentityUserId.Should().Be("user-123");
        admin.FirstName.Should().Be("John");
        admin.LastName.Should().Be("Admin");
        admin.Email.Should().Be("admin@vgc.ie");
        admin.Phone.Should().Be("1234567890");
        admin.Department.Should().Be("Administration");
    }

    [Fact]
    public void AdminProfile_WithoutOptionalFields_IsValid()
    {
        // Arrange & Act
        var admin = new AdminProfile
        {
            Id = 1,
            IdentityUserId = "user-123",
            FirstName = "John",
            LastName = "Admin",
            Email = "admin@vgc.ie"
        };

        // Assert
        admin.FirstName.Should().NotBeNullOrEmpty();
        admin.LastName.Should().NotBeNullOrEmpty();
        admin.Email.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void AdminProfile_WithApplicationUserNavigation_CanSetReference()
    {
        // Arrange
        var admin = new AdminProfile
        {
            Id = 1,
            IdentityUserId = "user-123",
            FirstName = "Jane",
            LastName = "Admin",
            Email = "jane@vgc.ie"
        };

        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "jane@vgc.ie",
            Email = "jane@vgc.ie"
        };

        // Act
        admin.ApplicationUser = user;

        // Assert
        admin.ApplicationUser.Should().NotBeNull();
        admin.ApplicationUser.Id.Should().Be("user-123");
    }
}
