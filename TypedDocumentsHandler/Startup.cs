using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TypedDocumentsHandler.Core;

[assembly: FunctionsStartup(typeof(TypedDocumentsHandler.Startup))]

namespace TypedDocumentsHandler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<CosmosDbService>();
            builder.Services.AddSingleton<BlobStorageLoader>();
        }
    }
}
