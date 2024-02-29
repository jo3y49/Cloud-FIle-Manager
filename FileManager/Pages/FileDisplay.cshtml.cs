using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileManager.Pages
{
    public class FileDisplayModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ISasTokenService _sasTokenService;
        public List<BlobFileInfo> BlobsInfo = [];
        public FileDisplayModel(BlobServiceClient blobServiceClient, ISasTokenService sasTokenService)
        {
            _blobServiceClient = blobServiceClient;
            _sasTokenService = sasTokenService;
        }

        public async Task OnGetAsync()
        {
            var containerName = "test-container"; // Specify your container name
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
            {
                var fileType = Path.GetExtension(blobItem.Name).ToLower();

                // Use the BlobClient to get the proper URL
                var sasToken = _sasTokenService.GetBlobSasUri(containerName, blobItem.Name);
                var blobUrl = CheckIfImage(fileType) ? $"{sasToken}" : string.Empty;

                BlobsInfo.Add(new BlobFileInfo { Name = blobItem.Name, FileType = fileType, Url = blobUrl });
            }
        }

        public async Task<IActionResult> OnGetDownloadAsync(string filename)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                {
                    return Content("Filename not specified.");
                }

                var containerName = "test-container";
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                
                var blobClient = blobContainerClient.GetBlobClient(filename);
                var stream = new MemoryStream();
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;

                return File(stream, "application/octet-stream", Path.GetFileName(filename));
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while retrieving the most recently uploaded file");
                // Handle the exception or rethrow it
                return RedirectToPage("Privacy");
            }
        }

        public async Task<IActionResult> OnGetDeleteAsync(string filename)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                {
                    return Content("Filename not specified.");
                }

                var containerName = "test-container";
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                
                var blobClient = blobContainerClient.GetBlobClient(filename);
                await blobClient.DeleteIfExistsAsync();

                return RedirectToPage("FileDisplay");
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while deleting the file");
                // Handle the exception or rethrow it
                return RedirectToPage("Privacy");
            }
        }

        private static bool CheckIfImage(string fileType)
        {
            string[] imageTypes = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp"];
            foreach (var type in imageTypes)
            {
                if (fileType.Contains(type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
