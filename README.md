# URL-shortener
Let's Build It: Url Shortener Course


## Infrastructure as Code

### Download Azure CLI
https://learn.microsoft.com/en-us/cli/azure/

### Log in into Azure
```bash
az login
```

### Create Resource Group

```bash
az group create --name url-shortener-dev-rg --location eastasia
```

### Deploy Bicep

### What if
```bash
az deployment group what-if --resource-group url-shortener-dev-rg --template-file infrastructure/main.bicep
```

### Deploy
```bash
az deployment group create --resource-group url-shortener-dev-rg --template-file infrastructure/main.bicep
```

### Create User for GH Actions

```bash
az ad sp create-for-rbac --name "GitHub-Actions-SP" \
                         --role contributor \
                         --scopes /subscriptions/9fa5afcc-60f3-4e03-beff-f5f5b18b5a1e \
                         --sdk-auth
```

### Apply to Custom Contributor Role

```bash
az ad sp create-for-rbac --name "GitHub-Actions-SP" --role 'infra_deploy' --scopes /subscriptions/9fa5afcc-60f3-4e03-beff-f5f5b18b5a1e --sdk-auth
```

#### Configure a federated identity credential on an app

https://learn.microsoft.com/en-gb/entra/workload-id/workload-identity-federation-create-trust?pivots=identity-wif-apps-methods-azp#configure-a-federated-identity-credential-on-an-app

## Get Azure Publish Profile

```bash
az webapp deployment list-publishing-profiles --name token-range-service-ujpsvnz4bdlkk --resource-group url-shortener-dev-rg --xml
```


## Get Static Web Apps Deployment Token

```bash
az staticwebapp secrets list --name web-app-ujpsvnz4bdlkk --query "properties.apiKey"
```

# Utilities

- Base62 converter: https://math.tools/calculator/base/10-62