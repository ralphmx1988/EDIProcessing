# EDI Processing Application

A comprehensive .NET 8 EDI (Electronic Data Interchange) processing application built with Azure cloud services. This application handles EDI file uploads via API and SFTP, validates files, processes transactions, and manages consumer accounts with their configurations.

> **Note**: This application has been migrated to .NET 8 for compatibility with Azure Functions v4. Azure Functions currently supports .NET 8 as the latest LTS version.

## Architecture

The application follows a clean architecture pattern with the following projects:

- **EDIProcessingApp.Core**: Domain entities, interfaces, and enums
- **EDIProcessingApp.Infrastructure**: Data access, Azure services implementation
- **EDIProcessingApp.API**: REST API for file upload and management
- **EDIProcessingApp.Functions**: Azure Functions for background processing

## Features

### Core Features
- ✅ EDI file upload via REST API
- ✅ SFTP polling for automatic file retrieval
- ✅ File validation (X12, EDIFACT formats)
- ✅ Transaction parsing and processing
- ✅ Account management with configurations
- ✅ Azure Blob Storage integration
- ✅ Background processing with Azure Functions

### Database Schema
The application uses the following main entities:
- **Accounts**: Consumer entities with account management
- **AccountConfigurations**: Key-value configuration pairs for each account
- **EdiFiles**: Uploaded EDI files with metadata and processing status
- **Transactions**: Parsed EDI transactions with JSON data storage

### Azure Services Integration
- **Azure SQL Database**: Primary data storage
- **Azure Blob Storage**: EDI file storage
- **Azure Functions**: Background processing and SFTP polling
- **Azure Service Bus**: Message queuing (future enhancement)

## Prerequisites

- .NET 9 SDK
- Visual Studio 2022 or Visual Studio Code
- Azure subscription
- SQL Server (LocalDB for development)
- Azure Storage Emulator (for development)

## Getting Started

### 1. Clone and Setup
```bash
git clone <repository-url>
cd EDIProcessing
```

### 2. Database Setup
The application uses Entity Framework Core with SQL Server. The database will be created automatically on first run.

Connection string (development):
```
Server=(localdb)\\mssqllocaldb;Database=EDIProcessingDB;Trusted_Connection=true;MultipleActiveResultSets=true
```

### 3. Azure Storage Setup
For development, use the Azure Storage Emulator:
```bash
# Start Azure Storage Emulator
AzureStorageEmulator.exe start
```

### 4. Configuration
Update the following configuration files:

**API Project** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-connection-string",
    "AzureStorage": "your-azure-storage-connection-string"
  }
}
```

**Azure Functions** (`local.settings.json`):
```json
{
  "Values": {
    "AzureWebJobsStorage": "your-azure-storage-connection-string",
    "ConnectionStrings:DefaultConnection": "your-sql-connection-string"
  }
}
```

### 5. Run the Application

#### API Project
```bash
cd src/EDIProcessingApp.API
dotnet run
```
The API will be available at `https://localhost:7001` with Swagger UI.

#### Azure Functions
```bash
cd src/EDIProcessingApp.Functions
func start
```

## API Endpoints

### Accounts Management
- `GET /api/accounts` - Get all accounts
- `GET /api/accounts/{id}` - Get account by ID
- `GET /api/accounts/code/{code}` - Get account by code
- `POST /api/accounts` - Create new account
- `PUT /api/accounts/{id}` - Update account
- `GET /api/accounts/{id}/configurations` - Get account configurations
- `POST /api/accounts/{id}/configurations` - Set account configuration

### EDI Files Management
- `POST /api/edifiles/upload` - Upload EDI file
- `GET /api/edifiles` - Get all EDI files (with filtering)
- `GET /api/edifiles/{id}` - Get EDI file by ID
- `POST /api/edifiles/{id}/validate` - Validate EDI file
- `POST /api/edifiles/{id}/process` - Process EDI file
- `GET /api/edifiles/{id}/transactions` - Get file transactions
- `GET /api/edifiles/pending` - Get pending files

### Supported EDI Transaction Types

The application supports the following standard EDI transaction types:

#### X12 Transaction Sets
- **850** - Purchase Order (Order request)
- **810** - Invoice (Billing information)
- **856** - Advance Ship Notice (Shipping notification)
- **855** - Purchase Order Acknowledgement (PO confirmation)
- **820** - Payment Order/Remittance (Payment/remittance information)
- **862** - Shipping Schedule (Delivery schedule)
- **997** - Functional Acknowledgement (Receipt confirmation of EDI message)

