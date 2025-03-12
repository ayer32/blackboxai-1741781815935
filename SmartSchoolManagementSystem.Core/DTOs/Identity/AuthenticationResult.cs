namespace SmartSchoolManagementSystem.Core.DTOs.Identity
{
    public class AuthenticationResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
