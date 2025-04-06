using System;

namespace YourNamespace.Data.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string[] Roles { get; set; }
    }
}