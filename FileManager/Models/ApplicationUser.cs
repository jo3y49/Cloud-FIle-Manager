using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public long MaxStorage { get; set; } = 104857600; // Default 100MB
    public long UsedStorage { get; set; } = 0;
}
