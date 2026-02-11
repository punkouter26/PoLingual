using Azure.Identity;

namespace PoLingual.Web.Extensions;

/// <summary>
/// Adds Azure Key Vault as a configuration source using DefaultAzureCredential.
/// </summary>
public static class KeyVaultConfigurationExtensions
{
    public static IConfigurationBuilder AddPoLingualKeyVault(this IConfigurationBuilder builder, IConfiguration currentConfig)
    {
        var keyVaultName = currentConfig["Azure:KeyVaultName"];
        if (!string.IsNullOrWhiteSpace(keyVaultName))
        {
            var kvUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            builder.AddAzureKeyVault(kvUri, new DefaultAzureCredential());
        }
        return builder;
    }
}
