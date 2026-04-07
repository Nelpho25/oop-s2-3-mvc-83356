# GitHub Workflow Quick Reference

## 📋 Workflow File
**Location**: `.github/workflows/ci.yml`  
**Name**: `Test & Publish Coverage (xUnit + ReportGenerator)`

---

## ✅ What It Does

### 1. **Builds Your Code**
- ✅ Restores NuGet packages
- ✅ Builds in Release mode
- ✅ Validates compilation

### 2. **Runs All Tests**
- ✅ Executes `tests/VgcCollege.Tests/VgcCollege.Tests.csproj`
- ✅ Runs 7+ test classes with 50+ test methods
- ✅ Collects code coverage data

### 3. **Generates Coverage Reports**
- ✅ Creates HTML coverage report
- ✅ Generates coverage badges
- ✅ Enforces 30% minimum coverage
- ✅ Provides detailed line-by-line coverage

### 4. **Deploys to GitHub Pages**
- ✅ Publishes coverage report online
- ✅ Creates public dashboard
- ✅ Available at: `https://<user>.github.io/<repo>/`

---

## 🔄 When It Runs

✅ **Manual**: Click "Run workflow" button  
✅ **Push**: Any push to `master` or `main`  
✅ **Pull Request**: Any PR to `master` or `main`  

---

## 📊 Test Coverage

**Test Project**: `VgcCollege.Tests`

**Test Categories**:
- **5 Model Tests**: Branch, Course, StudentProfile, CourseEnrolment, Exam & Assignment
- **4 Service Tests**: Attendance, Enrolment, Grade, ExamResult
- **2 Data Tests**: DbInitializer (unit + integration)

**Coverage Tools**:
- **Collector**: Coverlet
- **Reporter**: ReportGenerator
- **Format**: HTML + Cobertura + Badges

---

## 📈 Outputs

### Artifacts (Available for Download)
1. **test-results-<number>** (30-day retention)
   - TRX format test results
   - Detailed test logs

2. **coverage-report-<number>** (30-day retention)
   - HTML coverage report
   - Source view with coverage highlighting

### GitHub Pages
1. **URL**: `https://<username>.github.io/<repo>/`
2. **Content**: Latest coverage report
3. **Updated**: After each push to master

### Workflow Summary
Shows in GitHub Actions UI:
- Test project name
- Test framework (xUnit)
- Coverage percentage
- Artifact links

---

## 🎯 Key Configuration

```yaml
DOTNET_VERSION: "8.0.x"
TEST_PROJECT: "tests/VgcCollege.Tests/VgcCollege.Tests.csproj"
COVERAGE_THRESHOLD: 30%
RETENTION_DAYS: 30
```

---

## ✨ Special Features

### 1. **Concurrency Control**
- Prevents overlapping Pages deployments
- Cancels in-progress deployments when new push occurs

### 2. **Conditional Deployment**
- Only deploys to GitHub Pages on push to master/main
- Skips deploy on pull requests

### 3. **Coverage Badge Support**
- Generates badge images for coverage percentage
- Can be embedded in README

### 4. **Workflow Summary**
- Auto-generates GitHub Actions summary
- Shows coverage percentage
- Lists artifact links

### 5. **Multi-Branch Support**
- Works on both `master` and `main`
- Flexible for repository migration

---

## 🚀 First-Time Setup

### 1. Enable GitHub Pages
```
Repository Settings → Pages → Source: GitHub Actions
```

### 2. Run Workflow
```
Go to Actions → Run workflow → Select branch
```

### 3. View Results
```
Check Artifacts tab for downloads
Check Environments for Pages deployment
```

---

## 📝 Steps Breakdown

### Build Job (build-and-test)

