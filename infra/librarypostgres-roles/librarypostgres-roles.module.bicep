@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param librarypostgres_outputs_name string

param principalType string

param principalId string

param principalName string

resource librarypostgres 'Microsoft.DBforPostgreSQL/flexibleServers@2024-08-01' existing = {
  name: librarypostgres_outputs_name
}

resource librarypostgres_admin 'Microsoft.DBforPostgreSQL/flexibleServers/administrators@2024-08-01' = {
  name: principalId
  properties: {
    principalName: principalName
    principalType: principalType
  }
  parent: librarypostgres
}