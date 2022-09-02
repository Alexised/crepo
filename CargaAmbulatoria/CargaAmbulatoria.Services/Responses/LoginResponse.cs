using CargaAmbulatoria.Services.Interfaces;

namespace CargaAmbulatoria.Services.Responses
{
    public class LoginResponse : IResponse
    {
        public bool Success { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }

        public LoginResponse(bool success, string error, string token, DateTime validTo, string name, string role)
        {
            Success = success;
            Error = error;
            Token = token;
            ValidTo = validTo;
            Name = name;
            Role = role;
        }

        public LoginResponse(bool success, string error) :
            this(success, error, string.Empty, DateTime.MinValue, string.Empty, string.Empty)
        {

        }

    }
}
