using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileManager.Pages
{
    public class TestUploadModel : PageModel
    {
        private readonly ILogger<TestUploadModel> _logger;

        public TestUploadModel(ILogger<TestUploadModel> logger)
        {
            _logger = logger;
        }
    }
}
