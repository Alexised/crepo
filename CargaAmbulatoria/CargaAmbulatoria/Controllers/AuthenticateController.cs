using CargaAmbulatoria.Attributes;
using CargaAmbulatoria.EntityFramework.Models;
using CargaAmbulatoria.Services.Helpers;
using CargaAmbulatoria.Services.Request;
using CargaAmbulatoria.Services.Responses;
using CargaAmbulatoria.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargaAmbulatoria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        public readonly AuthenticateService service;

        public AuthenticateController(IConfiguration configuration)
        {
            this.service = new AuthenticateService(configuration);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var response = await service.Authenticate(model);
            if (response != null && response.Success)
                return Ok(response);
            return Unauthorized(response);
        }

        /// <summary>
        /// Validate authorization from bearer token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("authorization")]
        public IActionResult ValidateAuthorization()
        {
            var item = HttpContext.Items.ToList().FirstOrDefault(x => x.Key.Equals("User"));
            if (item.Value == null) return BadRequest();
            var user = (User)item.Value;
            return Ok(new { status = 200, email = user.Email, role = user.Role.ToString(), user = user.Name, userData = user });
        }

        /// <summary>
        /// Validate password token according to sent token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("validate-token")]
        public async Task<IActionResult> ValidatePasswordToken(string token) => Ok(new { Success = await service.ValidatePasswordToken(token) });

        /// <summary>
        /// Find email to send password reset url
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            var response = await service.ForgotPassword(model);
            return Ok(new BaseResponse { Success = response.IsFilled(), Data = response });
        }
        /// <summary>
        /// Reset Password after join to custom url
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model) => Ok(new { Success = await service.ResetPassword(model, HttpContext) });


    }
}
