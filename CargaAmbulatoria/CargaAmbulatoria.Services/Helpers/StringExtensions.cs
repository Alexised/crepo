
namespace CargaAmbulatoria.Services.Helpers
{
    public static class StringExtensions
    {
        public const string RequiredFieldsEmtpyMessage = "Por favor rellene todos los campos requeridos.";
        public const string InternalServerErrorMessage = "Error interno del servidor. Por favor intente de nuevo.";
        public static bool IsFilled(this string input) => !string.IsNullOrWhiteSpace(input);
    }
}
