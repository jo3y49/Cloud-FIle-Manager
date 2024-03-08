using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileManager.Pages
{
    public class FileDisplayModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ISasTokenService _sasTokenService;
        private readonly UserService _userService;
        private readonly string _containerName;

        public List<BlobFileInfo> BlobsInfo = [];
        public FileDisplayModel(BlobServiceClient blobServiceClient, ISasTokenService sasTokenService, UserService userService)
        {
            _blobServiceClient = blobServiceClient;
            _sasTokenService = sasTokenService;
            _userService = userService;
            _containerName = _userService.GetContainerName();
        }

        public async Task OnGetAsync()
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
            {
                var fileType = Path.GetExtension(blobItem.Name).ToLower();

                // Use the BlobClient to get the proper URL
                var sasToken = _sasTokenService.GetBlobSasUri(_containerName, blobItem.Name);
                var blobUrl = sasToken;

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

                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                
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

        public async Task<IActionResult> OnPostDeleteAsync(string filename)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                {
                    return Content("Filename not specified.");
                }

                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                
                var blobClient = blobContainerClient.GetBlobClient(filename);

                await _userService.UpdateUserUsedStorageAsync(-blobClient.GetProperties().Value.ContentLength);
                
                await blobClient.DeleteIfExistsAsync();
                return RedirectToPage();
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while deleting the file");
                // Handle the exception or rethrow it
                return RedirectToPage("Privacy");
            }
        }

        public async Task<IActionResult> OnPostDeleteAllAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                try
                {
                    await blobClient.DeleteIfExistsAsync();
                }
                catch (Exception ex)
                {
                    // Log the error (consider using ILogger or a similar logging mechanism)
                    Console.WriteLine($"Failed to delete {blobItem.Name}: {ex.Message}");
                }
            }

            await _userService.EmptyUserStorageAsync();

            // Redirect to the same page to refresh the file list
            return RedirectToPage();
        }

        
    }
}
