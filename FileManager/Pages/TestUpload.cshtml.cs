using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;
using System.Diagnostics;

namespace FileManager.Pages
{
    public class TestUploadModel : PageModel
    {
        private readonly ILogger<TestUploadModel> _logger;
        private readonly IConfiguration _configuration;

        public TestUploadModel(ILogger<TestUploadModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostUploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogInformation("File not found");
                return RedirectToPage("Index");
            }

            var connectionString = _configuration["ConnectionStrings:AzureBlobStorageConnectionString"];
            var containerName = "test-container";
            
            _logger.LogInformation("Setting up blob service client");
            var blobServiceClient = new BlobServiceClient(connectionString);

            _logger.LogInformation("Getting blob container");
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            _logger.LogInformation("Uploading file to blob storage");
            var blobClient = blobContainerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }
            
            _logger.LogInformation("File found");
            return RedirectToPage("UploadSuccess");
        }
    }
}
