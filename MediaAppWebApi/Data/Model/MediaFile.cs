namespace MediaAppWebApi.Data.Model
{
    public class MediaFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileData { get; set; }
        public DateTime UploadDate { get; set; }
    }

}
