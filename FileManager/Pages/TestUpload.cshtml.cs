using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;

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
                return RedirectToPage("UploadError");
            }

            var connectionString = _configuration["ConnectionStrings:AzureBlobStorageConnectionString"];
            var containerName = "test-container";
            
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return RedirectToPage("UploadSuccess");
        }
    }
}
