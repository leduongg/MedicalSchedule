namespace MedicalScheduleAPI.DTO
{
    public class PrescriptionDTO
    {
        public int PrescriptionId { get; set; }
        public string PatientName { get; set; }
        public string MedicationName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public string? Instructions { get; set; }

    }

    public class PrescriptionDTOPut
    {
        public string MedicationName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public string? Instructions { get; set; }

    }

    public class PrescriptionDTOPost
    {
        public int RecordId { get; set; }
        public string MedicationName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public string? Instructions { get; set; }

    }

}
