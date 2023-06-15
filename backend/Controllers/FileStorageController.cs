using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using Backend.Services;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class FileStorageController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly FileStorageService fileStorageService;

        public FileStorageController(ILogger<UsersController> logger, FileStorageService fileStorageService)
        {
            this.logger = logger;
            this.fileStorageService = fileStorageService;
        }

        //[Authorize]
        [HttpGet("{fileName}")]
        public async Task<ActionResult> DownloadGeneral(string fileName)
        {
            string folderPath = Path.Combine("Events");
            var fileDownloadResult = await fileStorageService.Download(folderPath, fileName);

            if (fileDownloadResult.Status == Status.Failure)
            {
                return new NotFoundResult();
            }
            
            return File(fileDownloadResult.Data.Content, fileDownloadResult.Data.ContentType, fileDownloadResult.Data.Name);            
        }

        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<Result<FileUploadResponseDTO>>> Upload(IFormFile file)
        {
            string folderPath = Path.Combine("Events");
            var fileUploadResult = await fileStorageService.Upload(folderPath, file);

            return new OkObjectResult(fileUploadResult);
        }
    }
}
