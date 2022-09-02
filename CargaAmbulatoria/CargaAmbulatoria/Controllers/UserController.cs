using CargaAmbulatoria.Attributes;
using CargaAmbulatoria.EntityFramework.Enums;
using CargaAmbulatoria.Services.Request;
using CargaAmbulatoria.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargaAmbulatoria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        public readonly UserService service;

        public UserController(IConfiguration configuration)
        {
            this.service = new UserService(configuration);
        }

        [Authorize(UserRoleEnum.Admin, UserRoleEnum.Agent)]
        [HttpGet]
        [Route("get-all")]
        public async Task<IActionResult> GetUsers() => Ok(await this.service.GetUsers());

        [Authorize(UserRoleEnum.Admin)]
        [HttpGet]
        [Route("get-user")]
        public async Task<IActionResult> GetUser(string id) => Ok(await this.service.GetUser(id));

        [Authorize(UserRoleEnum.Admin)]
        [HttpPost]
        [Route("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest request)
            => Ok(await this.service.CreateUser(request));

        //[Authorize(UserRoleEnum.Admin)]
        [HttpPost]
        [Route("disable-user")]
        public async Task<IActionResult> DisableUser(string id)
        => Ok(await this.service.DisableUser(id));
    }
}
