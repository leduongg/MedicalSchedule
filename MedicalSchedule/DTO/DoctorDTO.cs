namespace MedicalScheduleAPI.DTO
{
    public class DoctorDTO
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }

    public class DoctorDTOPut
    {
        public string FullName { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
