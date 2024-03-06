using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FileManager;

var builder = WebApplication.CreateBuilder(args);
bool IsDevelopment = builder.Environment.IsDevelopment();

Console.WriteLine($"IsDevelopment: {IsDevelopment}");

var sqlDatabaseString = "";

if (!IsDevelopment)
{
    var keyVaultName = Environment.GetEnvironmentVariable("keyVaultName");
    builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

    sqlDatabaseString = builder.Configuration["sqlDatabaseString"];
}

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(sqlDatabaseString);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<DatabaseContext>();

// Register BlobServiceClient for DI
builder.Services.AddSingleton((serviceProvider) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    Console.WriteLine("Getting blob container");

    if (IsDevelopment)
    {
        // Use Azurite in development
        string azuriteConnectionString = "UseDevelopmentStorage=true"; // Shortcut for Azurite
        return new BlobServiceClient(azuriteConnectionString);
    }
    else
    {
        // Use Azure Blob Storage in production
        var blobName = builder.Configuration["blobName"];
        var blobServiceUri = new Uri($"https://{blobName}.blob.core.windows.net/");
        return new BlobServiceClient(blobServiceUri, new DefaultAzureCredential());
    }
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<ISasTokenService, SasTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!IsDevelopment)
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
