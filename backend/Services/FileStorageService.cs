using System;
using System.IO;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

#if AZURE
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
#endif

namespace Backend.Services
{
    public class FileStorageService
    {
        private readonly ApplicationSettings settings;
        private readonly ILogger<UsersService> logger;

        public FileStorageService(ApplicationSettings settings, ILogger<UsersService> logger)
        {
            this.logger = logger;
            this.settings = settings;


        }

        // --- Getting ---

        public async Task<Result<FileDownloadResponse>> Download(string folderPath, string fileName)
        {
            return await DownloadInternal(folderPath, fileName);  
        }

        public async Task<Result<FileUploadResponseDTO>> Upload(string folderPath, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new Result<FileUploadResponseDTO>(Status.Failure, "File is empty or null");
            }
            
            if (file.Length > 2097152)
            {
                return new Result<FileUploadResponseDTO>(Status.Failure, "File is too large");
            }

            return await UploadInternal(folderPath, file);
        }

        private async Task<Result<FileDownloadResponse>> DownloadInternal(string folderPath, string fileName) // TODO: No async needed
        {
            Result<FileDownloadResponse> result = new Result<FileDownloadResponse>();

            try 
            {
                string filePath = Path.Combine(settings.FileStorageSettings.DebugFileStoragePath, folderPath, fileName);
                Console.WriteLine(filePath);
                if (File.Exists(filePath)) // TODO: Handle file not found
                {
                    var data = new FileDownloadResponse() 
                    {
                        Uri = filePath,
                        Name = fileName,
                        ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileName),
                        Content = new FileStream(filePath, FileMode.Open, FileAccess.Read),
                    };
                 
                    return new Result<FileDownloadResponse>(Status.Success, "", data);  
                }
                else
                {
                    return new Result<FileDownloadResponse>(Status.Failure, $"File with name {fileName} was not found");    
                }
            }
            catch (Exception ex) // TODO: Add better handling
            {
                logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}"); 
                return new Result<FileDownloadResponse>(Status.Failure, $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.");    
            }
        }

        private async Task<Result<FileUploadResponseDTO>> UploadInternal(string folderPath, IFormFile file) 
        {
            Result<FileUploadResponseDTO> result = new Result<FileUploadResponseDTO>();

            try
            {
                string filePath = Path.Combine(settings.FileStorageSettings.DebugFileStoragePath, folderPath, file.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                await using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var data = new FileUploadResponseDTO()  {
                    Uri = filePath,
                    Name = file.FileName
                };

                return new Result<FileUploadResponseDTO>(Status.Success, "", data);  
            }
            catch (Exception ex)
            {
                logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                return new Result<FileUploadResponseDTO>(Status.Failure, $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.");    
            }
        }
    }
}





