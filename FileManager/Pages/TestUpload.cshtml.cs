using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using System.Linq;
using Azure.Storage;

namespace FileManager.Pages
{
    public class TestUploadModel : PageModel
    {
        private readonly ILogger<TestUploadModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;

        public TestUploadModel(ILogger<TestUploadModel> logger, IConfiguration configuration, BlobServiceClient blobServiceClient)
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
                return RedirectToPage("TestUpload");
            }
            catch (Exception)
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
                var containerName = "test-container";
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
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
            catch (Exception)
            {
                Console.WriteLine("An error occurred while retrieving the most recently uploaded file");
                // Handle the exception or rethrow it
                return RedirectToPage("Privacy");
            }
        }
    }
}
