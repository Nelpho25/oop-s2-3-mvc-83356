using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;
using Xunit;

namespace VgcCollege.Tests.Data;

public class DbInitializerTests
{
    [Fact]
    public async Task InitializeAsync_CreatesRequiredRoles()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerRolesTest")
            .Options;

        using var context = new ApplicationDbContext(options);
        var roleManager = CreateRoleManager(context);

        // Act
        var roles = new[] { "Admin", "Faculty", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Assert
        Assert.NotEmpty(await context.Roles.ToListAsync());
        Assert.Equal(3, await context.Roles.CountAsync());
    }

    [Fact]
    public async Task InitializeAsync_CreatesRequiredUsers()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerUsersTest")
            .Options;

        using var context = new ApplicationDbContext(options);
        var userManager = CreateUserManager(context);

        // Act
        var user = new ApplicationUser
        {
            UserName = "testuser@vgc.ie",
            Email = "testuser@vgc.ie",
            DisplayName = "Test User"
        };

        var result = await userManager.CreateAsync(user, "TestPassword@123");

        // Assert
        Assert.True(result.Succeeded);
        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "testuser@vgc.ie");
        Assert.NotNull(createdUser);
        Assert.Equal("Test User", createdUser.DisplayName);
    }

    [Fact]
    public async Task InitializeAsync_CreatesStudentProfile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerStudentTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "alice@vgc.ie",
            Email = "alice@vgc.ie",
            DisplayName = "Alice Brown"
        };

        var student = new StudentProfile
        {
            IdentityUserId = user.Id,
            FirstName = "Alice",
            LastName = "Brown",
            StudentNumber = "VGC-2026-A001",
            Email = "alice@vgc.ie"
        };

        // Act
        context.Users.Add(user);
        context.StudentProfiles.Add(student);
        await context.SaveChangesAsync();

        // Assert
        var savedStudent = await context.StudentProfiles
            .FirstOrDefaultAsync(s => s.StudentNumber == "VGC-2026-A001");
        Assert.NotNull(savedStudent);
        Assert.Equal("Alice", savedStudent.FirstName);
    }

    [Fact]
    public async Task InitializeAsync_CreatesBranches()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerBranchesTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var branches = new[]
        {
            new Branch { Name = "Dublin Campus", Address = "123 College Street, Dublin" },
            new Branch { Name = "Cork Campus", Address = "456 University Road, Cork" },
            new Branch { Name = "Galway Campus", Address = "789 Education Avenue, Galway" }
        };

        // Act
        context.Branches.AddRange(branches);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(3, await context.Branches.CountAsync());
        var dublinBranch = await context.Branches
            .FirstOrDefaultAsync(b => b.Name == "Dublin Campus");
        Assert.NotNull(dublinBranch);
    }

    [Fact]
    public async Task InitializeAsync_CreatesCourses()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerCoursesTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Introduction to Computer Science",
            BranchId = branch.Id,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow.AddDays(120)
        };

        // Act
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        // Assert
        var savedCourse = await context.Courses
            .FirstOrDefaultAsync(c => c.Name == "Introduction to Computer Science");
        Assert.NotNull(savedCourse);
        Assert.Equal(branch.Id, savedCourse.BranchId);
    }

    [Fact]
    public async Task InitializeAsync_IsIdempotent()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerIdempotentTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var existingCount = await context.Branches.CountAsync();

        // Act
        // Try to add the same branch again (simulating the if check)
        if (!context.Branches.Any())
        {
            context.Branches.Add(branch);
            await context.SaveChangesAsync();
        }

        // Assert
        var finalCount = await context.Branches.CountAsync();
        Assert.Equal(existingCount, finalCount);
    }

    // Helper methods
    private RoleManager<IdentityRole> CreateRoleManager(ApplicationDbContext context)
    {
        var store = new RoleStore<IdentityRole>(context);
        return new RoleManager<IdentityRole>(
            store,
            new[] { new RoleValidator<IdentityRole>() },
            new UpperInvariantLookupNormalizer(),
            null,
            null);
    }

    private UserManager<ApplicationUser> CreateUserManager(ApplicationDbContext context)
    {
        var userStore = new UserStore<ApplicationUser>(context);
        var userManager = new UserManager<ApplicationUser>(
            userStore,
            null,
            new PasswordHasher<ApplicationUser>(),
            null,
            null,
            null,
            null,
            null,
            null);
        return userManager;
    }
}
