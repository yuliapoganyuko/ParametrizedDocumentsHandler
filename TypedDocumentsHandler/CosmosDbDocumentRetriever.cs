using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TypedDocumentsHandler.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;

namespace TypedDocumentsHandler
{
    public class CosmosDbDocumentRetriever
    {
        private readonly string _databaseId;
        private readonly string _containerId;
        private readonly ILogger<CosmosDbDocumentRetriever> _logger;
        private CosmosDbService _service;

        public CosmosDbDocumentRetriever(ILogger<CosmosDbDocumentRetriever> log, CosmosDbService service, IConfiguration configuration)
        {
            _logger = log;
            _service = service;
            _databaseId = configuration["AzureCosmosDbDatabaseId"];
            _containerId = configuration["AzureCosmosDbContainerId"];
        }

        [FunctionName("GetDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "document/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("GET function started processing a request.");

            if (string.IsNullOrWhiteSpace(id))
                return new BadRequestObjectResult("Invalid id");

            try
            {
                object document = await _service.GetDocumentAsync(id, id, _databaseId, _containerId);

                if (document == null)
                    return new NotFoundResult();

                return new OkObjectResult(document);

            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return new NotFoundResult();

                log.LogError(ex.Message);
                throw;
            }
        }
    }
}
