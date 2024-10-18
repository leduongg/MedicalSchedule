using System;
using System.Collections.Generic;

namespace MedicalScheduleAPI.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            Appointments = new HashSet<Appointment>();
        }

        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public virtual Account User { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
