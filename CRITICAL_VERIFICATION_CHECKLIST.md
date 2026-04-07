# ✅ CRITICAL VERIFICATION POINTS - Do NOT Miss These

## 🔴 MOST IMPORTANT: Must-Have Features (Non-negotiable for 100 marks)

### 1. **ResultsReleased Rule MUST be Enforced at QUERY Level**
```csharp
// ✅ CORRECT (enforced in database query)
var results = await _context.ExamResults
    .Where(r => r.Exam.ResultsReleased || !User.IsInRole("Student"))
    .ToListAsync();

// ❌ WRONG (only hidden in UI view - NOT SECURE)
// if (result.Exam.ResultsReleased) { show result }
```
**Verification**: Unreleased exam results are IMPOSSIBLE for students to access, even via URL.

### 2. **Authorization MUST be Server-Side Enforced**
```csharp
// ✅ CORRECT
[Authorize(Roles = "Admin")]
public IActionResult DeleteBranch(int id) { ... }

// ❌ WRONG (only hidden in view)
@if (User.IsInRole("Admin")) { <!-- Delete button --> }
```
**Verification**: Student accessing `/Admin/Branches/Delete/1` gets 403 Forbidden.

### 3. **Student Can't Enrol Twice in Same Course**
```csharp
// ✅ Database constraint enforced
var exists = await _context.CourseEnrolments
    .AnyAsync(e => e.StudentProfileId == studentId && e.CourseId == courseId);
if (exists) return null; // or error
```
**Test**: `EnrollStudentAsync_WhenAlreadyEnrolled_ReturnsNull` ✅ (MUST PASS)

### 4. **Email Must Be Unique**
```csharp
// ✅ Database unique index
modelBuilder.Entity<StudentProfile>()
    .HasIndex(s => s.Email)
    .IsUnique();

// ✅ Server-side validation
var exists = await _context.StudentProfiles
    .AnyAsync(s => s.Email == model.Email && s.Id != model.Id);
if (exists) ModelState.AddModelError("Email", "Email already registered.");
```
**Test**: `StudentProfile_Email_MustBeUnique` ✅ (MUST PASS)

---

## ✅ BUILD & TESTS (Must All Pass)

### Run These Commands Before Submission
```powershell
# Command 1: Restore
dotnet restore
# Expected: Restore succeeded with 0-1 warning

# Command 2: Build Release
dotnet build --configuration Release
# Expected: Build succeeded, 0 errors

# Command 3: Test
dotnet test --configuration Release
# Expected: 226 tests pass, 0 failures
```

### All Tests Must Pass
- ✅ No flaky tests (no TimeZone issues, no DateTime.Now without mocking)
- ✅ No trivial tests ("Assert.True(true)")
- ✅ Tests run deterministically every time
- ✅ InMemory database for all tests

---

## 🔐 SECURITY CHECKLIST

### [Authorize] Attributes on Every Sensitive Action
| Controller | Methods | Roles |
|-----------|---------|-------|
| AdminController | All | Admin |
| BranchController | All | Admin |
| StudentController | CRUD | Admin |
| CourseController | CRUD | Admin |
| ExamController | Create/Edit/Delete | Admin,Faculty |
| ExamController | View | Admin,Faculty + Student (released only) |
| AttendanceController | Create/Edit | Admin,Faculty |
| GradebookController | Entry | Admin,Faculty |
| StudentDashboardController | View | Student |

### Faculty Access Control
Faculty should only see:
- Their assigned courses
- Students enrolled in their courses
- Assignments/Exams/Attendance for their courses

Test: `Faculty_OnlySeesStudents_InTheirCourses` ✅

### Student Access Control
Student should only see:
- Their own profile
- Their own enrolments
- Their own grades/exams
- Only RELEASED exam results

Tests:
- ✅ `Student_OnlySeesOwn_Data`
- ✅ `Student_CanSee_ReleasedExamResult`
- ✅ `Student_CannotSee_UnreleasedExamResult`

---

## 🗄️ DATABASE VERIFICATION

### Entities Must Exist
```
✅ Branch
✅ Course
✅ StudentProfile
✅ FacultyProfile
✅ AdminProfile
✅ CourseEnrolment
✅ AttendanceRecord
✅ Assignment
✅ AssignmentResult
✅ Exam
✅ ExamResult
✅ FacultyCourseAssignment
```

### Foreign Keys Must Be Configured
```csharp
// ✅ Each relationship must have .HasForeignKey()
modelBuilder.Entity<CourseEnrolment>()
    .HasOne(ce => ce.StudentProfile)
    .WithMany(sp => sp.CourseEnrolments)
    .HasForeignKey(ce => ce.StudentProfileId)
    .OnDelete(DeleteBehavior.Restrict);
```

### Seed Data Must Exist
- ✅ 3 branches (Dublin, Cork, Galway)
- ✅ Multiple courses per branch
- ✅ Admin user: admin@vgc.ie / Admin@123!
- ✅ Faculty users: faculty@vgc.ie, etc.
- ✅ Student users: student1@vgc.ie, student2@vgc.ie, etc.
- ✅ Enrolments linking students to courses
- ✅ Attendance records
- ✅ Assignments with results
- ✅ Exams with results

---

## 🧪 TEST COVERAGE (Must Have 8+ Tests)

### Critical Tests That MUST Exist and PASS

