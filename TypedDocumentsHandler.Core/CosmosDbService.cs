using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace TypedDocumentsHandler.Core
{
    public class CosmosDbService: IDisposable
    {
        private CosmosClient cosmosClient;

        public CosmosDbService(IConfiguration configuration)
        {
            string connectionString = configuration["AzureCosmosDb"];
            cosmosClient = new CosmosClient(connectionString);
        }

        public async Task LoadDocumentAsync(object document, string databaseId, string containerId)
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container container = database.GetContainer(containerId);

            await container.CreateItemAsync(document);
        }

        public async Task<object?> GetDocumentAsync(string id, string partitionKeyValue, string databaseId, string containerId)
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container container = database.GetContainer(containerId);

            ItemResponse<object?> item = await container.ReadItemAsync<object?>(id, new PartitionKey(partitionKeyValue));

            return item?.Resource;
        }

        public async Task DeleteDocumentAsync(string id, string partitionKeyValue, string databaseId, string containerId)
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container container = database.GetContainer(containerId);

            await container.DeleteItemAsync<object?>(id, new PartitionKey(partitionKeyValue));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                cosmosClient.Dispose();
            }
        }
    }
}