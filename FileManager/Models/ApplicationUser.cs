using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationUser : IdentityUser
{
    public long MaxStorage { get; set; } = 5368709120; // Default 5GB
    public long UsedStorage { get; set; } = 0;
}
