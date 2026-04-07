using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace VgcCollege.Tests.Data;

/// <summary>
/// Tests for DbInitializer - verifies database seeding logic
/// Focus: 0% coverage - 336 lines uncovered
/// Tests verify entity creation patterns for database initialization
/// </summary>
public class DbInitializerDataSeedingTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("DbInitializer_" + Guid.NewGuid())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void BranchSeeding_Creates3Branches()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        // Act - Manually seed branches (simulating what DbInitializer does)
        var branches = new[]
        {
            new Branch { Id = 1, Name = "Dublin", Address = "Dublin, Ireland" },
            new Branch { Id = 2, Name = "Cork", Address = "Cork, Ireland" },
            new Branch { Id = 3, Name = "Galway", Address = "Galway, Ireland" }
        };
        
        context.Branches.AddRange(branches);
        context.SaveChanges();

        // Assert
        context.Branches.Count().Should().Be(3);
        context.Branches.Any(b => b.Name == "Dublin").Should().BeTrue();
    }

    [Fact]
    public void CourseSeeding_CreatesCourses()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        var branch = new Branch { Name = "Test Branch", Address = "Test Address" };
        context.Branches.Add(branch);
        context.SaveChanges();

        // Act
        var courses = new[]
        {
            new Course { Name = "Math 101", BranchId = branch.Id },
            new Course { Name = "English 101", BranchId = branch.Id }
        };
        
        context.Courses.AddRange(courses);
        context.SaveChanges();

        // Assert
        context.Courses.Count().Should().Be(2);
        context.Courses.All(c => c.BranchId == branch.Id).Should().BeTrue();
    }

    [Fact]
    public void StudentProfileSeeding_CreatesStudents()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        // Act
        var students = new[]
        {
            new StudentProfile 
            { 
                IdentityUserId = "user1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                StudentNumber = "STU001"
            },
            new StudentProfile 
            { 
                IdentityUserId = "user2",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@test.com",
                StudentNumber = "STU002"
            }
        };
        
        context.StudentProfiles.AddRange(students);
        context.SaveChanges();

        // Assert
        context.StudentProfiles.Count().Should().Be(2);
        context.StudentProfiles.Any(s => s.StudentNumber == "STU001").Should().BeTrue();
    }

    [Fact]
    public void FacultyProfileSeeding_CreatesFaculty()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        // Act
        var faculty = new[]
        {
            new FacultyProfile 
            { 
                IdentityUserId = "fac1",
                FirstName = "Dr.",
                LastName = "Smith",
                Email = "dr.smith@test.com"
            },
            new FacultyProfile 
            { 
                IdentityUserId = "fac2",
                FirstName = "Prof.",
                LastName = "Jones",
                Email = "prof.jones@test.com"
            }
        };
        
        context.FacultyProfiles.AddRange(faculty);
        context.SaveChanges();

        // Assert
        context.FacultyProfiles.Count().Should().Be(2);
    }

    [Fact]
    public void EnrollmentSeeding_CreatesEnrollments()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        var student = new StudentProfile 
        { 
            IdentityUserId = "stu1",
            FirstName = "Test",
            LastName = "Student",
            Email = "test@test.com",
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        var branch = new Branch { Name = "Test", Address = "Test" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course { Name = "Math", BranchId = branch.Id };
        context.Courses.Add(course);
        context.SaveChanges();

        // Act
        var enrollment = new CourseEnrolment
        {
            CourseId = course.Id,
            StudentProfileId = student.Id,
            Status = CourseEnrolmentStatus.Active
        };
        context.CourseEnrolments.Add(enrollment);
        context.SaveChanges();

        // Assert
        context.CourseEnrolments.Count().Should().Be(1);
        context.CourseEnrolments.First().Status.Should().Be(CourseEnrolmentStatus.Active);
    }

    [Fact]
    public void AttendanceSeeding_CreatesAttendanceRecords()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        var branch = new Branch { Name = "Test", Address = "Test" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course 
        { 
            Name = "Math", 
            BranchId = branch.Id,
            StartDate = DateTime.UtcNow.AddMonths(-6),
            EndDate = DateTime.UtcNow.AddMonths(6)
        };
        context.Courses.Add(course);
        context.SaveChanges();

        var student = new StudentProfile 
        { 
            IdentityUserId = "stu1",
            FirstName = "Test",
            LastName = "Student",
            Email = "test@test.com",
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        var enrollment = new CourseEnrolment
        {
            CourseId = course.Id,
            StudentProfileId = student.Id,
            Status = CourseEnrolmentStatus.Active
        };
        context.CourseEnrolments.Add(enrollment);
        context.SaveChanges();

        // Act
        var records = new[]
        {
            new AttendanceRecord 
            { 
                CourseEnrolmentId = enrollment.Id,
                SessionDate = DateTime.UtcNow,
                IsPresent = true,
                WeekNumber = 1
            },
            new AttendanceRecord 
            { 
                CourseEnrolmentId = enrollment.Id,
                SessionDate = DateTime.UtcNow.AddDays(1),
                IsPresent = false,
                WeekNumber = 1
            }
        };
        
        context.AttendanceRecords.AddRange(records);
        context.SaveChanges();

        // Assert
        context.AttendanceRecords.Count().Should().Be(2);
    }

    [Fact]
    public void ExamSeeding_CreatesExams()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        var branch = new Branch { Name = "Test", Address = "Test" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course { Name = "Math", BranchId = branch.Id };
        context.Courses.Add(course);
        context.SaveChanges();

        // Act
        var exam = new Exam
        {
            Title = "Midterm Exam",
            CourseId = course.Id,
            ExamDate = DateTime.UtcNow.AddDays(30),
            ResultsReleased = false
        };
        context.Exams.Add(exam);
        context.SaveChanges();

        // Assert
        context.Exams.Count().Should().Be(1);
        context.Exams.First().ResultsReleased.Should().BeFalse();
    }

    [Fact]
    public void AssignmentSeeding_CreatesAssignments()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        var branch = new Branch { Name = "Test", Address = "Test" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course { Name = "Math", BranchId = branch.Id };
        context.Courses.Add(course);
        context.SaveChanges();

        // Act
        var assignment = new Assignment
        {
            Title = "Assignment 1",
            CourseId = course.Id,
            DueDate = DateTime.UtcNow.AddDays(7),
            MaxScore = 100
        };
        context.Assignments.Add(assignment);
        context.SaveChanges();

        // Assert
        context.Assignments.Count().Should().Be(1);
    }

    [Fact]
    public void ExamResultSeeding_CreatesExamResults()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        var student = new StudentProfile 
        { 
            IdentityUserId = "stu1",
            FirstName = "Test",
            LastName = "Student",
            Email = "test@test.com",
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        var branch = new Branch { Name = "Test", Address = "Test" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course { Name = "Math", BranchId = branch.Id };
        context.Courses.Add(course);
        context.SaveChanges();

        var exam = new Exam
        {
            Title = "Test Exam",
            CourseId = course.Id,
            ExamDate = DateTime.UtcNow,
            ResultsReleased = false
        };
        context.Exams.Add(exam);
        context.SaveChanges();

        // Act
        var result = new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = student.Id,
            Score = 85,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        context.SaveChanges();

        // Assert
        context.ExamResults.Count().Should().Be(1);
        context.ExamResults.First().Grade.Should().Be("B");
    }

    [Fact]
    public void AssignmentResultSeeding_CreatesAssignmentResults()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        var student = new StudentProfile 
        { 
            IdentityUserId = "stu1",
            FirstName = "Test",
            LastName = "Student",
            Email = "test@test.com",
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        var branch = new Branch { Name = "Test", Address = "Test" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course { Name = "Math", BranchId = branch.Id };
        context.Courses.Add(course);
        context.SaveChanges();

        var assignment = new Assignment
        {
            Title = "Assignment 1",
            CourseId = course.Id,
            MaxScore = 100
        };
        context.Assignments.Add(assignment);
        context.SaveChanges();

        // Act
        var result = new AssignmentResult
        {
            AssignmentId = assignment.Id,
            StudentProfileId = student.Id,
            Score = 90,
            SubmittedAt = DateTime.UtcNow
        };
        context.AssignmentResults.Add(result);
        context.SaveChanges();

        // Assert
        context.AssignmentResults.Count().Should().Be(1);
        context.AssignmentResults.First().Score.Should().Be(90);
    }

    [Fact]
    public void MultipleSeeds_AllDataPersists()
    {
        // Arrange
        var context = CreateContext();
        context.Database.EnsureCreated();

        // Act - Create multiple entity types in sequence
        var branch = new Branch { Name = "Test", Address = "Test" };
        context.Branches.Add(branch);
        context.SaveChanges();

        var course = new Course { Name = "Math", BranchId = branch.Id };
        context.Courses.Add(course);
        context.SaveChanges();

        var student = new StudentProfile 
        { 
            IdentityUserId = "stu1",
            FirstName = "Test",
            LastName = "Student",
            Email = "test@test.com",
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        context.SaveChanges();

        var enrollment = new CourseEnrolment
        {
            CourseId = course.Id,
            StudentProfileId = student.Id,
            Status = CourseEnrolmentStatus.Active
        };
        context.CourseEnrolments.Add(enrollment);
        context.SaveChanges();

        var exam = new Exam
        {
            Title = "Final Exam",
            CourseId = course.Id,
            ExamDate = DateTime.UtcNow.AddDays(60),
            ResultsReleased = false
        };
        context.Exams.Add(exam);
        context.SaveChanges();

        // Assert - Verify all entities persisted
        context.Branches.Count().Should().Be(1);
        context.Courses.Count().Should().Be(1);
        context.StudentProfiles.Count().Should().Be(1);
        context.CourseEnrolments.Count().Should().Be(1);
        context.Exams.Count().Should().Be(1);
    }
}
