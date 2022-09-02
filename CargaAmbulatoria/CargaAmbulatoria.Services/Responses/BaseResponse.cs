using CargaAmbulatoria.Services.Interfaces;

namespace CargaAmbulatoria.Services.Responses
{
    public class BaseResponse : IResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
