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
    public class CosmosDbDocumentRemover
    {
        private readonly string _databaseId;
        private readonly string _containerId;
        private readonly ILogger<CosmosDbDocumentRemover> _logger;
        private CosmosDbService _service;

        public CosmosDbDocumentRemover(ILogger<CosmosDbDocumentRemover> log, CosmosDbService service, IConfiguration configuration)
        {
            _logger = log;
            _service = service;
            _databaseId = configuration["AzureCosmosDbDatabaseId"];
            _containerId = configuration["AzureCosmosDbContainerId"];
        }

        [FunctionName("DeleteDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "document/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("DELETE function started processing a request.");

            if (string.IsNullOrWhiteSpace(id))
                return new BadRequestObjectResult("Invalid id");

            try
            {
                await _service.DeleteDocumentAsync(id, id, _databaseId, _containerId);
                return new NoContentResult();
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
