param name string
param location string
param keyVaultName string
param subnetId string
param vnetId string

resource redisCluster 'Microsoft.Cache/redisEnterprise@2025-07-01' = {
  name: name
  location: location
  sku: {
    name: 'Balanced_B0'
  }
  properties: {
    publicNetworkAccess: 'Disabled'
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


resource redisCachePrivateEndpoint 'Microsoft.Network/privateEndpoints@2026-01-01' = {
  name: '${name}-privateEndpoint'
  location: location
  properties: {
    subnet: {
      id: subnetId
    }
    privateLinkServiceConnections: [
      {
        name: '${redisCluster.name}-privateendpoint'
        properties: {
          privateLinkServiceId: redisCluster.id
          groupIds: [
            'redisCache'
          ]
        }
      }
    ]
  }
}


var privateDnsZoneName = 'privatelink.redis.cache.windows.net'
resource privateDnsZone 'Microsoft.Network/privateDnsZones@2024-06-01' = {
  name: privateDnsZoneName
  location: 'global'
}

resource privateDnsZoneVNetLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2024-06-01' = {
  parent: privateDnsZone
  name: uniqueString(vnetId)
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: vnetId
    }
  }
}

resource privateDnsZoneARecord 'Microsoft.Network/privateDnsZones/A@2024-06-01' = {
  parent: privateDnsZone
  name: redisCluster.name
  properties: {
    aRecords: [
      {
        ipv4Address: redisCachePrivateEndpoint.properties.customDnsConfigs[0].ipAddresses[0]
      }
    ]
    ttl: 3600
  }
}


output id string = redisCluster.id