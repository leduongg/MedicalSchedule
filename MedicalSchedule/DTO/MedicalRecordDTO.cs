namespace MedicalScheduleAPI.DTO
{
    public class MedicalRecordDTO
    {
        public int RecordId { get; set; }
        public string PatientName { get; set; }
        public DateTime RecordDate { get; set; }
        public string? MedicalHistory { get; set; }
    }

    public class MedicalRecordDTOPut
    {
        public DateTime RecordDate { get; set; }
        public string? MedicalHistory { get; set; }
    }

    public class MedicalRecordDTOPost
    {
        public int PatientId { get; set; }
        public DateTime RecordDate { get; set; }
        public string? MedicalHistory { get; set; }
    }
}
