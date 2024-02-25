using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;
using Azure.Identity;

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
            try
            {
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("File not found");
                    return RedirectToPage("Index");
                }

                Console.WriteLine("File found");

                var connection = Environment.GetEnvironmentVariable("StorageAccountName");
                var containerName = "test-container";
                
                Console.WriteLine("Setting up blob service client");
                var blobServiceClient = new BlobServiceClient(new Uri($"https://{connection}.blob.core.windows.net/"), new DefaultAzureCredential());

                Console.WriteLine("Getting blob container");
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await blobContainerClient.CreateIfNotExistsAsync();

                Console.WriteLine("Uploading file to blob storage");
                var blobClient = blobContainerClient.GetBlobClient(file.FileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }
                
                Console.WriteLine("File found");
                return RedirectToPage("TestUpload");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while uploading the file");
                // Handle the exception or rethrow it
                return RedirectToPage("Privacy");
            }
        }
    }
}