#### EDIFACT Message Types
- **ORDERS** - Purchase Order (Order request)
- **INVOIC** - Invoice (Billing information)
- **DESADV** - Advance Ship Notice (Shipping notification)
- **ORDRSP** - Purchase Order Acknowledgement (PO confirmation)
- **PAYMUL/REMADV** - Payment Order/Remittance (Payment/remittance information)
- **DELFOR** - Shipping Schedule (Delivery schedule)
- **CONTRL** - Functional Acknowledgement (Receipt confirmation of EDI message)

### Transaction Types Management
- `GET /api/transactiontypes` - Get all supported transaction types
- `GET /api/transactiontypes/x12` - Get supported X12 codes
- `GET /api/transactiontypes/edifact` - Get supported EDIFACT codes
- `GET /api/transactiontypes/lookup/{code}` - Get transaction info by code
- `GET /api/transactiontypes/validate/{code}` - Validate transaction type
- `GET /api/transactiontypes/map/x12-to-edifact/{x12Code}` - Map X12 to EDIFACT
- `GET /api/transactiontypes/map/edifact-to-x12/{edifactName}` - Map EDIFACT to X12

### Transactions Management
- `GET /api/transactions` - Get all transactions (with filtering)
- `GET /api/transactions/{id}` - Get transaction by ID
- `POST /api/transactions/{id}/acknowledge` - Send acknowledgment
- `GET /api/transactions/by-partner/{partnerId}` - Get transactions by partner

## Account Configuration

Each account can have specific configurations for:

### SFTP Settings
- `SFTP.HostName`: SFTP server hostname
- `SFTP.UserName`: SFTP username
- `SFTP.Password`: SFTP password (encrypted)
- `SFTP.IncomingPath`: Path for incoming files
- `SFTP.ProcessedPath`: Path for processed files

### EDI Settings
- `EDI.FileFormat`: Supported format (X12, EDIFACT)
- `EDI.TransactionTypes`: Supported transaction types
- `EDI.ValidationRules`: Custom validation rules

### API Settings
- `API.EndpointUrl`: API endpoint for callbacks
- `API.ApiKey`: API key for authentication

## Background Processing

### Azure Functions
1. **ProcessPendingFiles**: Runs every 5 minutes to process pending EDI files
2. **PollSftpServers**: Runs every 10 minutes to poll SFTP servers for new files
3. **ProcessBlobTrigger**: Triggered when files are uploaded to blob storage

## Development

### Project Structure
```
EDIProcessing/
├── src/
│   ├── EDIProcessingApp.Core/          # Domain layer
│   │   ├── Entities/                   # Domain entities
│   │   ├── Interfaces/                 # Service interfaces
│   │   └── Enums/                      # Enumerations
│   ├── EDIProcessingApp.Infrastructure/ # Infrastructure layer
│   │   ├── Data/                       # EF configurations
│   │   ├── Repositories/               # Data repositories
│   │   └── Services/                   # Service implementations
│   ├── EDIProcessingApp.API/           # Web API layer
│   │   └── Controllers/                # API controllers
│   └── EDIProcessingApp.Functions/     # Azure Functions
└── EDIProcessingApp.sln                # Solution file
```

### Building and Testing
```bash
# Build the solution
dotnet build

# Run tests (when added)
dotnet test

# Restore packages
dotnet restore
```

## Deployment

### Azure Resources Required
1. **Azure SQL Database**
2. **Azure Storage Account** (for blob storage)
3. **Azure Function App**
4. **Azure App Service** (for API)

### Deployment Steps
1. Create Azure resources
2. Update connection strings in Azure portal
3. Deploy API to App Service
4. Deploy Functions to Function App
5. Run database migrations

## Security Considerations

- Sensitive configurations are encrypted
- API authentication should be implemented
- SFTP credentials are stored securely
- File access is controlled through Azure Storage permissions

## Monitoring and Logging

- Application Insights integration
- Structured logging with Serilog (recommended)
- Health checks for API endpoints
- Function monitoring through Azure portal

## Contributing

1. Follow the existing code patterns
2. Add appropriate logging
3. Include error handling
4. Update documentation
5. Add unit tests for new features

## Support

For issues and questions, please refer to the project documentation or create an issue in the repository.
