using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineClassManagementSystem.Database.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblEnrollment> TblEnrollments { get; set; }

    public virtual DbSet<TblSubClass> TblSubClasses { get; set; }

    public virtual DbSet<TblSubject> TblSubjects { get; set; }

    public virtual DbSet<TblTeachPlan> TblTeachPlans { get; set; }

    public virtual DbSet<TblTeachTracking> TblTeachTrackings { get; set; }

    public virtual DbSet<TblTimeTable> TblTimeTables { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Tbl_Enro__7F68771B68D9E8BC");

            entity.ToTable("Tbl_Enrollments");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EnrollDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("pending");

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
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OpenTime).HasMaxLength(50);
            entity.Property(e => e.Place).HasMaxLength(100);
            entity.Property(e => e.StudentCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<TblSubject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Tbl_Subj__AC1BA3A804EC31F2");

            entity.ToTable("Tbl_Subjects");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SubjectName).HasMaxLength(50);
        });

        modelBuilder.Entity<TblTeachPlan>(entity =>
        {
            entity.HasKey(e => e.TeachPlanId).HasName("PK__Tbl_Teac__4B08B04659FE333C");

            entity.ToTable("Tbl_TeachPlan");

            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Topic).HasMaxLength(255);

            entity.HasOne(d => d.Subject).WithMany(p => p.TblTeachPlans)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeachPlan_Subject");
        });

        modelBuilder.Entity<TblTeachTracking>(entity =>
        {
            entity.HasKey(e => e.TrackingId).HasName("PK__Tbl_Teac__3C19EDF18432B49E");

            entity.ToTable("Tbl_TeachTracking");

            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasPrecision(0);

            entity.HasOne(d => d.TeachPlan).WithMany(p => p.TblTeachTrackings)
                .HasForeignKey(d => d.TeachPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeachTracking_TeachPlan");
        });

        modelBuilder.Entity<TblTimeTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC073BFEBF72");

            entity.ToTable("Tbl_TimeTable");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DayOfWeek).HasMaxLength(15);
            entity.Property(e => e.Mode).HasMaxLength(20);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.TblTimeTables)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Class__571DF1D5");

            entity.HasOne(d => d.Subject).WithMany(p => p.TblTimeTables)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Subje__5812160E");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TblTimeTables)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Teach__59063A47");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Tbl_User__1788CC4CF5EC08CE");

            entity.ToTable("Tbl_Users");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.TelegramUsername).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
