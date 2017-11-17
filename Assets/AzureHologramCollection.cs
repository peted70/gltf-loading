//#if !UNITY_EDITOR
using Microsoft.WindowsAzure.Storage;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;

public class AzureHologramCollection : IHologramCollection
{
    private CloudBlobClient _serviceClient;
    private CloudBlobContainer _container;

    public AzureHologramCollection()
    {
        var accountName = "gltfmodels";// Environment.GetEnvironmentVariable("HOLOGRAMCOLLECTION_ACCOUNTNAME");
        var accountKey = "CRojB54EQeb+nEVcXIpp4mrAQ9iU+OY0u7QqOAxA3RO4rehyX9SWv3a8GrFd6at+fDD4y3o4cgO5fM330JyTSQ==";// Environment.GetEnvironmentVariable("HOLOGRAMCOLLECTION_ACCOUNTKEY");

        if (string.IsNullOrEmpty(accountKey) || string.IsNullOrEmpty(accountName))
        {
            throw new Exception("You must set HOLOGRAMCOLLECTION_ACCOUNTNAME & HOLOGRAMCOLLECTION_ACCOUNTKEY" + 
                                "environment variables - these can be found at http://portal.azure.com");
        }

#if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = (s, c, ch, pe) => true;
#endif

        string connectionString = 
            string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net",
            accountName, accountKey);
        var account = CloudStorageAccount.Parse(connectionString);
        _serviceClient = account.CreateCloudBlobClient();
        _container = _serviceClient.GetContainerReference("models");
    }

    public async Task<IEnumerable<IHologram>> GetHologramsAsync()
    {
        var results = await _container.ListBlobsSegmentedAsync(null);
        var list = results.Results;
        return list.Select(item =>
            {
                var blob = (CloudBlockBlob)item;
                return new Hologram
                {
                    Name = blob.Name,
                    Uri = blob.Uri,
                };
            });
    }
}
//#endif