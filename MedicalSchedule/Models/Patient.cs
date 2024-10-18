using System;
using System.Collections.Generic;

namespace MedicalScheduleAPI.Models
{
    public partial class Patient
    {
        public Patient()
        {
            Appointments = new HashSet<Appointment>();
            MedicalRecords = new HashSet<MedicalRecord>();
        }

        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public virtual Account User { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
