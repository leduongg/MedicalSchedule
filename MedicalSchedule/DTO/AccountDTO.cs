namespace MedicalScheduleAPI.DTO
{
    public class AccountDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AccountDTOs
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserType { get; set; } = null!;
    }

    public class AccountDTOPost
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserType { get; set; } = null!;
    }
}
