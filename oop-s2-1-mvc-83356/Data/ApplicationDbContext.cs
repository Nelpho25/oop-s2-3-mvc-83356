using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<FacultyProfile> FacultyProfiles { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<FacultyCourseAssignment> FacultyCourseAssignments { get; set; }
        public DbSet<CourseEnrolment> CourseEnrolments { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentResult> AssignmentResults { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser configuration
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // StudentProfile configuration
            modelBuilder.Entity<StudentProfile>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<StudentProfile>(s => s.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentProfile>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            modelBuilder.Entity<StudentProfile>()
                .HasIndex(s => s.IdentityUserId)
                .IsUnique();

            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.LastName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.StudentNumber)
                .IsRequired()
                .HasMaxLength(20);

            // FacultyProfile configuration
            modelBuilder.Entity<FacultyProfile>()
                .HasOne(f => f.ApplicationUser)
                .WithOne(u => u.FacultyProfile)
                .HasForeignKey<FacultyProfile>(f => f.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacultyProfile>()
                .HasIndex(f => f.IdentityUserId)
                .IsUnique();

            modelBuilder.Entity<FacultyProfile>()
                .Property(f => f.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<FacultyProfile>()
                .Property(f => f.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<FacultyProfile>()
                .Property(f => f.LastName)
                .IsRequired()
                .HasMaxLength(50);

            // AdminProfile configuration
            modelBuilder.Entity<AdminProfile>()
                .HasOne(a => a.ApplicationUser)
                .WithOne(u => u.AdminProfile)
                .HasForeignKey<AdminProfile>(a => a.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminProfile>()
                .HasIndex(a => a.IdentityUserId)
                .IsUnique();

            modelBuilder.Entity<AdminProfile>()
                .Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<AdminProfile>()
                .Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<AdminProfile>()
                .Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(50);

            // Branch configuration
            modelBuilder.Entity<Branch>()
                .Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Branch>()
                .Property(b => b.Address)
                .IsRequired();

            modelBuilder.Entity<Branch>()
                .HasMany(b => b.Courses)
                .WithOne(c => c.Branch)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course configuration
            modelBuilder.Entity<Course>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.CourseEnrolments)
                .WithOne(ce => ce.Course)
                .HasForeignKey(ce => ce.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Assignments)
                .WithOne(a => a.Course)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Exams)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.FacultyCourseAssignments)
                .WithOne(fca => fca.Course)
                .HasForeignKey(fca => fca.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // FacultyCourseAssignment configuration
            modelBuilder.Entity<FacultyCourseAssignment>()
                .HasOne(fca => fca.FacultyProfile)
                .WithMany(f => f.FacultyCourseAssignments)
                .HasForeignKey(fca => fca.FacultyProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // CourseEnrolment configuration
            modelBuilder.Entity<CourseEnrolment>()
                .HasOne(ce => ce.StudentProfile)
                .WithMany(s => s.CourseEnrolments)
                .HasForeignKey(ce => ce.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseEnrolment>()
                .Property(ce => ce.Status)
                .HasConversion<string>();

            modelBuilder.Entity<CourseEnrolment>()
                .HasMany(ce => ce.AttendanceRecords)
                .WithOne(ar => ar.CourseEnrolment)
                .HasForeignKey(ar => ar.CourseEnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // AttendanceRecord configuration
            modelBuilder.Entity<AttendanceRecord>()
                .Property(ar => ar.Notes)
                .HasMaxLength(500);

            // Assignment configuration
            modelBuilder.Entity<Assignment>()
                .Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Assignment>()
                .Property(a => a.MaxScore)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Assignment>()
                .HasMany(a => a.AssignmentResults)
                .WithOne(ar => ar.Assignment)
                .HasForeignKey(ar => ar.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // AssignmentResult configuration
            modelBuilder.Entity<AssignmentResult>()
                .Property(ar => ar.Score)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AssignmentResult>()
                .HasOne(ar => ar.StudentProfile)
                .WithMany(s => s.AssignmentResults)
                .HasForeignKey(ar => ar.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Exam configuration
            modelBuilder.Entity<Exam>()
                .Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Exam>()
                .Property(e => e.MaxScore)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Exam>()
                .HasMany(e => e.ExamResults)
                .WithOne(er => er.Exam)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExamResult configuration
            modelBuilder.Entity<ExamResult>()
                .Property(er => er.Score)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.StudentProfile)
                .WithMany(s => s.ExamResults)
                .HasForeignKey(er => er.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamResult>()
                .Property(er => er.Grade)
                .HasMaxLength(10);
        }
    }
}
