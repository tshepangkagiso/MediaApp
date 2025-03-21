using MediaAppWebApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediaAppWebApp.Pages
{
    public class MediaListModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public MediaListModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient();
        }

        public List<MediaFileViewModel> MediaFiles { get; set; } = new List<MediaFileViewModel>();

        public async Task OnGetAsync()
        {
            try
            {
                var mediaFiles = await _httpClient.GetFromJsonAsync<List<MediaFile>>("https://localhost:7065/api/media");
                if (mediaFiles != null)
                {
                    foreach (var media in mediaFiles)
                    {
                        string base64Data = null;

                        // If it's an image, retrieve the base64 data
                        if (MimeTypes.IsImage(media.FileType))
                        {
                            var base64Response = await _httpClient.GetFromJsonAsync<MediaBase64Response>(
                                $"https://localhost:7065/api/media/base64/{media.Id}"
                            );

                            if (base64Response != null)
                            {
                                base64Data = base64Response.Base64Data;
                                Console.WriteLine($"Retrieved Base64 for {media.FileName}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to retrieve Base64 for {media.FileName}");
                            }
                        }

                        // Add to the list
                        MediaFiles.Add(new MediaFileViewModel
                        {
                            Id = media.Id,
                            FileName = media.FileName,
                            FileType = media.FileType,
                            UploadDate = media.UploadDate,
                            Base64Data = base64Data
                        });

                        Console.WriteLine($"File Loaded: {media.FileName} ({media.FileType})");
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error loading media files: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error loading media files: {ex.Message}");
            }
        }

    }

    public class MediaFileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
        public string Base64Data { get; set; }
    }

    public class MediaFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileData { get; set; } // Store file data as bytes
        public DateTime UploadDate { get; set; }
    }

    public class MediaBase64Response
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Base64Data { get; set; }
    }

}
