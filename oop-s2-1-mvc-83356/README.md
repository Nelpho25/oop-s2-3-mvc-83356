# 🎓 VGC College - Student Management System

A comprehensive ASP.NET Core MVC (.NET 8) application for managing student enrollments, courses, faculty assignments, attendance, grades, and exams across multiple branch locations.

**Status**: ✅ **FULLY OPERATIONAL** | Production Ready | Auto-Seeded

---

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (included with Visual Studio)

### Run the Application

```bash
cd oop-s2-1-mvc-83356

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the application
dotnet run
```

✅ **Open browser**: https://localhost:7021

The database auto-seeds on first run with all demo data.

---

## 🔐 Login Credentials

### **ADMIN Account**
```
Email: admin@vgc.ie
Password: Admin@123!
```
**Access**: Full system access, admin dashboard, all management features

---

### **FACULTY Accounts**

**John Smith** (Dublin Branch)
```
Email: john.smith@vgc.ie
Password: Faculty@123!
```

**Mary Jones** (Cork Branch)
```
Email: mary.jones@vgc.ie
Password: Faculty@123!
```

**Paul Murphy** (Galway Branch)
```
Email: paul.murphy@vgc.ie
Password: Faculty@123!
```

---

### **STUDENT Accounts**

| Name | Email | Student ID | Password |
|------|-------|-----------|----------|
| Alice Ryan | alice.ryan@student.vgc.ie | VGC001 | Student@123! |
| Bob Kelly | bob.kelly@student.vgc.ie | VGC002 | Student@123! |
| Claire Walsh | claire.walsh@student.vgc.ie | VGC003 | Student@123! |
| David Byrne | david.byrne@student.vgc.ie | VGC004 | Student@123! |
| Emma Moore | emma.moore@student.vgc.ie | VGC005 | Student@123! |
| Finn Doyle | finn.doyle@student.vgc.ie | VGC006 | Student@123! |

**All passwords**: `Student@123!`

---

## ✨ Key Features

### 🏢 Multi-Branch Management
- 3 locations: Dublin, Cork, Galway
- Branch-specific courses
- Faculty assignments per branch

### 👥 Student Management
- 6 pre-seeded students
- Full enrollment tracking
- Student profiles and dashboards

### 📚 Course Management
- 5 courses across branches
- Faculty assignments
- Course scheduling
- Enrollment tracking

### 📝 Attendance Tracking
- 8-week attendance records per student
- Week 7 marked as absent (demo)
- Individual attendance history
- Attendance reports

### 📊 Grades & Assignments
- Assignment tracking with scores
- Student submissions and feedback
- Automatic grade calculations
- Assignment results with feedback

### 🧪 Exam Management
- Exam scheduling
- Released/unreleased results control
- Semester 1: Results visible ✅
- Semester 2: Results hidden 🔒
- Exam statistics

### 👨‍💼 Admin Dashboard
- System statistics (6 students, 5 courses, 3 branches, 3 faculty)
- Top 3 students by average grade
- Quick action buttons
- Campus overview

### 🔐 Role-Based Access Control
- **Admin**: Full system access
- **Faculty**: Course & grade management
- **Student**: View own data
- Identity-based authentication

---

## 🌐 Important URLs

| Page | URL |
|------|-----|
| **Home Page** | https://localhost:7021 |
| **Login** | https://localhost:7021/Account/Login |
| **Register** | https://localhost:7021/Account/Register |
| **Admin Dashboard** | https://localhost:7021/Admin/Index |
| **Student Dashboard** | https://localhost:7021/StudentDashboard/Dashboard |
| **Faculty Dashboard** | https://localhost:7021/Faculty/Index |
| **Courses** | https://localhost:7021/Course/Index |
| **Students** | https://localhost:7021/Student/Index |
| **Branches** | https://localhost:7021/Branch/Index |
| **Enrollments** | https://localhost:7021/Enrolment/Index |
| **Attendance** | https://localhost:7021/Attendance/Index |
| **Exams** | https://localhost:7021/Exam/Index |
| **Gradebook** | https://localhost:7021/Gradebook/Courses |

---

## 📊 Seed Data Overview

**Automatically Seeded on Startup**:

- **10 Users**: 1 Admin, 3 Faculty, 6 Students
- **5 Courses**: Across 3 branches with faculty assignments
- **3 Branches**: Dublin, Cork, Galway
- **6 Enrollments**: Students enrolled in multiple courses
- **8 Weeks Attendance**: Per enrollment (week 7 absent)
- **11 Assignments**: With scores and feedback for all students
- **Multiple Exams**: Semester 1 (released) & Semester 2 (hidden)
- **Grade Records**: Full academic records for all students

---

## 🛠️ Technology Stack

| Technology | Version |
|-----------|---------|
| **.NET** | 8.0 |
| **ASP.NET Core MVC** | 8.0 |
| **Entity Framework Core** | 8.0 |
| **SQL Server** | LocalDB |
| **Identity Framework** | 8.0 |
| **Bootstrap** | 5.3 |
| **Serilog** | Latest |

---

## 💾 Database

### Connection String
```
Server=(localdb)\mssqllocaldb;Database=aspnet-oop_s2_1_mvc_83356-f341a320-a083-4e29-bed0-2109e3906921;Trusted_Connection=True;MultipleActiveResultSets=true
```

### Database Commands

**Apply Migrations**:
```bash
dotnet ef database update
```

**Reset Database**:
```bash
dotnet ef database drop --force
dotnet ef database update
```

---

## 📁 Project Structure

```
oop-s2-1-mvc-83356/
├── Controllers/              # MVC Controllers
├── Views/                    # Razor Views
├── Pages/                    # Razor Pages (Home)
├── Models/                   # Domain Models
├── Data/                     # DbContext & DbInitializer
├── Services/                 # Business Logic
├── Controllers/Api/          # API Endpoints
└── Properties/               # Configuration
```

---

## 🧪 Testing

Run unit tests:
```bash
dotnet test
```

---

## ✅ Important Notes

✅ **Database auto-seeds** on application startup  
✅ **All 10 users** pre-configured with correct credentials  
✅ **Full academic records** for all 6 students  
✅ **Ready for testing** - no additional setup needed  
✅ **Try all 3 roles** - Admin, Faculty, Student  
✅ **Responsive design** - works on all devices  

---

## 📞 Repository

**GitHub**: https://github.com/Nelpho25/oop-s2-1-mvc-83356

---

**Last Updated**: March 31, 2026  
**Status**: Production Ready ✅
