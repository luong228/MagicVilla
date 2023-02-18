using System.Reflection.Metadata;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class RegisterationRequestDTO
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
