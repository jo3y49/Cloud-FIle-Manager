using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;

namespace FileManager.Pages
{
    public class FileUploadModel : PageModel
    {
        private readonly ILogger<FileUploadModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly UserService _userService;

        public float MaxStorage { get; private set; }
        public float UsedStorage { get; private set; }

        public FileUploadModel(ILogger<FileUploadModel> logger, IConfiguration configuration, BlobServiceClient blobServiceClient, UserService userService)
        {
            _logger = logger;
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;
            _userService = userService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            long max = await _userService.GetUserMaxStorageAsync();
            long used = await _userService.GetUserUsedStorageAsync();

            MaxStorage = FileChecker.ConvertBytesToMegabytes(max);
            UsedStorage = FileChecker.ConvertBytesToMegabytes(used);

            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            try
            {
                var containerName = _userService.GetContainerName();

                Console.WriteLine("Getting blob container");
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await blobContainerClient.CreateIfNotExistsAsync();

                foreach (var file in files)
                {
                    float fileLength = FileChecker.ConvertBytesToMegabytes(file.Length);

                    Console.WriteLine($"File length: {fileLength}");

                    if (file == null || fileLength == 0)
                    {
                        Console.WriteLine("File not found");
                        continue;
                    }

                    if (fileLength < MaxStorage - UsedStorage)
                    {
                        Console.WriteLine("File too large");
                        continue;
                    }

                    Console.WriteLine("Uploading file to blob storage");
                    var blobClient = blobContainerClient.GetBlobClient(file.FileName);

                    using var stream = file.OpenReadStream();

                    await blobClient.UploadAsync(stream, true);

                    await _userService.UpdateUserUsedStorageAsync(file.Length);
                    UsedStorage += fileLength;
                }

                Console.WriteLine("File found");
                return RedirectToPage();
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while uploading the file");
                // Handle the exception or rethrow it
                return RedirectToPage();
            }
        }
    }
}
