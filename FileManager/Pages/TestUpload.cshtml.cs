using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using System.Linq;

namespace FileManager.Pages
{
    public class TestUploadModel : PageModel
    {
        private readonly ILogger<TestUploadModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public TestUploadModel(ILogger<TestUploadModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = Environment.GetEnvironmentVariable("StorageAccountName");
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

                var containerName = "test-container";

                var blobServiceClient = new BlobServiceClient(new Uri($"https://{_connectionString}.blob.core.windows.net/"), new DefaultAzureCredential());

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

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            try
            {
                Console.WriteLine("Getting blob container");
                var blobServiceClient = new BlobServiceClient(new Uri($"https://{_connectionString}.blob.core.windows.net/"), new DefaultAzureCredential());
                var containerName = "test-container";
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                Console.WriteLine("Getting blobs");
                var blobs = blobContainerClient.GetBlobsAsync();

                List<BlobItem> blobItems = new List<BlobItem>();
                await foreach (var blobItem in blobs)
                {
                    blobItems.Add(blobItem);
                }

                var mostRecentBlob = blobItems.OrderByDescending(b => b.Properties.LastModified).FirstOrDefault();

                if (mostRecentBlob != null)
                {
                    // Do something with the most recently uploaded file
                    Console.WriteLine($"Most recently uploaded file: {mostRecentBlob.Name}");

                    var blobClient = blobContainerClient.GetBlobClient(mostRecentBlob.Name);
                    var stream = new MemoryStream();
                    await blobClient.DownloadToAsync(stream);
                    stream.Position = 0; // Reset stream position to the beginning

                    // Return the file to download
                    return File(stream, "application/octet-stream", mostRecentBlob.Name);
                }
                else
                {
                    Console.WriteLine("No files found in the container");
                }

                return RedirectToPage("TestUpload");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving the most recently uploaded file");
                // Handle the exception or rethrow it
                return RedirectToPage("Privacy");
            }
        }
    }
}