| # | Step | Action | Output |
|---|------|--------|--------|
| 1 | Checkout | Gets latest code | Ready to build |
| 2 | Setup .NET | Installs .NET 8 | Environment ready |
| 3 | Restore | Downloads packages | Dependencies ready |
| 4 | Show Directory | Lists working folder | Debug info |
| 5 | Build | Compiles code | Release binary |
| 6 | Test | Runs xUnit tests | Test results + coverage |
| 7 | Install Report Tool | Gets ReportGenerator | Tool ready |
| 8 | Generate Report | Creates HTML report | Coverage data |
| 9 | Extract Coverage | Calculates percentage | Coverage % |
| 10 | Upload Test Results | Saves test artifacts | Available for download |
| 11 | Upload Coverage | Saves coverage artifact | Available for download |
| 12 | Check Threshold | Validates 30% minimum | Pass/Fail |
| 13 | Test Summary | Creates summary | Shown in Actions UI |

### Upload Pages Job (upload-pages)

| # | Step | Action |
|---|------|--------|
| 1 | Download Report | Gets coverage report |
| 2 | Upload Pages | Prepares for deployment |

### Deploy Job (deploy)

| # | Step | Action |
|---|------|--------|
| 1 | Deploy Pages | Publishes to GitHub Pages |

---

## 🔍 Monitoring the Workflow

### In GitHub Actions UI
1. Click "Actions" tab
2. Select "Test & Publish Coverage"
3. Click latest run
4. Monitor step progress in real-time
5. View outputs and logs for each step

### After Completion
1. Download artifacts from Artifacts tab
2. Check GitHub Pages deployment
3. View coverage report online
4. Review workflow summary

---

## 🧪 Test Details

### Included Test Files
```
Model Tests (5):
├── BranchTests.cs
├── CourseTests.cs
├── CourseEnrolmentTests.cs
├── StudentProfileTests.cs
└── ExamAndAssignmentTests.cs

Service Tests (4):
├── AttendanceServiceTests.cs
├── EnrolmentServiceTests.cs
├── GradeServiceTests.cs
└── ExamResultServiceTests.cs

Data Tests (2):
├── DbInitializerTests.cs
└── DbInitializerIntegrationTests.cs
```

### What's Tested
- ✅ Entity validation
- ✅ Service business logic
- ✅ Database initialization
- ✅ Data layer operations
- ✅ Integration scenarios

---

## 📊 Coverage Expectations

### Target Coverage
- **Minimum**: 30%
- **Good**: 50-70%
- **Excellent**: 70%+

### Coverage Report Includes
- Line coverage percentage
- Branch coverage
- Method coverage
- File-by-file breakdown
- Source code highlighting
- Coverage trends

---

## 🛠️ Troubleshooting

### Tests Fail
1. Check test results artifact
2. Review test logs in workflow
3. Run tests locally: `dotnet test tests/VgcCollege.Tests`

### Coverage Low
1. Download coverage report artifact
2. Open `index.html` in browser
3. Identify untested code
4. Add more test cases

### Pages Not Deploying
1. Verify branch is `master` or `main`
2. Check Pages settings in repo
3. See deploy job logs

### Build Fails
1. Check build logs in workflow
2. Verify .NET 8 compatibility
3. Run locally: `dotnet build`

---

## 💡 Best Practices

1. **Write Tests**: Add test cases for new features
2. **Run Locally**: Test before pushing
3. **Monitor Coverage**: Keep it above 30%
4. **Review Reports**: Check what's not covered
5. **Fix Failures**: Address test failures immediately

---

## 🎓 Learning Resources

- [xUnit Documentation](https://xunit.net/)
- [Coverlet Coverage](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGeneratorPlus)
- [GitHub Actions](https://docs.github.com/en/actions)
- [GitHub Pages](https://docs.github.com/en/pages)

---

## ✅ Status

**Workflow**: ✅ Configured  
**Tests**: ✅ Included  
**Coverage**: ✅ Reporting  
**Deployment**: ✅ Enabled  

**Ready to use!** Push to master/main to run workflow.

---

## 📞 Support

For issues or questions:
1. Check GitHub Actions logs
2. Review this documentation
3. Check GitHub workflow syntax guide
4. Review test project structure
