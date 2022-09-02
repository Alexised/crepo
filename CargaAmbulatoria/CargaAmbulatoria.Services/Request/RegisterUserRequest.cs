using CargaAmbulatoria.EntityFramework.Enums;

namespace CargaAmbulatoria.Services.Request
{
    public class RegisterUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public UserRoleEnum ValidRole => Enum.Parse<UserRoleEnum>(Role);
    }
}
