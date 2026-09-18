using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineClassManagementSystem.Database.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<TblEnrollment> TblEnrollments { get; set; }

    public virtual DbSet<TblSubClass> TblSubClasses { get; set; }

    public virtual DbSet<TblSubject> TblSubjects { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TeachPlan> TeachPlans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ClassManagementDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC073BFEBF72");

            entity.Property(e => e.DayOfWeek).HasMaxLength(15);
            entity.Property(e => e.Mode).HasMaxLength(20);

            entity.HasOne(d => d.Class).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Class__571DF1D5");

            entity.HasOne(d => d.Subject).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Subje__5812160E");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Teach__59063A47");
        });

        modelBuilder.Entity<TblEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Tbl_Enro__7F68771B68D9E8BC");

            entity.ToTable("Tbl_Enrollments");

            entity.Property(e => e.EnrollDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TblEnrollments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Enrol__Class__5070F446");

            entity.HasOne(d => d.Student).WithMany(p => p.TblEnrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Enrol__Stude__5165187F");
        });

        modelBuilder.Entity<TblSubClass>(entity =>
        {
            entity.HasKey(e => e.SubClassId).HasName("PK__Tbl_SubC__6F37C0B5AAE36B92");

            entity.ToTable("Tbl_SubClass");

            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.OpenTime).HasMaxLength(50);
            entity.Property(e => e.Place).HasMaxLength(100);
            entity.Property(e => e.StudentCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<TblSubject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Tbl_Subj__AC1BA3A804EC31F2");

            entity.ToTable("Tbl_Subjects");

            entity.Property(e => e.SubjectName).HasMaxLength(50);
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Tbl_User__1788CC4CF5EC08CE");

            entity.ToTable("Tbl_Users");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<TeachPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeachPla__3214EC079F7DEF09");

            entity.Property(e => e.CompletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.Topic).HasMaxLength(200);

            entity.HasOne(d => d.Class).WithMany(p => p.TeachPlans)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeachPlan__Class__5CD6CB2B");

            entity.HasOne(d => d.Subject).WithMany(p => p.TeachPlans)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeachPlan__Subje__5DCAEF64");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeachPlans)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeachPlan__Teach__5BE2A6F2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
