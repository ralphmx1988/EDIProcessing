# Azure Resources Configuration

## Required Azure Resources

### 1. Resource Group
```bash
az group create --name rg-edi-processing --location eastus
```

### 2. Azure SQL Database
```bash
# Create SQL Server
az sql server create \
  --name sql-edi-processing \
  --resource-group rg-edi-processing \
  --location eastus \
  --admin-user ediadmin \
  --admin-password "YourSecurePassword123!"

# Create Database
az sql db create \
  --resource-group rg-edi-processing \
  --server sql-edi-processing \
  --name EDIProcessingDB \
  --service-objective Basic
```

### 3. Azure Storage Account
```bash
az storage account create \
  --name stediprocessing \
  --resource-group rg-edi-processing \
  --location eastus \
  --sku Standard_LRS \
  --kind StorageV2
```

### 4. Azure Service Bus (Optional)
```bash
az servicebus namespace create \
  --name sb-edi-processing \
  --resource-group rg-edi-processing \
  --location eastus \
  --sku Standard
```

### 5. Azure App Service Plan
```bash
az appservice plan create \
  --name asp-edi-processing \
  --resource-group rg-edi-processing \
  --location eastus \
  --sku B1 \
  --is-linux false
```

### 6. Azure Web App (API)
```bash
az webapp create \
  --name app-edi-processing-api \
  --resource-group rg-edi-processing \
  --plan asp-edi-processing \
  --runtime "DOTNET|8.0"
```

### 7. Azure Function App
```bash
az functionapp create \
  --name func-edi-processing \
  --resource-group rg-edi-processing \
  --consumption-plan-location eastus \
  --runtime dotnet-isolated \
  --runtime-version 8 \
  --functions-version 4 \
  --storage-account stediprocessing
```

## Configuration

### Environment Variables for Web App
```bash
az webapp config appsettings set \
  --resource-group rg-edi-processing \
  --name app-edi-processing-api \
  --settings \
    ConnectionStrings__DefaultConnection="Server=sql-edi-processing.database.windows.net;Database=EDIProcessingDB;User Id=ediadmin;Password=YourSecurePassword123!;Encrypt=True;" \
    ConnectionStrings__AzureStorage="DefaultEndpointsProtocol=https;AccountName=stediprocessing;AccountKey=<storage-key>;EndpointSuffix=core.windows.net"
```

### Environment Variables for Function App
```bash
az functionapp config appsettings set \
  --resource-group rg-edi-processing \
  --name func-edi-processing \
  --settings \
    ConnectionStrings__DefaultConnection="Server=sql-edi-processing.database.windows.net;Database=EDIProcessingDB;User Id=ediadmin;Password=YourSecurePassword123!;Encrypt=True;" \
    ConnectionStrings__AzureStorage="DefaultEndpointsProtocol=https;AccountName=stediprocessing;AccountKey=<storage-key>;EndpointSuffix=core.windows.net"
```

## Deployment Commands

### Deploy API
```bash
cd src/EDIProcessingApp.API
dotnet publish -c Release -o ./publish
az webapp deployment source config-zip \
  --resource-group rg-edi-processing \
  --name app-edi-processing-api \
  --src ./publish.zip
```

### Deploy Functions
```bash
cd src/EDIProcessingApp.Functions
func azure functionapp publish func-edi-processing
```
