using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(File.OpenReadStream()), "file", File.FileName);
                var response = await _httpClient.PostAsync("https://localhost:7065/api/media", content);

                if (response.IsSuccessStatusCode)
                    UploadSuccess = true;
                else
                    ModelState.AddModelError("File", "Error uploading file.");
            }

            return Page();
        }
    }

}
