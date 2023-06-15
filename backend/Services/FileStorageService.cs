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

#if AZURE
        private readonly BlobContainerClient azureBlobContainerClient;
#endif 

        public FileStorageService(ApplicationSettings settings, ILogger<UsersService> logger)
        {
            this.logger = logger;
            this.settings = settings;

#if AZURE
            azureBlobContainerClient = new BlobContainerClient(settings.FileStorageSettings.AzureConnectionString, settings.FileStorageSettings.AzureContainerName);
#endif
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

#if AZURE 
        private async Task<BlobDTO> DownloadInternal(string folderPath, string fileName) // TODO: Use path not only the name
        {
            try
            {
                BlobClient client = azureBlobContainerClient.GetBlobClient(fileName);

                // Check if the file exists in the container
                if (await client.ExistsAsync())
                {
                    var blobDTO = new BlobDTO 
                    {
                        Content = await client.OpenReadAsync(),
                        Name = fileName,
                        ContentType = (await client.DownloadContentAsync()).Value.Details.ContentType,
                    };
                    return blobDTO;
                }
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound) // TODO: Add better handling
            {
                logger.LogError($"File {fileName} was not found.");
            }
            catch (RequestFailedException ex)
            {
                logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            }

            return null;
        }

        private async Task<BlobResponseDTO> UploadInternal(string folderPath, IFormFile file) // TODO: Use actual path 
        {
            BlobResponseDTO response = new();

            try
            {
                BlobClient client = azureBlobContainerClient.GetBlobClient(file.FileName); 
                await using (Stream stream = file.OpenReadStream())
                    await client.UploadAsync(stream);

                response.Status = $"File {file.FileName} Uploaded Successfully";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                logger.LogError($"File with name {file.FileName} already exists in container '{settings.FileStorageSettings.AzureContainerName}'");
                response.Status = $"File with name {file.FileName} already exists";
                response.Error = true;
                return response;
            } 
            catch (RequestFailedException ex)
            {
                logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                response.Status = $"Unexpected error: {ex.StackTrace}";
                response.Error = true;
                return response;
            }

            return response;
        }
#else
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
#endif
    }
}





