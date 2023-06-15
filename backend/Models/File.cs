using System.IO;

namespace Backend.Models
{
    public class FileDownloadResponse
    {
        public string Uri { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public Stream Content { get; set; }
    }

    public class FileUploadResponseDTO
    {
        public string Uri { get; set; }

        public string Name { get; set; }
    }
}