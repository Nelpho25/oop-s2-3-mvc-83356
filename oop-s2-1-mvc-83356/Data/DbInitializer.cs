using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Seed roles if they don't exist
            await SeedRolesAsync(roleManager);

            // Seed identity users and profiles
            await SeedIdentityUsersAsync(userManager, context);

            // Seed branches
            await SeedBranchesAsync(context);

            // Seed courses
            await SeedCoursesAsync(context);

            // Seed faculty course assignments
            await SeedFacultyCourseAssignmentsAsync(context, userManager);

            // Seed course enrolments
            await SeedCourseEnrolmentsAsync(context, userManager);

            // Seed attendance records
            await SeedAttendanceRecordsAsync(context);

            // Seed assignments and assignment results
            await SeedAssignmentsAsync(context);

            // Seed exams and exam results
            await SeedExamsAsync(context);

            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "Faculty", "Student" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedIdentityUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        // Skip if users already exist
        if (context.Users.Any())
            return;

        var adminCredentials = new { Email = "admin@vgc.ie", Password = "Admin@123!", DisplayName = "Administrator", Role = "Admin" };

        var facultyCredentials = new[]
        {
            new { Email = "faculty1@vgc.ie", Password = "Faculty@123!", DisplayName = "Dr. John Smith", Role = "Faculty" },
            new { Email = "faculty2@vgc.ie", Password = "Faculty@123!", DisplayName = "Prof. Mary Johnson", Role = "Faculty" },
            new { Email = "faculty3@vgc.ie", Password = "Faculty@123!", DisplayName = "Dr. Patrick O'Brien", Role = "Faculty" },
        };

        var studentCredentials = new[]
        {
            new { Email = "student1@vgc.ie", Password = "Student@123!", DisplayName = "Alice Brown", Role = "Student" },
            new { Email = "student2@vgc.ie", Password = "Student@123!", DisplayName = "Bob Wilson", Role = "Student" },
            new { Email = "student3@vgc.ie", Password = "Student@123!", DisplayName = "Charlie Davis", Role = "Student" },
            new { Email = "student4@vgc.ie", Password = "Student@123!", DisplayName = "Diana Thompson", Role = "Student" },
            new { Email = "student5@vgc.ie", Password = "Student@123!", DisplayName = "Edward Martinez", Role = "Student" },
            new { Email = "student6@vgc.ie", Password = "Student@123!", DisplayName = "Fiona Garcia", Role = "Student" },
            new { Email = "student7@vgc.ie", Password = "Student@123!", DisplayName = "George Hassan", Role = "Student" },
            new { Email = "student8@vgc.ie", Password = "Student@123!", DisplayName = "Hannah White", Role = "Student" },
            new { Email = "student9@vgc.ie", Password = "Student@123!", DisplayName = "Isaac Lee", Role = "Student" },
            new { Email = "student10@vgc.ie", Password = "Student@123!", DisplayName = "Jessica Miller", Role = "Student" },
        };

        var studentFaker = new Faker<StudentProfile>()
            .RuleFor(s => s.FirstName, f => f.Person.FirstName)
            .RuleFor(s => s.LastName, f => f.Person.LastName)
            .RuleFor(s => s.StudentNumber, f => GenerateStudentNumber(f))
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f => f.Person.Phone)
            .RuleFor(s => s.Address, f => f.Address.FullAddress())
            .RuleFor(s => s.DateOfBirth, f => f.Person.DateOfBirth);

        var facultyFaker = new Faker<FacultyProfile>()
            .RuleFor(f => f.FirstName, f => f.Person.FirstName)
            .RuleFor(f => f.LastName, f => f.Person.LastName)
            .RuleFor(f => f.Email, f => f.Internet.Email())
            .RuleFor(f => f.Phone, f => f.Person.Phone)
            .RuleFor(f => f.Department, f => f.Commerce.Department());

        var adminFaker = new Faker<AdminProfile>()
            .RuleFor(a => a.FirstName, f => f.Person.FirstName)
            .RuleFor(a => a.LastName, f => f.Person.LastName)
            .RuleFor(a => a.Email, f => f.Internet.Email())
            .RuleFor(a => a.Phone, f => f.Person.Phone)
            .RuleFor(a => a.Department, f => "Administration");

        // Create Admin User
        var adminUser = new ApplicationUser
        {
            UserName = adminCredentials.Email,
            Email = adminCredentials.Email,
            DisplayName = adminCredentials.DisplayName,
            CreatedAt = DateTime.UtcNow
        };

        var adminResult = await userManager.CreateAsync(adminUser, adminCredentials.Password);
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminCredentials.Role);

            var adminProfile = new AdminProfile
            {
                IdentityUserId = adminUser.Id,
                FirstName = "System",
                LastName = "Administrator",
                Email = adminCredentials.Email,
                Phone = "+353 1 234 5678",
                Department = "Administration"
            };

            context.AdminProfiles.Add(adminProfile);
        }

        // Create Faculty Users
        foreach (var credential in facultyCredentials)
        {
            var user = new ApplicationUser
            {
                UserName = credential.Email,
                Email = credential.Email,
                DisplayName = credential.DisplayName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, credential.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, credential.Role);

                var facultyFake = facultyFaker.Generate();
                var facultyProfile = new FacultyProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = credential.DisplayName.Split(' ')[0],
                    LastName = credential.DisplayName.Split(' ').Length > 1 ? credential.DisplayName.Split(' ')[1] : credential.DisplayName,
                    Email = credential.Email,
                    Phone = facultyFake.Phone,
                    Department = facultyFake.Department
                };

                context.FacultyProfiles.Add(facultyProfile);
            }
        }

        // Create Student Users
        foreach (var credential in studentCredentials)
        {
            var user = new ApplicationUser
            {
                UserName = credential.Email,
                Email = credential.Email,
                DisplayName = credential.DisplayName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, credential.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, credential.Role);

                var studentFake = studentFaker.Generate();
                var studentProfile = new StudentProfile
                {
                    IdentityUserId = user.Id,
                    FirstName = credential.DisplayName.Split(' ')[0],
                    LastName = credential.DisplayName.Split(' ').Length > 1 ? credential.DisplayName.Split(' ')[1] : credential.DisplayName,
                    StudentNumber = GenerateStudentNumberFromEmail(credential.Email),
                    Email = credential.Email,
                    Phone = studentFake.Phone,
                    Address = studentFake.Address,
                    DateOfBirth = studentFake.DateOfBirth
                };

                context.StudentProfiles.Add(studentProfile);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedBranchesAsync(ApplicationDbContext context)
    {
        if (context.Branches.Any())
            return;

        var branches = new[]
        {
            new Branch { Name = "Dublin Campus", Address = "123 College Street, Dublin, D01 H7P, Ireland" },
            new Branch { Name = "Cork Campus", Address = "456 University Road, Cork, T12 K8AF, Ireland" },
            new Branch { Name = "Galway Campus", Address = "789 Education Avenue, Galway, H91 TK33, Ireland" }
        };

        context.Branches.AddRange(branches);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCoursesAsync(ApplicationDbContext context)
    {
        if (context.Courses.Any())
            return;

        var branches = await context.Branches.ToListAsync();
        var courses = new List<Course>();
        var faker = new Faker();

        var courseNames = new[]
        {
            "Introduction to Computer Science",
            "Web Development Fundamentals",
            "Data Science Essentials",
            "Mobile App Development",
            "Cloud Computing Architecture",
            "Advanced Database Design",
            "Software Engineering Principles",
            "Cybersecurity Basics",
            "Machine Learning 101",
            "Business Analytics",
            "Python Programming",
            "Java Development",
            "C# and .NET Development",
            "JavaScript and React",
            "Database Administration",
            "Network Security",
            "IT Project Management",
            "Digital Marketing",
            "Business Communication",
            "Data Visualization"
        };

        foreach (var branch in branches)
        {
            // Create 6-7 courses per branch to ensure comprehensive coverage
            for (int i = 0; i < 7; i++)
            {
                if (branch.Id * 7 + i < courseNames.Length)
                {
                    var course = new Course
                    {
                        Name = courseNames[branch.Id * 7 + i],
                        BranchId = branch.Id,
                        StartDate = faker.Date.Past(30, DateTime.UtcNow),
                        EndDate = faker.Date.Future(60, DateTime.UtcNow)
                    };

                    courses.Add(course);
                }
            }
        }

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedFacultyCourseAssignmentsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.FacultyCourseAssignments.Any())
            return;

        var facultyProfiles = await context.FacultyProfiles.ToListAsync();
        var courses = await context.Courses.ToListAsync();

        var assignments = new List<FacultyCourseAssignment>();
        var faker = new Faker();

        // Assign each course to at least one faculty member
        foreach (var course in courses)
        {
            var selectedFaculty = faker.PickRandom(facultyProfiles);
            var isTutor = faker.Random.Bool(0.7f); // 70% chance of being tutor

            assignments.Add(new FacultyCourseAssignment
            {
                FacultyProfileId = selectedFaculty.Id,
                CourseId = course.Id,
                IsTutor = isTutor
            });

            // Randomly add a second faculty member as co-instructor (30% chance)
            if (faker.Random.Bool(0.3f) && facultyProfiles.Count > 1)
            {
                var coFaculty = faker.PickRandom(facultyProfiles.Where(f => f.Id != selectedFaculty.Id).ToList());
                assignments.Add(new FacultyCourseAssignment
                {
                    FacultyProfileId = coFaculty.Id,
                    CourseId = course.Id,
                    IsTutor = false
                });
            }
        }

        context.FacultyCourseAssignments.AddRange(assignments);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCourseEnrolmentsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.CourseEnrolments.Any())
            return;

        var students = await context.StudentProfiles.ToListAsync();
        var courses = await context.Courses.ToListAsync();

        var enrolments = new List<CourseEnrolment>();
        var faker = new Faker();

        // Enrol each student in multiple courses
        foreach (var student in students)
        {
            // Each student enrolls in 3-5 courses
            var courseCount = faker.Random.Int(3, Math.Min(5, courses.Count));
            var assignedCourses = faker.PickRandom(courses, courseCount).ToList();

            foreach (var course in assignedCourses)
            {
                var enrolment = new CourseEnrolment
                {
                    StudentProfileId = student.Id,
                    CourseId = course.Id,
                    EnrolDate = DateTime.UtcNow.AddDays(faker.Random.Int(-60, -20)),
                    Status = CourseEnrolmentStatus.Active
                };

                enrolments.Add(enrolment);
            }
        }

        context.CourseEnrolments.AddRange(enrolments);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAttendanceRecordsAsync(ApplicationDbContext context)
    {
        if (context.AttendanceRecords.Any())
            return;

        var enrolments = await context.CourseEnrolments.ToListAsync();
        var records = new List<AttendanceRecord>();

        foreach (var enrolment in enrolments)
        {
            // Create 4 weeks of attendance records
            for (int week = 1; week <= 4; week++)
            {
                var record = new AttendanceRecord
                {
                    CourseEnrolmentId = enrolment.Id,
                    SessionDate = DateTime.UtcNow.AddDays(-(5 - week) * 7),
                    WeekNumber = week,
                    IsPresent = new Faker().Random.Bool(0.85f), // 85% attendance rate
                    Notes = new Faker().Random.Bool(0.3f) ? new Faker().Lorem.Sentence() : null
                };

                records.Add(record);
            }
        }

        context.AttendanceRecords.AddRange(records);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAssignmentsAsync(ApplicationDbContext context)
    {
        if (context.Assignments.Any())
            return;

        var courses = await context.Courses.ToListAsync();
        var assignments = new List<Assignment>();
        var faker = new Faker();

        // Create 2 assignments per course
        foreach (var course in courses)
        {
            for (int i = 1; i <= 2; i++)
            {
                var assignment = new Assignment
                {
                    CourseId = course.Id,
                    Title = faker.Lorem.Sentence(4),
                    MaxScore = faker.Random.Decimal(50, 100),
                    DueDate = DateTime.UtcNow.AddDays(faker.Random.Int(7, 30))
                };

                assignments.Add(assignment);
            }
        }

        context.Assignments.AddRange(assignments);
        await context.SaveChangesAsync();

        // Create assignment results for all students in each course
        var assignmentResults = new List<AssignmentResult>();
        var enrolments = await context.CourseEnrolments.Include(e => e.Course).ToListAsync();

        foreach (var assignment in await context.Assignments.ToListAsync())
        {
            var courseEnrolments = enrolments.Where(e => e.CourseId == assignment.CourseId).ToList();

            foreach (var enrolment in courseEnrolments)
            {
                var result = new AssignmentResult
                {
                    AssignmentId = assignment.Id,
                    StudentProfileId = enrolment.StudentProfileId,
                    Score = faker.Random.Decimal(0, assignment.MaxScore),
                    Feedback = faker.Random.Bool(0.6f) ? faker.Lorem.Paragraph() : null,
                    SubmittedAt = DateTime.UtcNow.AddDays(faker.Random.Int(-7, 0))
                };

                assignmentResults.Add(result);
            }
        }

        context.AssignmentResults.AddRange(assignmentResults);
        await context.SaveChangesAsync();
    }

    private static async Task SeedExamsAsync(ApplicationDbContext context)
    {
        if (context.Exams.Any())
            return;

        var courses = await context.Courses.ToListAsync();
        var exams = new List<Exam>();
        var faker = new Faker();

        // Create 1 exam per course
        for (int i = 0; i < courses.Count; i++)
        {
            var exam = new Exam
            {
                CourseId = courses[i].Id,
                Title = $"Final Examination - {courses[i].Name}",
                ExamDate = DateTime.UtcNow.AddDays(faker.Random.Int(14, 45)),
                MaxScore = 100m,
                ResultsReleased = i % 2 == 0 // Alternate released/unreleased
            };

            exams.Add(exam);
        }

        context.Exams.AddRange(exams);
        await context.SaveChangesAsync();

        // Create exam results for all students in each course
        var examResults = new List<ExamResult>();
        var enrolments = await context.CourseEnrolments.Include(e => e.Course).ToListAsync();

        foreach (var exam in await context.Exams.ToListAsync())
        {
            var courseEnrolments = enrolments.Where(e => e.CourseId == exam.CourseId).ToList();

            foreach (var enrolment in courseEnrolments)
            {
                var score = faker.Random.Decimal(0, exam.MaxScore);
                var grade = CalculateGrade(score, exam.MaxScore);

                var result = new ExamResult
                {
                    ExamId = exam.Id,
                    StudentProfileId = enrolment.StudentProfileId,
                    Score = score,
                    Grade = grade
                };

                examResults.Add(result);
            }
        }

        context.ExamResults.AddRange(examResults);
        await context.SaveChangesAsync();
    }

    private static string CalculateGrade(decimal score, decimal maxScore)
    {
        var percentage = (score / maxScore) * 100;

        return percentage switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }

    private static string GenerateStudentNumber(Faker faker)
    {
        return $"VGC-{DateTime.UtcNow.Year}-{faker.Random.String2(4, "0123456789ABCDEF")}";
    }

    private static string GenerateStudentNumberFromEmail(string email)
    {
        var year = DateTime.UtcNow.Year;
        var hash = email.GetHashCode().ToString("X4");
        return $"VGC-{year}-{hash}";
    }
}
