using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypedDocumentsHandler.Core
{
    public  class BlobStorageLoader
    {
        private BlobServiceClient blobServiceClient;

        public BlobStorageLoader(IConfiguration configuration)
        {
            string connectionString = configuration["AzureStorage"];
            blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task LoadDocumentAsync(string blobContainerName, string blobName, Stream blobContent)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            
            await containerClient.UploadBlobAsync(blobName, blobContent);
        }
    }
}
