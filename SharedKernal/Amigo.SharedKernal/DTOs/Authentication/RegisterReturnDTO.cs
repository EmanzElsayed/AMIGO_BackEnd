using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Authentication
{
    public class RegisterReturnDTO
    {
        public string FullName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
