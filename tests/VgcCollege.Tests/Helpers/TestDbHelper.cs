using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;
using oop_s2_1_mvc_83356.Models;

namespace VgcCollege.Tests.Helpers;

/// <summary>
/// Shared helper for creating and seeding test database contexts.
/// Reduces duplication across test files.
/// </summary>
public static class TestDbHelper
{
    /// <summary>
    /// Creates an InMemory DbContext for testing with a unique database name.
    /// </summary>
    public static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    /// <summary>
    /// Seeds basic test data: Branch, Course, StudentProfile.
    /// Returns the created entities for use in tests.
    /// </summary>
    public static async Task<(Branch branch, Course course, StudentProfile student)> SeedBasicDataAsync(
        ApplicationDbContext context)
    {
        var branch = new Branch
        {
            Name = "Test Branch",
            Address = "123 Test Street"
        };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Test Course",
            BranchId = branch.Id,
            StartDate = DateTime.Now.AddMonths(-3),
            EndDate = DateTime.Now.AddMonths(3)
        };
        context.Courses.Add(course);

        var student = new StudentProfile
        {
            FirstName = "Test",
            LastName = "Student",
            Email = $"test{Guid.NewGuid():N}@test.com",
            StudentNumber = $"VGC-{Guid.NewGuid().ToString()[..8]}",
            IdentityUserId = Guid.NewGuid().ToString()
        };
        context.StudentProfiles.Add(student);
        await context.SaveChangesAsync();

        return (branch, course, student);
    }

    /// <summary>
    /// Seeds a course enrollment for a student.
    /// </summary>
    public static async Task<CourseEnrolment> SeedEnrolmentAsync(
        ApplicationDbContext context, int studentId, int courseId)
    {
        var enrolment = new CourseEnrolment
        {
            StudentProfileId = studentId,
            CourseId = courseId,
            EnrolDate = DateTime.Now,
            Status = CourseEnrolmentStatus.Active
        };
        context.CourseEnrolments.Add(enrolment);
        await context.SaveChangesAsync();
        return enrolment;
    }

    /// <summary>
    /// Seeds an exam for a course.
    /// </summary>
    public static async Task<Exam> SeedExamAsync(
        ApplicationDbContext context, int courseId, bool released = true)
    {
        var exam = new Exam
        {
            CourseId = courseId,
            Title = "Test Exam",
            ExamDate = DateTime.Now.AddDays(-7),
            MaxScore = 100,
            ResultsReleased = released
        };
        context.Exams.Add(exam);
        await context.SaveChangesAsync();
        return exam;
    }

    /// <summary>
    /// Seeds an exam result for a student.
    /// </summary>
    public static async Task<ExamResult> SeedExamResultAsync(
        ApplicationDbContext context, int examId, int studentProfileId, decimal score = 85)
    {
        var result = new ExamResult
        {
            ExamId = examId,
            StudentProfileId = studentProfileId,
            Score = score,
            Grade = "B"
        };
        context.ExamResults.Add(result);
        await context.SaveChangesAsync();
        return result;
    }

    /// <summary>
    /// Seeds an assignment for a course.
    /// </summary>
    public static async Task<Assignment> SeedAssignmentAsync(
        ApplicationDbContext context, int courseId)
    {
        var assignment = new Assignment
        {
            CourseId = courseId,
            Title = "Test Assignment",
            MaxScore = 50,
            DueDate = DateTime.Now.AddDays(7)
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();
        return assignment;
    }

    /// <summary>
    /// Seeds attendance records for an enrollment.
    /// </summary>
    public static async Task SeedAttendanceRecordsAsync(
        ApplicationDbContext context, int enrolmentId, int count = 5, int presentCount = 3)
    {
        var startDate = DateTime.Now.AddDays(-count);
        for (int i = 0; i < count; i++)
        {
            context.AttendanceRecords.Add(new AttendanceRecord
            {
                CourseEnrolmentId = enrolmentId,
                SessionDate = startDate.AddDays(i),
                WeekNumber = (i / 7) + 1,
                IsPresent = i < presentCount
            });
        }
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds multiple students for bulk testing.
    /// </summary>
    public static async Task<List<StudentProfile>> SeedMultipleStudentsAsync(
        ApplicationDbContext context, int count = 5)
    {
        var students = new List<StudentProfile>();
        for (int i = 0; i < count; i++)
        {
            var student = new StudentProfile
            {
                FirstName = $"Student{i + 1}",
                LastName = $"Test{i + 1}",
                Email = $"student{i + 1}@test.com",
                StudentNumber = $"VGC-{i + 1:D5}",
                IdentityUserId = Guid.NewGuid().ToString()
            };
            context.StudentProfiles.Add(student);
            students.Add(student);
        }
        await context.SaveChangesAsync();
        return students;
    }

    /// <summary>
    /// Seeds multiple courses for a branch.
    /// </summary>
    public static async Task<List<Course>> SeedMultipleCoursesAsync(
        ApplicationDbContext context, int branchId, int count = 3)
    {
        var courses = new List<Course>();
        for (int i = 0; i < count; i++)
        {
            var course = new Course
            {
                Name = $"Course {i + 1}",
                BranchId = branchId,
                StartDate = DateTime.Now.AddMonths(-3),
                EndDate = DateTime.Now.AddMonths(3)
            };
            context.Courses.Add(course);
            courses.Add(course);
        }
        await context.SaveChangesAsync();
        return courses;
    }
}
