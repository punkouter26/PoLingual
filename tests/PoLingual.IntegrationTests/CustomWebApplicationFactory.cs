using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoLingual.Web.Services.Data;

namespace PoLingual.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Azure:StorageConnectionString"] = "UseDevelopmentStorage=true",
                ["Azure:KeyVaultName"] = "",
                ["Azure:AzureOpenAIEndpoint"] = "",
                ["Azure:AzureOpenAIDeploymentName"] = "",
                ["Azure:AzureOpenAIApiKey"] = "",
                ["Azure:AzureSpeechSubscriptionKey"] = "",
                ["Azure:AzureSpeechRegion"] = "",
                ["Azure:ApplicationInsightsConnectionString"] = ""
            });
        });
    }
}
