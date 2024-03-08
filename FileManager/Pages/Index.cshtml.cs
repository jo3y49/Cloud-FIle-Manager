using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileManager.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UserService _userService;

    public string Username { get; set; }

    public IndexModel(ILogger<IndexModel> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;

        Username = string.Empty;
    }

    public void OnGet()
    {
        Username = _userService.GetUserID();
    }
}
