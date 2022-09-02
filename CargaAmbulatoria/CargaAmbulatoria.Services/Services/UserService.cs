
using CargaAmbulatoria.EntityFramework.DataBase;
using CargaAmbulatoria.EntityFramework.Enums;
using CargaAmbulatoria.EntityFramework.Models;
using CargaAmbulatoria.Services.Helpers;
using CargaAmbulatoria.Services.Helpers.Log;
using CargaAmbulatoria.Services.Request;
using CargaAmbulatoria.Services.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaAmbulatoria.Services.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly CargaAmbulatoriaDbContext dbContext = new CargaAmbulatoriaDbContext();
        private readonly PasswordService passwordService;
        private readonly EmailService emailService;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
            passwordService = new PasswordService(configuration);
            emailService = new EmailService(configuration);
        }
        public async Task<IEnumerable<User>> GetUsers() => dbContext.Users.ToList();

        public async Task<User> GetUser(string id) => await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == id);

        public async Task<BaseResponse> CreateUser(RegisterUserRequest request)
        {
            try
            {
                //Validate Main Properties
                if (request.Name.IsFilled() && request.Email.IsFilled())
                {
                    var userId = Guid.NewGuid();
                    dbContext.Users.Add(new User
                    {
                        UserId = userId.ToString(),
                        Status = UserStatusEnum.Enabled,
                        PasswordStored = passwordService.Encrypt(request.Password), 
                        Role = request.ValidRole,
                        Name = request.Name,
                        Email = request.Email,
                    });

                    await dbContext.SaveChangesAsync();
                    emailService.SendWelcomeMail(request.Email, _configuration["Application:Url"], request.Email, request.Password);
                    return new BaseResponse { Success = true };
                }
            }
            catch (Exception ex)
            {
                //Ignore 
                Log.LogEvent("User", "Create user", ex);
                return new BaseResponse { Success = false, Error = StringExtensions.InternalServerErrorMessage };
            }

            return new BaseResponse { Success = false, Error = StringExtensions.RequiredFieldsEmtpyMessage };
        }

        public async Task<BaseResponse> DisableUser(string id)
        {
            try
            {
                var user = dbContext.Users.FirstOrDefault(x => x.UserId == id);
                if (user == null)
                    return new BaseResponse { Success = false, Error = "El usuario no existe" };
                user.Status = user.Status == UserStatusEnum.Enabled ? UserStatusEnum.Disabled : UserStatusEnum.Enabled;
                await dbContext.SaveChangesAsync();
                return new BaseResponse { Success = true };
            }
            catch (Exception ex)
            {
                Log.LogEvent("User", "Disable User", ex);
                return new BaseResponse { Success = false, Error = StringExtensions.InternalServerErrorMessage };
            }
        }

        //public async Task<User> CreateSimpleUser(string email, string name)
        //{
        //    var user = dbContext.Users.FirstOrDefault(x => x.Email == email);
        //    if (user != null) return user;
        //    var userId = Guid.NewGuid();
        //    var password = RandomString(12);
        //    user = new User
        //    {
        //        UserId = userId.ToString(),
        //        Status = UserStatusEnum.Enabled,
        //        PasswordStored = passwordService.Encrypt(password),
        //        Role = UserRoleEnum.Evaluator,
        //        Name = name,
        //        Email = email,
        //    };
        //    dbContext.Users.Add(user);
        //    await dbContext.SaveChangesAsync();
        //    //emailService.SendWelcomeMail("kevin.alvarez@mitconsulting.com.co", _configuration["Application:Url"], user, password);
        //    emailService.SendWelcomeMail(user.Email, _configuration["Application:Url"], user, password);
        //    return user;
        //}

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
