namespace MediaAppWebApi.Data.Model
{
    public static class MimeTypes
    {
        public static readonly string[] ImageMimeTypes =
        {
        "image/jpeg", "image/png", "image/gif", "image/bmp",
        "image/webp", "image/svg+xml", "image/tiff", "image/x-icon"
        };

        public static readonly string[] VideoMimeTypes =
        {
        "video/mp4", "video/webm", "video/ogg",
        "video/quicktime", "video/x-msvideo", "video/x-ms-wmv",
        "video/x-matroska","video/mp3"
        };

        public static readonly string[] AudioMimeTypes =
        {
        "audio/mpeg", "audio/wav", "audio/ogg",
        "audio/aac", "audio/webm", "audio/x-wav","audio/mp4","audio/mp3"
        };

        public static readonly string[] DocumentMimeTypes =
        {
        "application/pdf", "application/zip",
        "application/x-zip-compressed", "application/vnd.ms-excel"
        };

        public static bool IsImage(string mimeType) =>
            ImageMimeTypes.Contains(mimeType);

        public static bool IsVideo(string mimeType) =>
            VideoMimeTypes.Contains(mimeType);

        public static bool IsAudio(string mimeType) =>
            AudioMimeTypes.Contains(mimeType);

        public static bool IsDocument(string mimeType) =>
            DocumentMimeTypes.Contains(mimeType);
    }
}
