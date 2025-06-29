# dometrain-url-shortener
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
az group create --name url-shortener-dev --location westeurope
```

### Deploy Bicep

### What if
```bash
az deployment group what-if --resource-group url-shortener-dev --template-file infrastructure/main.bicep
```

### Deploy
```bash
az deployment group create --resource-group url-shortener-dev --template-file infrastructure/main.bicep
```

