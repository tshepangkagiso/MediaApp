using MediaAppWebApi.Data;
using MediaAppWebApi.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace MediaAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly MediaDbContext _context;

        public MediaController(MediaDbContext context)
        {
            _context = context;
        }

        // GET: api/Media
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MediaFile>>> GetMediaFiles()
        {
            return await _context.MediaFiles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMediaFile(int id)
        {
            var mediaFile = await _context.MediaFiles.FindAsync(id);
            if (mediaFile == null)
            {
                return NotFound();
            }

            var contentType = !string.IsNullOrWhiteSpace(mediaFile.FileType)
                ? mediaFile.FileType
                : "application/octet-stream"; // Fallback if file type is missing

            Console.WriteLine($"Returning file: {mediaFile.FileName}, Type: {contentType}");

            return File(mediaFile.FileData, contentType, mediaFile.FileName);
        }


        // GET: api/Media/Base64/5
        [HttpGet("Base64/{id}")]
        public async Task<IActionResult> GetMediaFileBase64(int id)
        {
            var mediaFile = await _context.MediaFiles.FindAsync(id);
            if (mediaFile == null) return NotFound();

            if (!MimeTypes.IsImage(mediaFile.FileType))
                return BadRequest("The requested file is not an image.");

            var base64String = Convert.ToBase64String(mediaFile.FileData);
            var result = new
            {
                FileName = mediaFile.FileName,
                FileType = mediaFile.FileType,
                Base64Data = base64String
            };

            return Ok(result);
        }

        // POST: api/Media
        [HttpPost]
        public async Task<ActionResult<MediaFile>> PostMediaFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileType = file.ContentType;

            // If ContentType is empty, detect it using file extension
            if (string.IsNullOrEmpty(fileType))
            {
                fileType = GetMimeTypeFromFileName(file.FileName);
                Console.WriteLine($"Detected MIME type: {fileType}");
            }

            if (string.IsNullOrEmpty(fileType))
            {
                return BadRequest("Invalid file type.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var mediaFile = new MediaFile
                {
                    FileName = file.FileName,
                    FileType = fileType,
                    FileData = memoryStream.ToArray(),
                    UploadDate = DateTime.UtcNow
                };

                _context.MediaFiles.Add(mediaFile);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMediaFile), new { id = mediaFile.Id }, mediaFile);
            }
        }

        private string GetMimeTypeFromFileName(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(fileName, out var mimeType))
            {
                return mimeType;
            }

            return "application/octet-stream"; // Default fallback
        }



        // DELETE: api/Media/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaFile(int id)
        {
            var mediaFile = await _context.MediaFiles.FindAsync(id);
            if (mediaFile == null) return NotFound();

            _context.MediaFiles.Remove(mediaFile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
