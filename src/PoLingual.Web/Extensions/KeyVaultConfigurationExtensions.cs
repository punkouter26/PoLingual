using Azure.Identity;

namespace PoLingual.Web.Extensions;

/// <summary>
/// Adds Azure Key Vault as a configuration source using DefaultAzureCredential.
/// Fails gracefully if Key Vault is not accessible.
/// </summary>
public static class KeyVaultConfigurationExtensions
{
    public static IConfigurationBuilder AddPoLingualKeyVault(this IConfigurationBuilder builder, IConfiguration currentConfig)
    {
        var keyVaultName = currentConfig["Azure:KeyVaultName"];
        if (!string.IsNullOrWhiteSpace(keyVaultName))
        {
            try
            {
                var kvUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
                builder.AddAzureKeyVault(kvUri, new DefaultAzureCredential());
            }
            catch (Exception ex)
            {
                // Log but don't crash — app continues with local/env config
                Console.WriteLine($"[WARN] Key Vault '{keyVaultName}' not accessible: {ex.Message}");
            }
        }
        return builder;
    }
}
