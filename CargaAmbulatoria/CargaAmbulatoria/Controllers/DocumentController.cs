using CargaAmbulatoria.Attributes;
using CargaAmbulatoria.Services.Request;
using CargaAmbulatoria.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargaAmbulatoria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly DocumentService service;
        public DocumentController(IConfiguration configuration)
        {
            this.service = new DocumentService(configuration);
        }

        [HttpGet]
        [Authorize]
        [Route("get-documentByUserId")]
        public async Task<IActionResult> GetPerformances(string id) => Ok(await this.service.GetDocumentsByUser(id));

        [HttpPost]
        [Authorize]
        [Route("create-document")]
        public async Task<IActionResult> CreateDocument([FromBody] DocumentRequest request)
            => Ok(await this.service.CreateDocument(request));

        [HttpGet]
        [Authorize]
        [Route("cohorts")]
        public async Task<IActionResult> GetCohorts()
            => Ok(await this.service.GetCohorts());


        [HttpPost]
        [Authorize]
        [Route("delete-document")]
        public async Task<IActionResult> DeletePerformance(string id)
        => Ok(await this.service.DeleteDocument(id));
    }
}
