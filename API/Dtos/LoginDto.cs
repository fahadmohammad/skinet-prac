using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace API.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}