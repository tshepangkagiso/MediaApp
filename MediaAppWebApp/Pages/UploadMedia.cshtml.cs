using MediaAppWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace MediaAppWebApp.Pages
{
    public class UploadMediaModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UploadMediaModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient();
        }

        [BindProperty]
        public IFormFile File { get; set; }

        public bool UploadSuccess { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (File == null || File.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return Page();
            }

            using (var memoryStream = new MemoryStream())
            {
                // Read the file into a memory stream
                await File.CopyToAsync(memoryStream);
                var fileData = memoryStream.ToArray();

                // Compress the file data
                var compressedData = CompressionHelper.Compress(fileData);

                // Create the HTTP content with the compressed data
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(compressedData);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(File.ContentType);
                    content.Add(fileContent, "file", File.FileName);

                    // Send the compressed data to the server
                    var response = await _httpClient.PostAsync("https://localhost:7065/api/media", content);

                    if (response.IsSuccessStatusCode)
                        UploadSuccess = true;
                    else
                        ModelState.AddModelError("File", "Error uploading file.");
                }
            }

            return Page();
        }
    }

}
