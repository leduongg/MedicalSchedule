
namespace MedicalScheduleAPI.DTO
{
    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Notes { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
    public class AppointmentDTOs
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Notes { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class AppointmentDTOPut
    {
        public DateTime AppointmentDateTime { get; set; }
        public string Notes { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class AppointmentDTOPost
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Notes { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}