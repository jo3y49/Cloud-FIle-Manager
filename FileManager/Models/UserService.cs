using FileManager;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class UserService
{
    private readonly DatabaseContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserID()
    {
        string? userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return userId ?? string.Empty;
    }

    public string GetContainerName()
    {
        string userId = GetUserID();

        return $"user-{userId}";
    }

    private async Task<ApplicationUser> GetUserAsync()
    {
        string userId = GetUserID();

        var user = await _context.Users
            .FindAsync(userId);

        if (user == null) throw new Exception("User not found.");

        return user;
    }

    public async Task<long> GetUserMaxStorageAsync()
    {
        string userId = GetUserID();

        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.MaxStorage })
            .FirstOrDefaultAsync();

        if (user == null) throw new Exception("User not found.");

        return user.MaxStorage;
    }

    public async Task<long> GetUserUsedStorageAsync()
    {
        string userId = GetUserID();

        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.UsedStorage })
            .FirstOrDefaultAsync();

        if (user == null) throw new Exception("User not found.");

        return user.UsedStorage;
    }

    public async Task UpdateUserUsedStorageAsync(long bytesToAdd)
    {
        var user = await GetUserAsync();

        if (user == null) throw new Exception("User not found.");

        user.UsedStorage += bytesToAdd;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task EmptyUserStorageAsync()
    {
        var user = await GetUserAsync();

        if (user == null) throw new Exception("User not found.");

        user.UsedStorage = 0;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
