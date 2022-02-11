using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TypedDocumentsHandler.Core;

namespace TypedDocumentsHandler
{
    public class CosmosDbDocumentSaver
    {
        private readonly string _databaseId;
        private readonly string _containerId;
        private readonly ILogger<CosmosDbDocumentSaver> _logger;
        private CosmosDbService _service;

        public CosmosDbDocumentSaver(ILogger<CosmosDbDocumentSaver> log, CosmosDbService service, IConfiguration configuration)
        {
            _logger = log;
            _service = service;
            _databaseId = configuration["AzureCosmosDbDatabaseId"];
            _containerId = configuration["AzureCosmosDbContainerId"];
        }


        [FunctionName("SaveDocumentToCosmosDb")]
        public async Task Run([ServiceBusTrigger("sbt-documents-dev-001", "ScienceDocuments", Connection = "AzureServiceBus", AutoComplete = true)] string serviceBusMessage)
        {
            _logger.LogInformation($"SaveDocumentToCosmosDb function processed message: {serviceBusMessage}");

            bool isMessageValidJson = TryParseMessage(serviceBusMessage, out JObject messageAsJson);
            if (!isMessageValidJson)
                throw new ArgumentException("Incoming message contains invalid Json");

            messageAsJson["id"] = Guid.NewGuid().ToString();

            await _service.LoadDocumentAsync(messageAsJson, _databaseId, _containerId);
        }

        private bool TryParseMessage(string input, out JObject parsedObject)
        {
            try
            {
                parsedObject = JObject.Parse(input);
                return true;
            }
            catch (JsonReaderException e)
            {
                parsedObject = null;
                return false;
            }
        }
    }
}
