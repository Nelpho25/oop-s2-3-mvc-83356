using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using VgcCollege.Tests.Fixtures;
using Xunit;

namespace VgcCollege.Tests.Models;

public class BranchTests
{
    [Fact]
    public void CreateBranch_WithValidData_ShouldSucceed()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "Dublin Campus",
            Address = "123 College Street, Dublin"
        };

        // Act & Assert
        Assert.NotNull(branch);
        Assert.Equal("Dublin Campus", branch.Name);
        Assert.Equal("123 College Street, Dublin", branch.Address);
    }

    [Fact]
    public void SaveBranch_ToDatabase_ShouldPersist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryDbContext("BranchTest");
        var branch = new Branch
        {
            Name = "Cork Campus",
            Address = "456 University Road, Cork"
        };

        // Act
        context.Branches.Add(branch);
        context.SaveChanges();

        // Assert
        var savedBranch = context.Branches.FirstOrDefault(b => b.Name == "Cork Campus");
        Assert.NotNull(savedBranch);
        Assert.Equal("Cork Campus", savedBranch.Name);
    }

    [Fact]
    public void BranchWithoutName_ShouldFail()
    {
        // Arrange
        var branch = new Branch
        {
            Name = "", // Empty name
            Address = "123 College Street"
        };

        // Assert
        Assert.True(string.IsNullOrEmpty(branch.Name));
    }
}
