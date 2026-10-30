param name string
param location string
param keyVaultName string

resource redisCluster 'Microsoft.Cache/redisEnterprise@2025-07-01' = {
  name: name
  location: location
  sku: {
    name: 'Balanced_B0'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    minimumTlsVersion: '1.2'
  }
}

resource redisDatabase 'Microsoft.Cache/redisEnterprise/databases@2025-07-01' = {
  parent: redisCluster
  name: 'default' // must be 'default'
  properties: {
    clientProtocol: 'Encrypted'
    port: 10000
    clusteringPolicy: 'NoCluster'   // or 'EnterpriseCluster' for a single endpoint
    evictionPolicy: 'VolatileLRU'    // use 'NoEviction' if you don't want keys evicted
    accessKeysAuthentication: 'Enabled' // Entra ID only, equivalent to 'aad-enabled'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2026-05-15' existing = {
  name: keyVaultName
}

resource redisCacheConnectionString 'Microsoft.KeyVault/vaults/secrets@2026-05-15' = {
  parent: keyVault
  name: 'Redis--ConnectionString'
  properties: {
    value: '${redisCluster.properties.hostName}:10000,password=${redisDatabase.listKeys().primaryKey},ssl=True,abortConnect=False'
  }
}

output id string = redisCluster.id
output hostName string = redisCluster.properties.hostName
