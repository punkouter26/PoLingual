targetScope = 'subscription'

@description('Primary location for all resources')
param location string = 'eastus'

@description('Environment name (dev, staging, prod)')
param environmentName string = 'dev'

@description('Resource group name')
param resourceGroupName string = 'PoLingual'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'polingual-resources'
  params: {
    location: location
    environmentName: environmentName
  }
}

output AZURE_RESOURCE_GROUP string = rg.name
output WEB_APP_NAME string = resources.outputs.webAppName
output WEB_APP_URL string = resources.outputs.webAppUrl
