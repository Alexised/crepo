
using CargaAmbulatoria.EntityFramework.Models;
using CargaAmbulatoria.Services.Request;
using CargaAmbulatoria.Services.Responses;
using Microsoft.AspNetCore.Http;

namespace CargaAmbulatoria.Services.Interfaces
{
    public interface IAuthenticateService
    {
        Task<LoginResponse> Authenticate(LoginRequest model);
        User GetById(string id);
        Task<bool> ResetPassword(ResetPasswordRequest model, HttpContext context);
        Task<string> ForgotPassword(ForgotPasswordRequest model);
        Task<bool> ValidatePasswordToken(string token);
    }
}
