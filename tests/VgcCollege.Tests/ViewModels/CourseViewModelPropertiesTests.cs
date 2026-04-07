using FluentAssertions;
using oop_s2_1_mvc_83356.ViewModels;
using Xunit;

namespace VgcCollege.Tests.ViewModels;

public class CourseViewModelPropertiesTests
{
    [Fact]
    public void CourseIndexViewModel_HasIdProperty()
    {
        // Arrange & Act
        var vm = new CourseIndexViewModel { Id = 1, Name = "Math 101" };

        // Assert
        vm.Id.Should().Be(1);
        vm.Name.Should().Be("Math 101");
    }

    [Fact]
    public void CourseDetailsViewModel_HasCorrectProperties()
    {
        // Arrange & Act
        var vm = new CourseDetailsViewModel
        {
            Id = 1,
            Name = "Physics 101",
            BranchId = 2,
            AssignedFaculty = new(),
            EnrolledStudents = new()
        };

        // Assert
        vm.Id.Should().Be(1);
        vm.Name.Should().Be("Physics 101");
        vm.BranchId.Should().Be(2);
        vm.AssignedFaculty.Should().NotBeNull();
        vm.EnrolledStudents.Should().NotBeNull();
    }

    [Fact]
    public void CourseCreateEditViewModel_HasCorrectProperties()
    {
        // Arrange & Act
        var vm = new CourseCreateEditViewModel
        {
            Id = 1,
            Name = "Chemistry 101",
            BranchId = 1,
            StartDate = DateTime.Now
        };

        // Assert
        vm.Id.Should().Be(1);
        vm.Name.Should().Be("Chemistry 101");
        vm.BranchId.Should().Be(1);
        vm.StartDate.Should().NotBeNull();
    }
}