1. **Exam Visibility** (ResultsReleased enforced)
   ```
   ✅ Student_CannotSee_UnreleasedExamResult
   ✅ Student_CanSee_ReleasedExamResult
   ✅ Admin_CanAlwaysSee_AllExamResults
   ```

2. **Enrolment Rules**
   ```
   ✅ Student_CannotEnrol_TwiceInSameCourse
   ✅ Enrolment_Requires_ValidStudentAndCourse
   ✅ Enrolment_DefaultStatus_IsActive
   ```

3. **Grade Calculation** (All 5 grades achievable)
   ```
   ✅ Score 90-100 → "A"
   ✅ Score 75-89 → "B"
   ✅ Score 60-74 → "C"
   ✅ Score 50-59 → "D"
   ✅ Score 0-49 → "F"
   ```

4. **Faculty Filtering**
   ```
   ✅ Faculty_OnlySeesStudents_InTheirCourses
   ✅ Faculty_CannotSee_OtherFacultyStudents
   ```

5. **Student Filtering**
   ```
   ✅ Student_OnlySeesOwn_Data
   ✅ Student_CannotAccess_OtherStudentData
   ```

6. **Validation**
   ```
   ✅ AssignmentResult_WithScore_AboveMaxScore_IsInvalid
   ✅ ExamResult_WithScore_AboveMaxScore_IsInvalid
   ```

7. **Attendance**
   ```
   ✅ AttendanceRecord_IsLinked_ToCorrectEnrolment
   ✅ Cannot_CreateAttendance_ForNonExistentEnrolment
   ```

8. **Unique Constraint**
   ```
   ✅ StudentProfile_Email_MustBeUnique
   ```

---

## 🔄 CI/CD PIPELINE

### GitHub Actions Workflow Must Have
- ✅ File: `.github/workflows/ci.yml`
- ✅ Triggers: push to "master" + pull_request
- ✅ Steps: Checkout → Setup .NET 8 → Restore → Build → Test
- ✅ Coverage collection: `--collect:"XPlat Code Coverage"`
- ✅ ReportGenerator for HTML reports
- ✅ Artifact upload

### Critical: Workflow MUST Pass on Master
If workflow fails:
- ❌ 0/10 marks for CI/CD
- ❌ Application may be considered non-functional

**Test**: Push to master and verify green checkmark ✅

---

## 📚 README Documentation

Must Include:
- [x] Project Overview
- [x] Stack (ASP.NET Core MVC, EF Core, Identity, xUnit)
- [x] Setup: dotnet restore, ef database update, dotnet run
- [x] Running Tests: cd tests/VgcCollege.Tests && dotnet test
- [x] Demo Accounts with ALL credentials
- [x] Design decisions

---

## 🎯 POINTS THAT LOSE MARKS THE MOST

### ❌ Will Lose 10+ Marks:
1. **ResultsReleased NOT enforced at query level** → Only hidden in UI
2. **[Authorize] missing** → Can access admin pages without auth
3. **Faculty sees all students** → Not filtered by their courses
4. **Student sees all grades** → Including other students' unreleased results
5. **No tests** → Or tests don't actually test security rules
6. **Build fails** → Automatic 0 marks
7. **Tests fail** → Automatic marks loss (depends on count)

### ⚠️ Will Lose 5+ Marks:
1. **No CI/CD pipeline** → 0/10 marks
2. **README incomplete** → 0/3 marks
3. **No demo accounts** → 0/3 marks
4. **Database not seeded** → 0/4 marks
5. **Email not unique** → 0/5 marks validation

### 📝 Will Lose 1-2 Marks:
1. UI styling issues
2. Validation messages missing
3. Error handling incomplete
4. Broken links in navigation
5. Inconsistent design

---

## 🚀 FINAL CHECKLIST BEFORE SUBMISSION

```
PRE-SUBMISSION CHECKLIST:

[ ] dotnet restore succeeds (0 critical errors)
[ ] dotnet build --configuration Release succeeds
[ ] dotnet test passes: 226/226 tests (0 failures)

[ ] All 11+ entities in code: ✓
  [ ] Branch, Course, StudentProfile, FacultyProfile, AdminProfile
  [ ] CourseEnrolment, AttendanceRecord, Assignment, AssignmentResult
  [ ] Exam, ExamResult, FacultyCourseAssignment

[ ] ResultsReleased enforced in QUERY (not just UI): ✓
[ ] [Authorize] attributes on all sensitive actions: ✓
[ ] Faculty filtering implemented: ✓
[ ] Student filtering implemented: ✓
[ ] Email uniqueness enforced: ✓
[ ] Duplicate enrolment prevention: ✓

[ ] 8+ unit tests exist and pass: ✓
[ ] Critical tests present:
  [ ] ExamVisibilityTests
  [ ] EnrolmentRuleTests
  [ ] GradeCalculationTests
  [ ] FacultyFilteringTests
  [ ] StudentDataFilteringTests
  [ ] ValidationTests
  [ ] AttendanceTests

[ ] .github/workflows/ci.yml exists: ✓
[ ] Workflow triggers on master push: ✓
[ ] README.md complete with demo accounts: ✓

[ ] Application runs without errors: ✓
[ ] Login works for all 3 roles: ✓
[ ] No broken links or dead pages: ✓
[ ] Error pages (404, 403, 500) configured: ✓

READY FOR SUBMISSION: ✅
```

---

**Important**: If ANY of the critical items above are NOT met, the application will lose significant marks. Double-check BEFORE pushing to GitHub.

**Submission Status**: Ready when all checkboxes are checked ✅
