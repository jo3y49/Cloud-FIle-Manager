using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using System.Linq;
using Azure.Storage;

namespace FileManager.Pages
{
    public class FileUploadModel : PageModel
    {
        private readonly ILogger<FileUploadModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;

        public FileUploadModel(ILogger<FileUploadModel> logger, IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            try
            {
                var containerName = "test-container";

                Console.WriteLine("Getting blob container");
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await blobContainerClient.CreateIfNotExistsAsync();

                foreach (var file in files)
                {
                    if (file == null || file.Length == 0)
                    {
                        Console.WriteLine("File not found");
                        continue;
                    }

                    Console.WriteLine("Uploading file to blob storage");
                    var blobClient = blobContainerClient.GetBlobClient(file.FileName);

                    using (var stream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                }

                Console.WriteLine("File found");
                return RedirectToPage("FileUpload");
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while uploading the file");
                // Handle the exception or rethrow it
                return RedirectToPage("Privacy");
            }
        }
    }
}
