using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MedicalScheduleAPI.Models
{
    public partial class MedicalScheduleContext : DbContext
    {
        public MedicalScheduleContext()
        {
        }

        public MedicalScheduleContext(DbContextOptions<MedicalScheduleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Doctor> Doctors { get; set; } = null!;
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<Prescription> Prescriptions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =DESKTOP-8U3PQAV; database = MedicalSchedule;uid=sa;pwd=sa;Trusted_Connection=True;Encrypt=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Account__1788CCAC40901449");

                entity.ToTable("Account");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UserType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");

                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");

                entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");

                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Docto__403A8C7D");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Appointme__Patie__3F466844");
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor");

                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Specialty)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Doctors)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Doctor__UserID__398D8EEE");
            });

            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__MedicalR__FBDF78C983CD10BE");

                entity.ToTable("MedicalRecord");

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.MedicalHistory).HasColumnType("text");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.RecordDate).HasColumnType("date");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__MedicalRe__Patie__4316F928");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Patient__UserID__3C69FB99");
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.ToTable("Prescription");

                entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");

                entity.Property(e => e.Dosage)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Frequency)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Instructions).HasColumnType("text");

                entity.Property(e => e.MedicationName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__Prescript__Recor__45F365D3");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
