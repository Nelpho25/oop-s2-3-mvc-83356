# GitHub Actions CI/CD Workflow Documentation

## Workflow Overview

The updated GitHub Actions workflow (`ci.yml`) provides comprehensive continuous integration and continuous deployment for the VGC College Management System.

**Workflow Name**: `Test & Publish Coverage (xUnit + ReportGenerator)`

---

## ✨ Key Features

### 1. **Comprehensive Test Execution**
- Runs all xUnit tests from `tests/VgcCollege.Tests/VgcCollege.Tests.csproj`
- Tests cover:
  - Models (Branch, Course, StudentProfile, CourseEnrolment, Exam, Assignment)
  - Services (Attendance, Enrolment, Grade, ExamResult)
  - Data (DbInitializer)

### 2. **Code Coverage Reporting**
- Collects coverage data with Coverlet
- Generates HTML reports with ReportGenerator
- Includes coverage badges
- Enforces 30% minimum coverage threshold

### 3. **GitHub Pages Deployment**
- Automatically deploys coverage reports to GitHub Pages
- Only on push to `master` or `main` branch
- Creates public coverage dashboard

### 4. **Artifact Management**
- Stores test results (30-day retention)
- Stores coverage reports (30-day retention)
- Available for download on every run

---

## 🔄 Workflow Triggers

The workflow runs on:
1. **Manual Trigger**: `workflow_dispatch`
2. **Push**: To `master` or `main` branch
3. **Pull Request**: Against `master` or `main` branch

---

## 📋 Workflow Jobs

### Job 1: `build-and-test`
**Purpose**: Build project, run tests, generate coverage reports

**Steps**:
1. Checkout code
2. Setup .NET 8.0
3. Restore dependencies
4. Show working directory (debugging)
5. List project structure (debugging)
6. Build in Release mode
7. Run all xUnit tests with coverage collection
8. Install ReportGenerator
9. Generate HTML and Cobertura coverage reports
10. Extract coverage percentage
11. Upload test results artifact
12. Upload coverage report artifact
13. Check coverage threshold (≥30%)
14. Generate test summary in workflow summary

### Job 2: `upload-pages`
**Condition**: Push to master/main only
**Purpose**: Prepare coverage report for GitHub Pages deployment

**Steps**:
1. Download coverage report artifact
2. Upload as GitHub Pages artifact

### Job 3: `deploy`
**Condition**: Push to master/main only
**Depends On**: `upload-pages` job
**Purpose**: Deploy coverage report to GitHub Pages

**Steps**:
1. Deploy coverage report to GitHub Pages
2. Output deployment URL

---

## 📊 Test Coverage

### Included Tests

#### Model Tests
- `BranchTests.cs` - Branch entity validation
- `CourseTests.cs` - Course entity validation
- `CourseEnrolmentTests.cs` - Enrollment logic
- `StudentProfileTests.cs` - Student profile data
- `ExamAndAssignmentTests.cs` - Exam and assignment models

#### Service Tests
- `AttendanceServiceTests.cs` - Attendance management
- `EnrolmentServiceTests.cs` - Student enrollment
- `GradeServiceTests.cs` - Grade calculation
- `ExamResultServiceTests.cs` - Exam results

#### Data Tests
- `DbInitializerTests.cs` - Unit tests
- `DbInitializerIntegrationTests.cs` - Integration tests

**Total**: 7+ test classes covering models, services, and data layer

---

## 🔧 Configuration

### Environment Variables
```yaml
DOTNET_VERSION: "8.0.x"
TEST_PROJECT: "tests/VgcCollege.Tests/VgcCollege.Tests.csproj"
```

### Coverage Threshold
- **Minimum**: 30% line coverage
- **Action on Failure**: Workflow fails if below threshold

### Retention Policy
- **Test Results**: 30 days
- **Coverage Reports**: 30 days

---

## 📈 Outputs and Artifacts

### Generated Artifacts

1. **Test Results**
   - Format: TRX (Visual Studio Test Results)
   - Name: `test-results-<run-number>`
   - Contains: Detailed test execution logs

2. **Coverage Report**
   - Format: HTML
   - Name: `coverage-report-<run-number>`
   - Contains: 
     - Code coverage statistics
     - File-level coverage details
     - Coverage badges
     - Line-by-line source view

3. **GitHub Pages**
   - URL: `https://<username>.github.io/<repo>/`
   - Content: Latest coverage report
   - Available: When pushed to master/main

### Workflow Summary
The workflow generates a summary that appears in the GitHub Actions UI:
```
## 🧪 Test Execution Summary

- **Test Project**: VgcCollege.Tests
- **Framework**: xUnit
- **Coverage Tool**: Coverlet
- **Report Tool**: ReportGenerator
- **Code Coverage**: XX.X%
- **Artifacts**: Available for download
```

---

## 🚀 Usage

### Running Tests Locally
```powershell
dotnet test tests/VgcCollege.Tests/VgcCollege.Tests.csproj
```

### Generating Coverage Report Locally
```powershell
dotnet test tests/VgcCollege.Tests/VgcCollege.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults

reportgenerator \
  -reports:"./TestResults/**/coverage.cobertura.xml" \
  -targetdir:"./coveragereport" \
  -reporttypes:"Html"
```

### Viewing Coverage Report
1. **Local**: Open `./coveragereport/index.html`
2. **Online**: Visit GitHub Pages URL (after push to master/main)

---

## ✅ Verification Checklist

- ✅ Workflow file syntax valid
- ✅ All test projects included
- ✅ Coverage reporting configured
- ✅ GitHub Pages deployment enabled
- ✅ Artifact management configured
- ✅ Coverage threshold enforced
- ✅ Workflow summary generated
- ✅ Multiple trigger events supported

---

## 🔍 Monitoring & Debugging

### Check Workflow Status
1. Go to **GitHub Actions** tab
2. Click on **"Test & Publish Coverage"** workflow
3. See latest run status

### Download Artifacts
1. Click on a workflow run
2. Scroll to "Artifacts" section
3. Download test results or coverage report

### View Coverage Report Online
1. Check workflow summary (after push to master)
2. Click "Deploy to GitHub Pages" step
3. Find URL: `https://<user>.github.io/<repo>/`

### Troubleshooting
- **Tests fail**: Check test results artifact
- **Coverage low**: Review coverage report artifact
- **Pages not deploy**: Ensure push to master/main
- **Build fails**: Check build logs in workflow

---

## 📝 Test Projects Included

### VgcCollege.Tests
Complete test suite for the VGC College Management System

**Location**: `tests/VgcCollege.Tests/VgcCollege.Tests.csproj`

**Dependencies**:
- xUnit (test framework)
- Coverlet (coverage collection)
- ReportGenerator (coverage reporting)
- Entity Framework Core Test Utilities

---

## 🎯 Next Steps

1. **Push to master/main** to trigger workflow
2. **Monitor GitHub Actions** for results
3. **Download coverage reports** from artifacts
4. **View online** on GitHub Pages (after master push)
5. **Improve coverage** based on reports

---

## 📞 Questions?

Refer to:
- `.github/workflows/ci.yml` - Workflow definition
- `tests/VgcCollege.Tests/` - Test project
- GitHub Actions documentation

---

**Status**: ✅ Workflow configured and ready  
**Last Updated**: 2026-04-06  
**Coverage Tool**: xUnit + Coverlet + ReportGenerator
