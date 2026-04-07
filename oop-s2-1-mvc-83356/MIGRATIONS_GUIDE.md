# VGC College - Data Model & Migrations Guide

## ✅ Entities Created

All 11 entities have been created with proper configurations:

1. **ApplicationUser** - Custom Identity user with DisplayName & CreatedAt
2. **Branch** - College branches
3. **Course** - Courses offered by branches
4. **StudentProfile** - Student profiles linked to ApplicationUser
5. **FacultyProfile** - Faculty profiles linked to ApplicationUser
6. **FacultyCourseAssignment** - Junction table (Faculty ↔ Course)
7. **CourseEnrolment** - Student enrollments with Status enum
8. **AttendanceRecord** - Attendance tracking per enrollment
9. **Assignment** - Course assignments
10. **AssignmentResult** - Student assignment scores
11. **Exam** - Course exams
12. **ExamResult** - Student exam scores

## 🔐 Relationships & Constraints

### Key Configurations:
- ✅ All required properties marked with `[Required]`
- ✅ Max length constraints via `[MaxLength]`
- ✅ Unique indexes on StudentNumber & IdentityUserId
- ✅ DeleteBehavior.Restrict to prevent accidental cascade deletes
- ✅ Proper foreign key relationships with [ForeignKey] attributes
- ✅ CourseEnrolmentStatus enum for Status field (string conversion)

### Foreign Key DeleteBehavior:
- **Restrict** (default for most): Prevent deletion if related records exist
- **Cascade**: Only for child records (AttendanceRecord, AssignmentResult, ExamResult)

## 📦 Fluent API Configuration

The ApplicationDbContext includes:
- Unique constraints on StudentProfile.StudentNumber & IdentityUserId
- Unique constraints on FacultyProfile.IdentityUserId
- Required field configurations with max lengths
- Proper relationship navigation
- Property conversion for enum (CourseEnrolmentStatus)

## 🔨 Initial Migration Instructions

### Step 1: Create Initial Migration
```powershell
# From the project root directory
cd oop-s2-1-mvc-83356

# Create the initial migration
dotnet ef migrations add InitialCreate --project oop-s2-1-mvc-83356.csproj
```

### Step 2: Apply Migration to Database
```powershell
# Apply the migration to create the database
dotnet ef database update
```

### Alternative (via Package Manager Console in Visual Studio):
```powershell
# In Visual Studio Package Manager Console
Add-Migration InitialCreate
Update-Database
```

## 📋 Database Connection String

The connection string is configured in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-vgccollege;Trusted_Connection=true;"
}
```

For SQLite (development):
```json
"DefaultConnection": "Data Source=vgccollege.db"
```

Update the connection string in `appsettings.json` as needed before running migrations.

## ✨ What's Next

After migrations are applied:
1. Create CRUD controllers for each entity
2. Implement authorization policies (Restrict, Release Exam Results, etc.)
3. Create Razor Views for all pages
4. Implement attendance tracking
5. Add grade calculation logic for ExamResult
6. Set up seed data with Bogus
7. Configure CI/CD pipeline with GitHub Actions

## 📊 Entity Relationships Diagram

```
ApplicationUser (Identity)
├─ StudentProfile (1:0..1)
│  ├─ CourseEnrolment (1:*)
│  │  └─ AttendanceRecord (1:*)
│  ├─ AssignmentResult (1:*)
│  └─ ExamResult (1:*)
│
└─ FacultyProfile (1:0..1)
   └─ FacultyCourseAssignment (1:*)

Branch (1:*)
└─ Course (1:*)
   ├─ CourseEnrolment (1:*)
   ├─ FacultyCourseAssignment (1:*)
   ├─ Assignment (1:*)
   │  └─ AssignmentResult (1:*)
   └─ Exam (1:*)
      └─ ExamResult (1:*)
```

---

**All entities are ready for initial EF Core migration!** 🚀
