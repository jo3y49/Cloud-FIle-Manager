using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

public interface ISasTokenService
{
    string GetBlobSasUri(string containerName, string blobName, string policyName = null);
}

public class SasTokenService : ISasTokenService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly bool _isDevelopment;

    public SasTokenService(BlobServiceClient blobServiceClient, IHostEnvironment env)
    {
        _blobServiceClient = blobServiceClient;
        _isDevelopment = env.IsDevelopment();
    }

    public string GetBlobSasUri(string containerName, string blobName, string policyName = null)
    {
        // Check if we're in development or production
        if (_isDevelopment)
        {
            // Generate a valid SAS token for Azurite
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // Set appropriate expiry time
            };

            // Set permissions for the SAS token
            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

            // The name and key should match your Azurite configuration
            string accountName = "devstoreaccount1"; // Default account name for Azurite
            string accountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="; // Default account key for Azurite

            BlobSasQueryParameters sasQueryParameters = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, accountKey));
            string sasToken = sasQueryParameters.ToString();

            return $"{blobClient.Uri}?{sasToken}";
        }
        else
        {
            // Generate a real SAS token using Azure Blob Storage SDK
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // 1 hour expiry time
            };
            
            // Specify permissions for the SAS token
            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

            // Use a stored access policy if provided, otherwise create an ad hoc SAS
            if (!string.IsNullOrEmpty(policyName))
            {
                sasBuilder.Identifier = policyName;
            }
            else
            {
                sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);
            }

            // Use the key to get the SAS token
            var blobKey = Environment.GetEnvironmentVariable("StorageAccountKey");
            BlobSasQueryParameters sasQueryParameters = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName, blobKey));
            string sasToken = sasQueryParameters.ToString();

            return $"{blobClient.Uri}?{sasToken}";
        }
    }
}
