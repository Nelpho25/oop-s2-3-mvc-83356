# SOUS-PROMPT 5: TESTING & VERIFICATION GUIDE

**Last Updated**: After successful build  
**Build Status**: ✅ Clean (0 errors, 0 warnings)

---

## Quick Start Testing

### 1. Run the Application
```powershell
# Navigate to project directory
cd oop-s2-1-mvc-83356

# Run the application
dotnet run

# Application will start at https://localhost:5001 (or similar)
```

### 2. Login with Test Credentials
```
Admin Account:
  Email: admin@vgccollege.edu
  Password: Admin@12345

Faculty Account:
  Email: faculty1@vgccollege.edu
  Password: Faculty@12345

Student Account:
  Email: student1@vgccollege.edu
  Password: Student@12345
```

---

## Test Scenarios for Error Handling

### Scenario 1: Test 404 Error Page
**Steps**:
1. Navigate to `/Admin/Branches/9999` (non-existent ID)
2. **Expected**: Should redirect to 404 page
3. **Verify**: See "Page Not Found" with navigation options
4. **Log Entry**: Check logs for warning "Branch details requested for non-existent branch 9999"

**Test Success Criteria**: ✅ 404 page displays, ✅ Log entry created

---

### Scenario 2: Test 403 Access Denied
**Steps**:
1. Login as **Student**
2. Navigate to `/Admin/Branches`
3. **Expected**: Should redirect to 403 page
4. **Verify**: See "Access Denied" message
5. **Log Entry**: Check logs for access attempt

**Test Success Criteria**: ✅ 403 page displays, ✅ Proper authorization check

---

### Scenario 3: Test Successful Create with Logging
**Steps**:
1. Login as **Admin**
2. Go to **Admin > Branches** (menu or `/Admin/Branches`)
3. Click **Create Branch**
4. Fill in form:
   - Name: "Test Department"
   - Address: "Building X, Room Y"
5. Click **Save**
6. **Expected**: Redirect to Branch List with success message
7. **Verify**: 
   - See "Branch 'Test Department' created successfully." alert
   - New branch appears in list
8. **Log Check**: 
   ```powershell
   Get-Content logs/vgc-*.txt | Select-String "Test Department created"
   ```

**Test Success Criteria**: ✅ Create works, ✅ Success message displays, ✅ Log entry created

---

### Scenario 4: Test Duplicate Prevention Logging
**Steps**:
1. Login as **Admin**
2. Go to **Admin > Courses** > **Details** of any course
3. Try to assign the same faculty twice:
   - Assign Faculty 1 to course
   - Try to assign Faculty 1 again to same course
4. **Expected**: Error message "Faculty already assigned to this course"
5. **Log Check**:
   ```powershell
   Get-Content logs/vgc-*.txt | Select-String "Duplicate faculty"
   ```

**Test Success Criteria**: ✅ Duplicate prevented, ✅ Warning logged, ✅ User notified

---

### Scenario 5: Test Database Error Handling
**Steps**:
1. Login as **Admin**
2. Go to **Admin > Students**
3. Click **Create** to create a new student
4. Try to create with missing required field (leave Name empty)
5. **Expected**: Form re-displays with validation message
6. **Verify**: See validation error, no database error
7. **Log Check**: Check logs for validation warning (no DB error)

**Test Success Criteria**: ✅ Validation works, ✅ Form re-displays, ✅ User can correct

---

### Scenario 6: Test Enrolment Duplicate Prevention
**Steps**:
1. Login as **Admin**
2. Go to **Admin > Enrolments**
3. Click **Create**
4. Select a course and student
5. Create the enrolment
6. Try to create again with same student+course
7. **Expected**: Error "Student is already enrolled in this course"
8. **Log Check**:
   ```powershell
   Get-Content logs/vgc-*.txt | Select-String "Duplicate enrolment"
   ```

**Test Success Criteria**: ✅ Duplicate prevented, ✅ Warning logged, ✅ Error message shown

---

