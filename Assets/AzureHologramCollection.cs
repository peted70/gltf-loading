using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity3dAzure.StorageServices;

public class AzureHologramCollection : IHologramCollection
{
    private StorageServiceClient _storageClient;

    public AzureHologramCollection()
    {
        var account = Environment.GetEnvironmentVariable("");
        var accessKey = Environment.GetEnvironmentVariable("");

        _storageClient = new StorageServiceClient(account, accessKey);
    }

    public Task<IEnumerable<IHologram>> GetHologramsAsync()
    {
        // enumerate the Azure Storage Container..
        var blobs = _storageClient.GetBlobService();
        StartCoroutine( blobs.ListBlobs()

    }
}
