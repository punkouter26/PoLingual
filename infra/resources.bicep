@description('Primary location for all resources')
param location string

@description('Environment name')
param environmentName string

// ── Shared App Service Plan (Linux, Free F1) ─────────────────────────────────
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'asp-polingual-${environmentName}'
  location: location
  kind: 'linux'
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  properties: {
    reserved: true
  }
}

// ── Web App ──────────────────────────────────────────────────────────────────
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'app-polingual-${environmentName}'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn: false
      appSettings: [
        {
          name: 'Azure__KeyVaultName'
          value: 'kv-poshared'
        }
        {
          name: 'Azure__StorageConnectionString'
          value: storageAccount.properties.primaryEndpoints.table
        }
        {
          name: 'Azure__TableStorageConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
        }
      ]
    }
  }
}

// ── Storage Account (Table Storage) ──────────────────────────────────────────
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: 'stpolingual${environmentName}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
  }
}

// ── Table Storage services ───────────────────────────────────────────────────
resource tableServices 'Microsoft.Storage/storageAccounts/tableServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

// ── Application Insights ─────────────────────────────────────────────────────
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'log-polingual-${environmentName}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-polingual-${environmentName}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
