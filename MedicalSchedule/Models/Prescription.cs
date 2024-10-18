using System;
using System.Collections.Generic;

namespace MedicalScheduleAPI.Models
{
    public partial class Prescription
    {
        public int PrescriptionId { get; set; }
        public int RecordId { get; set; }
        public string MedicationName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public string? Instructions { get; set; }

        public virtual MedicalRecord Record { get; set; } = null!;
    }
}