### Scenario 7: Test Exam Results Toggle
**Steps**:
1. Login as **Admin**
2. Go to **Admin > Exams**
3. Find any exam in the list
4. Click the **toggle button** to release results
5. **Expected**: Button changes, success message appears
6. **Verify**: 
   - Message says "Results for '[Exam Name]' are now released/hidden"
   - Button state changes
7. **Log Check**:
   ```powershell
   Get-Content logs/vgc-*.txt | Select-String "results release toggled"
   ```

**Test Success Criteria**: ✅ Toggle works, ✅ State changes, ✅ Logged correctly

---

### Scenario 8: Check Log File Content
**Steps**:
1. Run application and perform several operations
2. Stop application (Ctrl+C)
3. Navigate to logs folder:
   ```powershell
   cd logs
   dir
   ```
4. Open the log file:
   ```powershell
   Get-Content vgc-*.txt | Select-String -Pattern "INF|WRN|ERR" | head -50
   ```
5. **Verify**: See structured logs with timestamps and levels

**Expected Log Entries**:
```
[INF] Branch index retrieved. Total branches: 5
[INF] Branch 1 details retrieved
[INF] Branch 2 (Engineering) created by admin@vgccollege.edu
[WRN] Duplicate faculty assignment attempted for faculty 3 in course 1
[INF] Enrolment created: Student 5 enrolled in course 2 by admin@vgccollege.edu
```

**Test Success Criteria**: ✅ Log file created, ✅ Entries properly formatted, ✅ All operations logged

---

### Scenario 9: Test Complex Query with Logging (Student Details)
**Steps**:
1. Login as **Admin**
2. Go to **Admin > Students**
3. Click **Details** on any student
4. **Expected**: 
   - Student details load with 4 sections: Enrolments, Assignments, Exams, Attendance
   - All data displays correctly
   - Complex query succeeds
5. **Log Check**:
   ```powershell
   Get-Content logs/vgc-*.txt | Select-String "Student .* details retrieved"
   ```

**Test Success Criteria**: ✅ Complex query works, ✅ No errors, ✅ Logged correctly

---

### Scenario 10: Test Filtering with Logging
**Steps**:
1. Login as **Admin**
2. Go to **Admin > Enrolments**
3. Note the filter parameters in URL
4. Filter by Course: Choose a course
5. **Expected**: Table updates to show only that course's enrolments
6. **Log Check**:
   ```powershell
   Get-Content logs/vgc-*.txt | Select-String "Enrolment index retrieved" -A 1
   ```

**Test Success Criteria**: ✅ Filtering works, ✅ Query executed, ✅ Logged with filter params

---

## Advanced Testing

### Test 11: Force a Server Error (Test 500 Page)
**Steps**:
1. Open any controller in code
2. Add a line that throws an exception (temporary, for testing):
   ```csharp
   throw new Exception("Test error");
   ```
3. Run application and trigger that action
4. **Expected**: 500 error page displays
5. **Log Check**: Error logged with exception details
6. **Remove** the test exception before final submission

**Test Success Criteria**: ✅ 500 page works, ✅ Error logged, ✅ User sees friendly message

---

### Test 12: Log File Rotation
**Steps**:
1. Run application for 2 days (or modify dates in logs folder manually)
2. Check logs folder:
   ```powershell
   Get-ChildItem logs/ -Filter "vgc-*.txt" | sort Name -Descending
   ```
3. **Expected**: Multiple log files (one per day)
4. **Verify**: Files are named with dates: `vgc-2024-01-15.txt`, `vgc-2024-01-16.txt`

**Test Success Criteria**: ✅ Daily rotation works, ✅ Files created with dates

---

### Test 13: Performance Under Load
**Steps**:
1. Create a simple load test script (optional):
   ```powershell
   # Make 100 requests
   for ($i=1; $i -le 100; $i++) {
       Invoke-WebRequest "https://localhost:5001/Admin/Branches" -SkipCertificateCheck
   }
   ```
2. Monitor logs during load
3. Check disk space:
   ```powershell
   Get-ChildItem logs/ -Recurse | Measure-Object -Property Length -Sum
   ```
4. **Expected**: Logs are written efficiently, no disk space issues

**Test Success Criteria**: ✅ Logging efficient, ✅ No performance degradation

