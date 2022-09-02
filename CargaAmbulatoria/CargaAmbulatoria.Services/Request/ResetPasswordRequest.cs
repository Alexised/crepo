
namespace CargaAmbulatoria.Services.Request
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string UserId { get; set; }

    }
}
