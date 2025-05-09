
    using HomeAssistant.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using System.Threading.Tasks;

    namespace HomeAssistant.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class FileUploadController : ControllerBase
        {
            private readonly AppDbContext _context;

            public FileUploadController(AppDbContext context)
            {
                _context = context;
            }

            [HttpPost("uploadfile")]
            public async Task<IActionResult> UploadFileToDb(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var uploadedFile = new UploadedFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Content = memoryStream.ToArray()
                };

                _context.UploadedFiles.Add(uploadedFile);
                await _context.SaveChangesAsync();

                return Ok(new { message = "The file has been uploaded successfully" });
            }
        }
    }

