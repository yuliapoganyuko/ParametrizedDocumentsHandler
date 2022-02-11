using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TypedDocumentsHandler.Core;

namespace TypedDocumentsHandler
{
    public class BlobStorageDocumentSaver
    {
        private readonly string _containerName = "documents";
        private readonly ILogger<BlobStorageDocumentSaver> _logger;
        private BlobStorageLoader _loader;

        public BlobStorageDocumentSaver(ILogger<BlobStorageDocumentSaver> log, BlobStorageLoader loader)
        {
            _logger = log;
            _loader = loader;
        }

        [FunctionName("SaveDocumentToBlobStorage")]
        public async Task Run([ServiceBusTrigger("sbt-documents-dev-001", "NonScienceDocuments", Connection = "AzureServiceBus", AutoComplete = true)]
            Message serviceBusMessage)
        {
            _logger.LogInformation($"SaveDocumentToBlobStorage function processed message: {serviceBusMessage.MessageId}");

            using (var messageStream = new MemoryStream(serviceBusMessage.Body))
            {
                await _loader.LoadDocumentAsync(_containerName, serviceBusMessage.MessageId, messageStream);
            }
        }
    }
}
