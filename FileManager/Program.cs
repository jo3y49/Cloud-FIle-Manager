using Azure.Identity;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

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
        Console.WriteLine("Running in production environment");
        // Use Azure Blob Storage in production
        var blobName = Environment.GetEnvironmentVariable("StorageAccountName");
        var blobServiceUri = new Uri($"https://{blobName}.blob.core.windows.net/");
        return new BlobServiceClient(blobServiceUri, new DefaultAzureCredential());
    }
});

// Add services to the container.
builder.Services.AddRazorPages();

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
