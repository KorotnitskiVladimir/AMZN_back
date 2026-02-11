namespace AMZN.DTOs.Auth
{
    public class MeResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
