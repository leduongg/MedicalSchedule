using System;
using System.Collections.Generic;

namespace MedicalScheduleAPI.Models
{
    public partial class Account
    {
        public Account()
        {
            Doctors = new HashSet<Doctor>();
            Patients = new HashSet<Patient>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserType { get; set; } = null!;

        public virtual ICollection<Doctor> Doctors { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }
}
