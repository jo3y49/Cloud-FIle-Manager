using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FileManager;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectionString = builder.Configuration["sqlDatabaseString"];
    options.UseSqlServer(connectionString);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<DatabaseContext>();

// Register BlobServiceClient for DI
builder.Services.AddSingleton((serviceProvider) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var env = serviceProvider.GetRequiredService<IHostEnvironment>();

    Console.WriteLine("Getting blob container");

    if (env.IsDevelopment())
    {
        Console.WriteLine("Running in development environment");
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
if (!app.Environment.IsDevelopment())
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
