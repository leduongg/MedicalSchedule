using System;
using System.Collections.Generic;

namespace MedicalScheduleAPI.Models
{
    public partial class MedicalRecord
    {
        public MedicalRecord()
        {
            Prescriptions = new HashSet<Prescription>();
        }

        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public DateTime RecordDate { get; set; }
        public string? MedicalHistory { get; set; }

        public virtual Patient Patient { get; set; } = null!;
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}
