param name string
param location string

resource storageAccount 'Microsoft.Storage/storageAccounts@2026-06-01' = {
  name: name
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
    properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'

output storageAccountId string = storageAccount.id
output storageAccountName string = storageAccount.name
output storageConnectionString string = storageConnectionString
