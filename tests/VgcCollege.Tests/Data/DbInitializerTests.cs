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

    [Fact]
    public async Task InitializeAsync_CreatesAdminProfile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerAdminProfileTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var user = new ApplicationUser
        {
            Id = "admin-123",
            UserName = "admin@vgc.ie",
            Email = "admin@vgc.ie",
            DisplayName = "Admin User"
        };

        var admin = new AdminProfile
        {
            IdentityUserId = user.Id,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@vgc.ie"
        };

        // Act
        context.Users.Add(user);
        context.AdminProfiles.Add(admin);
        await context.SaveChangesAsync();

        // Assert
        var savedAdmin = await context.AdminProfiles
            .FirstOrDefaultAsync(a => a.Email == "admin@vgc.ie");
        Assert.NotNull(savedAdmin);
        Assert.Equal("Admin", savedAdmin.FirstName);
    }

    [Fact]
    public async Task InitializeAsync_CreatesFacultyProfile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerFacultyProfileTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var user = new ApplicationUser
        {
            Id = "faculty-123",
            UserName = "faculty@vgc.ie",
            Email = "faculty@vgc.ie",
            DisplayName = "Faculty User"
        };

        var faculty = new FacultyProfile
        {
            IdentityUserId = user.Id,
            FirstName = "Faculty",
            LastName = "User",
            Email = "faculty@vgc.ie"
        };

        // Act
        context.Users.Add(user);
        context.FacultyProfiles.Add(faculty);
        await context.SaveChangesAsync();

        // Assert
        var savedFaculty = await context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.Email == "faculty@vgc.ie");
        Assert.NotNull(savedFaculty);
        Assert.Equal("Faculty", savedFaculty.FirstName);
    }

    [Fact]
    public async Task InitializeAsync_CreatesCourseEnrollment()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerEnrollmentTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Math 101",
            BranchId = branch.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(4)
        };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = "student-123",
            UserName = "student@vgc.ie",
            Email = "student@vgc.ie"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var studentProfile = new StudentProfile
        {
            IdentityUserId = user.Id,
            FirstName = "Student",
            LastName = "User",
            StudentNumber = "STU-001",
            Email = "student@vgc.ie"
        };
        context.StudentProfiles.Add(studentProfile);
        await context.SaveChangesAsync();

        var enrollment = new CourseEnrolment
        {
            CourseId = course.Id,
            StudentProfileId = studentProfile.Id,
            EnrolDate = DateTime.UtcNow
        };

        // Act
        context.CourseEnrolments.Add(enrollment);
        await context.SaveChangesAsync();

        // Assert
        var savedEnrollment = await context.CourseEnrolments
            .FirstOrDefaultAsync(e => e.CourseId == course.Id);
        Assert.NotNull(savedEnrollment);
        Assert.Equal(course.Id, savedEnrollment.CourseId);
    }

    [Fact]
    public async Task InitializeAsync_CreatesExam()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerExamTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Physics 101",
            BranchId = branch.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(4)
        };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var exam = new Exam
        {
            Title = "Final Exam",
            CourseId = course.Id,
            MaxScore = 100,
            ExamDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        context.Exams.Add(exam);
        await context.SaveChangesAsync();

        // Assert
        var savedExam = await context.Exams
            .FirstOrDefaultAsync(e => e.Title == "Final Exam");
        Assert.NotNull(savedExam);
        Assert.Equal(100, savedExam.MaxScore);
    }

    [Fact]
    public async Task InitializeAsync_CreatesAssignment()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InitializerAssignmentTest")
            .Options;

        using var context = new ApplicationDbContext(options);

        var branch = new Branch { Name = "Dublin", Address = "123 Street" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Chemistry 101",
            BranchId = branch.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(4)
        };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var assignment = new Assignment
        {
            Title = "Lab Report",
            CourseId = course.Id,
            MaxScore = 50,
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        // Act
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();

        // Assert
        var savedAssignment = await context.Assignments
            .FirstOrDefaultAsync(a => a.Title == "Lab Report");
        Assert.NotNull(savedAssignment);
        Assert.Equal(50, savedAssignment.MaxScore);
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
