using CargaAmbulatoria.EntityFramework.DataBase;
using CargaAmbulatoria.EntityFramework.Models;
using CargaAmbulatoria.Services.Helpers;
using CargaAmbulatoria.Services.Interfaces;
using CargaAmbulatoria.Services.Request;
using CargaAmbulatoria.Services.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CargaAmbulatoria.Services.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IConfiguration _configuration;
        private readonly CargaAmbulatoriaDbContext dbContext = new CargaAmbulatoriaDbContext();
        private readonly PasswordService passwordService;
        private readonly EmailService emailService;

        public AuthenticateService(IConfiguration configuration)
        {
            _configuration = configuration;
            passwordService = new PasswordService(configuration);
            emailService = new EmailService(configuration);
        }

        public async Task<LoginResponse> Authenticate(LoginRequest model)
        {
            try
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null && user.Status == EntityFramework.Enums.UserStatusEnum.Enabled
                    && user.PasswordStored == passwordService.Encrypt(model.Password))
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim("UserId", user.UserId),
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("Role", user.Role.ToString()),
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );
                    if (user != null)
                    {
                        user.PasswordTries = 0;
                        dbContext.Entry(user).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    return new LoginResponse(true, string.Empty, new JwtSecurityTokenHandler().WriteToken(token),
                        token.ValidTo, user.Name, user.Role.ToString());
                }
                if(user != null)
                {
                    user.PasswordTries++;
                    if(user.PasswordTries > 4 && user.Status != EntityFramework.Enums.UserStatusEnum.Disabled)
                        user.Status = EntityFramework.Enums.UserStatusEnum.PasswordDisabled;
                    dbContext.Entry(user).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }

                return new LoginResponse(false, user?.Status == EntityFramework.Enums.UserStatusEnum.PasswordDisabled ?
                    "Usuario bloqueado por intentos fallidos, por favor reestablezca la contraseña." : "Usuario o contraseña inválida");
            }
            catch (Exception ex)
            {
                return new LoginResponse(false, ex.Message);
            }
        }

        public async Task<bool> ValidatePasswordToken(string token)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.TokenReset == token);
            if (user == null) return false;
            return user.TokenResetExpiration > DateTime.Now;
        }

        public async Task<string> ForgotPassword(ForgotPasswordRequest model)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null) return null;
            var random = new Random();
            var resultToken = new string(
                Enumerable.Repeat(_configuration["Application:Secret"], 24)
                    .Select(token => token[random.Next(token.Length)]).ToArray());
            emailService.SendResetPasswordMail(user.Email, $"{_configuration["Application:Url"]}/reset-password?token={resultToken}", resultToken);
            user.TokenReset = resultToken;
            user.TokenResetExpiration = DateTime.Now.AddHours(2);

            dbContext.Entry(user).State = EntityState.Modified;
            dbContext.SaveChanges();
            var mail = $"{user.Email.Substring(0, 1)}********" +
                $"{user.Email.Substring(user.Email.LastIndexOf("@") - 4, user.Email.LastIndexOf("@") - 1)}" +
                $"@*******{user.Email.Substring(user.Email.LastIndexOf("."), user.Email.Length - 1)}";
            return mail;
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest model, HttpContext context)
        {
            if (model.Token != null)
            {
                var user = dbContext.Users.FirstOrDefault(u => u.TokenReset == model.Token);
                if (user == null) return false;
                user.PasswordStored = passwordService.Encrypt(model.NewPassword);
                user.TokenReset = null;
                user.Status = EntityFramework.Enums.UserStatusEnum.Enabled;
                dbContext.Entry(user).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return true;
            }
            else if (model.UserId.IsFilled())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.UserId == model.UserId);
                if (user == null) return false;
                user.PasswordStored = passwordService.Encrypt(model.NewPassword);
                user.TokenReset = null;
                user.Status = EntityFramework.Enums.UserStatusEnum.Enabled;
                dbContext.Entry(user).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                try
                {
                    // Find User in Session
                    var item = context.Items.ToList().FirstOrDefault(x => x.Key.Equals("User"));
                    var user = (User)item.Value;

                    // Validate User is old or new and change Password
                    if (user.PasswordStored != passwordService.Encrypt(model.OldPassword))
                    {
                        user.PasswordStored = passwordService.Encrypt(model.NewPassword);
                        user.Status = EntityFramework.Enums.UserStatusEnum.Enabled;
                        dbContext.Entry(user).State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();

                        return true;

                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;

                }
            }
            return false;
        }

        public User GetById(string id) => dbContext.Users.First(u => u.UserId == id);

    }
}
