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
            await SeedFacultyCourseAssignmentsAsync(context);

            // Seed course enrolments
            await SeedCourseEnrolmentsAsync(context);

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

        // Admin
        var adminUser = new ApplicationUser
        {
            UserName = "admin@vgc.ie",
            Email = "admin@vgc.ie",
            DisplayName = "Administrator",
            CreatedAt = DateTime.UtcNow
        };
        await userManager.CreateAsync(adminUser, "Admin@123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");

        var adminProfile = new AdminProfile
        {
            IdentityUserId = adminUser.Id,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@vgc.ie",
            Phone = "+353 1 234 5678",
            Department = "Administration"
        };
        context.AdminProfiles.Add(adminProfile);

        // Faculty Members
        var facultyData = new[]
        {
            new { Email = "john.smith@vgc.ie", FirstName = "John", LastName = "Smith" },
            new { Email = "mary.jones@vgc.ie", FirstName = "Mary", LastName = "Jones" },
            new { Email = "paul.murphy@vgc.ie", FirstName = "Paul", LastName = "Murphy" }
        };

        foreach (var fData in facultyData)
        {
            var facultyUser = new ApplicationUser
            {
                UserName = fData.Email,
                Email = fData.Email,
                DisplayName = $"{fData.FirstName} {fData.LastName}",
                CreatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(facultyUser, "Faculty@123!");
            await userManager.AddToRoleAsync(facultyUser, "Faculty");

            var facultyProfile = new FacultyProfile
            {
                IdentityUserId = facultyUser.Id,
                FirstName = fData.FirstName,
                LastName = fData.LastName,
                Email = fData.Email,
                Phone = $"+353 {1 + Array.IndexOf(facultyData, fData)} 555 1234",
                Department = "Computer Science"
            };
            context.FacultyProfiles.Add(facultyProfile);
        }

        // Students
        var studentData = new[]
        {
            new { Email = "alice.ryan@student.vgc.ie", FirstName = "Alice", LastName = "Ryan", StudentNumber = "VGC001" },
            new { Email = "bob.kelly@student.vgc.ie", FirstName = "Bob", LastName = "Kelly", StudentNumber = "VGC002" },
            new { Email = "claire.walsh@student.vgc.ie", FirstName = "Claire", LastName = "Walsh", StudentNumber = "VGC003" },
            new { Email = "david.byrne@student.vgc.ie", FirstName = "David", LastName = "Byrne", StudentNumber = "VGC004" },
            new { Email = "emma.moore@student.vgc.ie", FirstName = "Emma", LastName = "Moore", StudentNumber = "VGC005" },
            new { Email = "finn.doyle@student.vgc.ie", FirstName = "Finn", LastName = "Doyle", StudentNumber = "VGC006" }
        };

        foreach (var sData in studentData)
        {
            var studentUser = new ApplicationUser
            {
                UserName = sData.Email,
                Email = sData.Email,
                DisplayName = $"{sData.FirstName} {sData.LastName}",
                CreatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(studentUser, "Student@123!");
            await userManager.AddToRoleAsync(studentUser, "Student");

            var studentProfile = new StudentProfile
            {
                IdentityUserId = studentUser.Id,
                FirstName = sData.FirstName,
                LastName = sData.LastName,
                StudentNumber = sData.StudentNumber,
                Email = sData.Email,
                Phone = $"+353 {85 + Array.IndexOf(studentData, sData)} 123 4567",
                Address = $"{100 + Array.IndexOf(studentData, sData)} Main Street, Dublin",
                DateOfBirth = new DateTime(2003, 1, 1).AddMonths(Array.IndexOf(studentData, sData) * 3)
            };
            context.StudentProfiles.Add(studentProfile);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedBranchesAsync(ApplicationDbContext context)
    {
        if (context.Branches.Any())
            return;

        var branches = new[]
        {
            new Branch { Name = "Dublin Campus", Address = "12 Grafton Street, Dublin 2" },
            new Branch { Name = "Cork Campus", Address = "5 Patrick Street, Cork" },
            new Branch { Name = "Galway Campus", Address = "8 Shop Street, Galway" }
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

        var dublinCourses = new[]
        {
            new { Name = "BSc Computer Science", Branch = branches[0] },
            new { Name = "BSc Software Engineering", Branch = branches[0] }
        };

        var corkCourses = new[]
        {
            new { Name = "BA Business Management", Branch = branches[1] },
            new { Name = "BA Marketing & Digital", Branch = branches[1] }
        };

        var galwayCourses = new[]
        {
            new { Name = "BSc Data Science", Branch = branches[2] }
        };

        foreach (var course in dublinCourses)
            courses.Add(new Course { Name = course.Name, BranchId = course.Branch.Id, StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 5, 31) });

        foreach (var course in corkCourses)
            courses.Add(new Course { Name = course.Name, BranchId = course.Branch.Id, StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 5, 31) });

        foreach (var course in galwayCourses)
            courses.Add(new Course { Name = course.Name, BranchId = course.Branch.Id, StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 5, 31) });

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedFacultyCourseAssignmentsAsync(ApplicationDbContext context)
    {
        if (context.FacultyCourseAssignments.Any())
            return;

        var facultyProfiles = await context.FacultyProfiles.ToListAsync();
        var courses = await context.Courses.Include(c => c.Branch).ToListAsync();

        var assignments = new List<FacultyCourseAssignment>();

        // Dublin courses assigned to John Smith
        var dublinCourses = courses.Where(c => c.Branch.Name == "Dublin Campus").ToList();
        foreach (var course in dublinCourses)
        {
            assignments.Add(new FacultyCourseAssignment
            {
                FacultyProfileId = facultyProfiles[0].Id,
                CourseId = course.Id,
                IsTutor = true
            });
        }

        // Cork courses assigned to Mary Jones
        var corkCourses = courses.Where(c => c.Branch.Name == "Cork Campus").ToList();
        foreach (var course in corkCourses)
        {
            assignments.Add(new FacultyCourseAssignment
            {
                FacultyProfileId = facultyProfiles[1].Id,
                CourseId = course.Id,
                IsTutor = true
            });
        }

        // Galway courses assigned to Paul Murphy
        var galwayCourses = courses.Where(c => c.Branch.Name == "Galway Campus").ToList();
        foreach (var course in galwayCourses)
        {
            assignments.Add(new FacultyCourseAssignment
            {
                FacultyProfileId = facultyProfiles[2].Id,
                CourseId = course.Id,
                IsTutor = true
            });
        }

        context.FacultyCourseAssignments.AddRange(assignments);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCourseEnrolmentsAsync(ApplicationDbContext context)
    {
        if (context.CourseEnrolments.Any())
            return;

        var students = await context.StudentProfiles.ToListAsync();
        var courses = await context.Courses.Include(c => c.Branch).ToListAsync();

        var enrolments = new List<CourseEnrolment>();

        // BSc Computer Science (Dublin) - Alice, Bob, Claire
        var csCourse = courses.FirstOrDefault(c => c.Name == "BSc Computer Science");
        var cssStudents = students.Where(s => s.StudentNumber == "VGC001" || s.StudentNumber == "VGC002" || s.StudentNumber == "VGC003").ToList();
        foreach (var student in cssStudents)
        {
            enrolments.Add(new CourseEnrolment
            {
                StudentProfileId = student.Id,
                CourseId = csCourse.Id,
                EnrolDate = new DateTime(2024, 9, 1),
                Status = CourseEnrolmentStatus.Active
            });
        }

        // BSc Software Engineering (Dublin) - David, Alice
        var seCourse = courses.FirstOrDefault(c => c.Name == "BSc Software Engineering");
        var seStudents = students.Where(s => s.StudentNumber == "VGC004" || s.StudentNumber == "VGC001").ToList();
        foreach (var student in seStudents)
        {
            enrolments.Add(new CourseEnrolment
            {
                StudentProfileId = student.Id,
                CourseId = seCourse.Id,
                EnrolDate = new DateTime(2024, 9, 1),
                Status = CourseEnrolmentStatus.Active
            });
        }

        // BA Business Management (Cork) - Emma, Finn
        var bmCourse = courses.FirstOrDefault(c => c.Name == "BA Business Management");
        var bmStudents = students.Where(s => s.StudentNumber == "VGC005" || s.StudentNumber == "VGC006").ToList();
        foreach (var student in bmStudents)
        {
            enrolments.Add(new CourseEnrolment
            {
                StudentProfileId = student.Id,
                CourseId = bmCourse.Id,
                EnrolDate = new DateTime(2024, 9, 1),
                Status = CourseEnrolmentStatus.Active
            });
        }

        // BA Marketing & Digital (Cork) - Emma
        var mdCourse = courses.FirstOrDefault(c => c.Name == "BA Marketing & Digital");
        var mdStudents = students.Where(s => s.StudentNumber == "VGC005").ToList();
        foreach (var student in mdStudents)
        {
            enrolments.Add(new CourseEnrolment
            {
                StudentProfileId = student.Id,
                CourseId = mdCourse.Id,
                EnrolDate = new DateTime(2024, 9, 1),
                Status = CourseEnrolmentStatus.Active
            });
        }

        // BSc Data Science (Galway) - Bob, Claire
        var dsCourse = courses.FirstOrDefault(c => c.Name == "BSc Data Science");
        var dsStudents = students.Where(s => s.StudentNumber == "VGC002" || s.StudentNumber == "VGC003").ToList();
        foreach (var student in dsStudents)
        {
            enrolments.Add(new CourseEnrolment
            {
                StudentProfileId = student.Id,
                CourseId = dsCourse.Id,
                EnrolDate = new DateTime(2024, 9, 1),
                Status = CourseEnrolmentStatus.Active
            });
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
            // Create 8 weeks of attendance records
            for (int week = 1; week <= 8; week++)
            {
                var isPresent = week == 7 ? false : true;  // Week 7 is absent
                
                var record = new AttendanceRecord
                {
                    CourseEnrolmentId = enrolment.Id,
                    SessionDate = DateTime.UtcNow.AddDays(-(8 - week) * 7),
                    WeekNumber = week,
                    IsPresent = isPresent
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
        var students = await context.StudentProfiles.ToListAsync();
        var assignments = new List<Assignment>();
        var results = new List<AssignmentResult>();

        var csAssignments = new[]
        {
            new { Title = "Programming Fundamentals Lab", MaxScore = 100m, DueDate = new DateTime(2024, 10, 15) },
            new { Title = "Data Structures Essay", MaxScore = 50m, DueDate = new DateTime(2024, 11, 20) },
            new { Title = "Final Project", MaxScore = 100m, DueDate = new DateTime(2025, 1, 10) }
        };

        var seAssignments = new[]
        {
            new { Title = "Requirements Analysis Report", MaxScore = 100m, DueDate = new DateTime(2024, 10, 20) },
            new { Title = "UML Design Assignment", MaxScore = 75m, DueDate = new DateTime(2024, 11, 25) }
        };

        var bmAssignments = new[]
        {
            new { Title = "Market Research Report", MaxScore = 100m, DueDate = new DateTime(2024, 10, 18) },
            new { Title = "Financial Analysis Case Study", MaxScore = 100m, DueDate = new DateTime(2024, 11, 28) }
        };

        var dsAssignments = new[]
        {
            new { Title = "Python Data Analysis Lab", MaxScore = 100m, DueDate = new DateTime(2024, 10, 22) },
            new { Title = "Statistical Methods Report", MaxScore = 80m, DueDate = new DateTime(2024, 12, 5) }
        };

        var csCourse = courses.FirstOrDefault(c => c.Name == "BSc Computer Science");
        if (csCourse != null)
        {
            var cssStudents = students.Where(s => s.StudentNumber == "VGC001" || s.StudentNumber == "VGC002" || s.StudentNumber == "VGC003").ToList();
            
            var assn1 = new Assignment { CourseId = csCourse.Id, Title = csAssignments[0].Title, MaxScore = csAssignments[0].MaxScore, DueDate = csAssignments[0].DueDate };
            assignments.Add(assn1);
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = cssStudents[0].Id, Score = 87, Feedback = "Excellent work, well-structured code." });
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = cssStudents[1].Id, Score = 72, Feedback = "Good effort, improve variable naming." });
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = cssStudents[2].Id, Score = 91, Feedback = "Outstanding submission." });

            var assn2 = new Assignment { CourseId = csCourse.Id, Title = csAssignments[1].Title, MaxScore = csAssignments[1].MaxScore, DueDate = csAssignments[1].DueDate };
            assignments.Add(assn2);
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = cssStudents[0].Id, Score = 44, Feedback = "Very thorough analysis." });
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = cssStudents[1].Id, Score = 38, Feedback = "Good but missing some examples." });
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = cssStudents[2].Id, Score = 47, Feedback = "Excellent, clear and well-referenced." });

            var assn3 = new Assignment { CourseId = csCourse.Id, Title = csAssignments[2].Title, MaxScore = csAssignments[2].MaxScore, DueDate = csAssignments[2].DueDate };
            assignments.Add(assn3);
            results.Add(new AssignmentResult { Assignment = assn3, StudentProfileId = cssStudents[0].Id, Score = 90, Feedback = "Impressive project, great UI." });
            results.Add(new AssignmentResult { Assignment = assn3, StudentProfileId = cssStudents[1].Id, Score = 65, Feedback = "Functional but needs more polish." });
            results.Add(new AssignmentResult { Assignment = assn3, StudentProfileId = cssStudents[2].Id, Score = 88, Feedback = "Well done, solid implementation." });
        }

        var seCourse = courses.FirstOrDefault(c => c.Name == "BSc Software Engineering");
        if (seCourse != null)
        {
            var seStudents = students.Where(s => s.StudentNumber == "VGC004" || s.StudentNumber == "VGC001").ToList();
            
            var assn1 = new Assignment { CourseId = seCourse.Id, Title = seAssignments[0].Title, MaxScore = seAssignments[0].MaxScore, DueDate = seAssignments[0].DueDate };
            assignments.Add(assn1);
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = seStudents[0].Id, Score = 78, Feedback = "Clear requirements, good use cases." });
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = seStudents[1].Id, Score = 83, Feedback = "Strong analysis with good diagrams." });

            var assn2 = new Assignment { CourseId = seCourse.Id, Title = seAssignments[1].Title, MaxScore = seAssignments[1].MaxScore, DueDate = seAssignments[1].DueDate };
            assignments.Add(assn2);
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = seStudents[0].Id, Score = 60, Feedback = "Good structure, some relationships unclear." });
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = seStudents[1].Id, Score = 70, Feedback = "Excellent UML, well-labelled." });
        }

        var bmCourse = courses.FirstOrDefault(c => c.Name == "BA Business Management");
        if (bmCourse != null)
        {
            var bmStudents = students.Where(s => s.StudentNumber == "VGC005" || s.StudentNumber == "VGC006").ToList();
            
            var assn1 = new Assignment { CourseId = bmCourse.Id, Title = bmAssignments[0].Title, MaxScore = bmAssignments[0].MaxScore, DueDate = bmAssignments[0].DueDate };
            assignments.Add(assn1);
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = bmStudents[0].Id, Score = 82, Feedback = "Good primary research, solid conclusions." });
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = bmStudents[1].Id, Score = 74, Feedback = "Decent work, needs stronger analysis." });

            var assn2 = new Assignment { CourseId = bmCourse.Id, Title = bmAssignments[1].Title, MaxScore = bmAssignments[1].MaxScore, DueDate = bmAssignments[1].DueDate };
            assignments.Add(assn2);
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = bmStudents[0].Id, Score = 88, Feedback = "Excellent financial modelling." });
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = bmStudents[1].Id, Score = 69, Feedback = "Good effort, work on ratio analysis." });
        }

        var dsCourse = courses.FirstOrDefault(c => c.Name == "BSc Data Science");
        if (dsCourse != null)
        {
            var dsStudents = students.Where(s => s.StudentNumber == "VGC002" || s.StudentNumber == "VGC003").ToList();
            
            var assn1 = new Assignment { CourseId = dsCourse.Id, Title = dsAssignments[0].Title, MaxScore = dsAssignments[0].MaxScore, DueDate = dsAssignments[0].DueDate };
            assignments.Add(assn1);
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = dsStudents[0].Id, Score = 80, Feedback = "Clean code, good use of pandas." });
            results.Add(new AssignmentResult { Assignment = assn1, StudentProfileId = dsStudents[1].Id, Score = 93, Feedback = "Excellent analysis and visualisations." });

            var assn2 = new Assignment { CourseId = dsCourse.Id, Title = dsAssignments[1].Title, MaxScore = dsAssignments[1].MaxScore, DueDate = dsAssignments[1].DueDate };
            assignments.Add(assn2);
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = dsStudents[0].Id, Score = 62, Feedback = "Good understanding of core concepts." });
            results.Add(new AssignmentResult { Assignment = assn2, StudentProfileId = dsStudents[1].Id, Score = 75, Feedback = "Outstanding, very thorough." });
        }

        context.Assignments.AddRange(assignments);
        context.AssignmentResults.AddRange(results);
        await context.SaveChangesAsync();
    }

    private static async Task SeedExamsAsync(ApplicationDbContext context)
    {
        if (context.Exams.Any())
            return;

        var courses = await context.Courses.ToListAsync();
        var students = await context.StudentProfiles.ToListAsync();
        var exams = new List<Exam>();
        var results = new List<ExamResult>();

        var csCourse = courses.FirstOrDefault(c => c.Name == "BSc Computer Science");
        if (csCourse != null)
        {
            var exam1 = new Exam
            {
                CourseId = csCourse.Id,
                Title = "Semester 1 Exam",
                ExamDate = new DateTime(2025, 1, 20),
                MaxScore = 100m,
                ResultsReleased = true
            };
            exams.Add(exam1);

            var cssStudents = students.Where(s => s.StudentNumber == "VGC001" || s.StudentNumber == "VGC002" || s.StudentNumber == "VGC003").ToList();
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = cssStudents[0].Id, Score = 85, Grade = "A" });
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = cssStudents[1].Id, Score = 71, Grade = "B" });
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = cssStudents[2].Id, Score = 90, Grade = "A" });

            var exam2 = new Exam
            {
                CourseId = csCourse.Id,
                Title = "Semester 2 Exam",
                ExamDate = new DateTime(2025, 5, 20),
                MaxScore = 100m,
                ResultsReleased = false
            };
            exams.Add(exam2);

            results.Add(new ExamResult { Exam = exam2, StudentProfileId = cssStudents[0].Id, Score = 88, Grade = "A" });
            results.Add(new ExamResult { Exam = exam2, StudentProfileId = cssStudents[1].Id, Score = 66, Grade = "C" });
            results.Add(new ExamResult { Exam = exam2, StudentProfileId = cssStudents[2].Id, Score = 92, Grade = "A" });
        }

        var seCourse = courses.FirstOrDefault(c => c.Name == "BSc Software Engineering");
        if (seCourse != null)
        {
            var exam1 = new Exam
            {
                CourseId = seCourse.Id,
                Title = "Semester 1 Exam",
                ExamDate = new DateTime(2025, 1, 22),
                MaxScore = 100m,
                ResultsReleased = true
            };
            exams.Add(exam1);

            var seStudents = students.Where(s => s.StudentNumber == "VGC004" || s.StudentNumber == "VGC001").ToList();
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = seStudents[0].Id, Score = 74, Grade = "B" });
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = seStudents[1].Id, Score = 80, Grade = "A" });
        }

        var dsCourse = courses.FirstOrDefault(c => c.Name == "BSc Data Science");
        if (dsCourse != null)
        {
            var exam1 = new Exam
            {
                CourseId = dsCourse.Id,
                Title = "Semester 1 Exam",
                ExamDate = new DateTime(2025, 1, 21),
                MaxScore = 100m,
                ResultsReleased = true
            };
            exams.Add(exam1);

            var dsStudents = students.Where(s => s.StudentNumber == "VGC002" || s.StudentNumber == "VGC003").ToList();
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = dsStudents[0].Id, Score = 77, Grade = "B" });
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = dsStudents[1].Id, Score = 89, Grade = "A" });
        }

        var bmCourse = courses.FirstOrDefault(c => c.Name == "BA Business Management");
        if (bmCourse != null)
        {
            var exam1 = new Exam
            {
                CourseId = bmCourse.Id,
                Title = "End of Year Exam",
                ExamDate = new DateTime(2025, 5, 15),
                MaxScore = 100m,
                ResultsReleased = false
            };
            exams.Add(exam1);

            var bmStudents = students.Where(s => s.StudentNumber == "VGC005" || s.StudentNumber == "VGC006").ToList();
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = bmStudents[0].Id, Score = 85, Grade = "A" });
            results.Add(new ExamResult { Exam = exam1, StudentProfileId = bmStudents[1].Id, Score = 72, Grade = "B" });
        }

        context.Exams.AddRange(exams);
        context.ExamResults.AddRange(results);
        await context.SaveChangesAsync();
    }
}
