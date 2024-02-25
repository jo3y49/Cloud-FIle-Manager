using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileManager.Pages
{
    public class FileDisplayModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        public FileDisplayModel(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        
    }
}