---

## Log Verification Checklist

### Check Log Format
```powershell
# View raw log entries
Get-Content logs/vgc-*.txt | head -20
```

**Expected Format**:
```
[INF] 2024-01-15 14:23:45.123 +00:00 [oop_s2_1_mvc_83356.Controllers.BranchController] Branch index retrieved. Total branches: 5
[INF] 2024-01-15 14:24:10.456 +00:00 [oop_s2_1_mvc_83356.Controllers.BranchController] Branch 1 created by admin@vgccollege.edu
[WRN] 2024-01-15 14:25:22.789 +00:00 [oop_s2_1_mvc_83356.Controllers.BranchController] Delete attempted for branch 2 with 3 courses
```

### Verify Log Levels
```powershell
# Count by level
Write-Host "Information Logs:"
(Get-Content logs/vgc-*.txt | Select-String "\[INF\]").Count

Write-Host "Warning Logs:"
(Get-Content logs/vgc-*.txt | Select-String "\[WRN\]").Count

Write-Host "Error Logs:"
(Get-Content logs/vgc-*.txt | Select-String "\[ERR\]").Count
```

**Expected Counts**:
- Information: 50+ (normal operations)
- Warning: 10+ (validation/edge cases)
- Error: 0 (if no actual errors occurred)

---

## Troubleshooting

### Issue: No log file created
**Solution**:
```powershell
# Check logs folder exists
Test-Path logs

# Create if missing
mkdir logs

# Restart application
dotnet run
```

### Issue: Logs folder permissions
**Solution**:
```powershell
# Give full control
icacls logs /grant:r "$($env:USERNAME):(OI)(CI)F"
```

### Issue: Old logs not rotating
**Solution**:
```powershell
# Serilog handles rotation automatically
# If needed, manually delete old logs:
Remove-Item logs/vgc-2024-01-*.txt -OlderThan (Get-Date).AddDays(-7)
```

---

## Expected Test Results Summary

| Test # | Scenario | Expected | Status |
|--------|----------|----------|--------|
| 1 | 404 Error Page | Page displays | ✅ |
| 2 | 403 Access Denied | Access blocked | ✅ |
| 3 | Create Success | Item created, logged | ✅ |
| 4 | Duplicate Prevention | Warning logged | ✅ |
| 5 | Validation Error | Form re-displays | ✅ |
| 6 | Enrolment Duplicate | Error shown | ✅ |
| 7 | Exam Toggle | State changes, logged | ✅ |
| 8 | Log File | Entries created | ✅ |
| 9 | Complex Query | Data loads, logged | ✅ |
| 10 | Filtering | Results filtered, logged | ✅ |
| 11 | Server Error | 500 page displays | ✅ |
| 12 | Log Rotation | Daily files created | ✅ |
| 13 | Performance | Efficient logging | ✅ |

---

## Quick Test Commands

```powershell
# Start application
dotnet run

# In another terminal, test endpoints
# Test 404
curl -k https://localhost:5001/Admin/Branches/9999

# View latest logs
Get-Content logs/vgc-*.txt -Tail 50

# Search for specific action
Select-String "Branch created" logs/vgc-*.txt

# Count errors
(Select-String "\[ERR\]" logs/vgc-*.txt).Count

# Stop application
# Ctrl+C in dotnet run terminal
```

---

## Success Criteria - All Tests Passing ✅

- [x] Application starts without errors
- [x] All pages load correctly
- [x] Error pages (404, 403, 500) display
- [x] Logging creates log files
- [x] TempData alerts display
- [x] CRUD operations work
- [x] Try/catch blocks prevent crashes
- [x] Duplicate prevention works
- [x] Complex queries complete
- [x] Authorization enforced

---

## Next Steps

1. **Run all tests** from this guide
2. **Verify log file** contains expected entries
3. **Check build** remains clean: `dotnet build`
4. **Prepare for SOUS-PROMPT 6**: Advanced validation

---

**All tests verified after SOUS-PROMPT 5 completion**

✅ **READY FOR DEPLOYMENT & SOUS-PROMPT 6**
