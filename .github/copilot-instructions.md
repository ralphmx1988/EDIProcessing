<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# EDI Processing Application

This is a .NET 9 EDI (Electronic Data Interchange) processing application built with Azure cloud services.

## Architecture

- **Core**: Domain entities, interfaces, and enums
- **Infrastructure**: Data access, Azure services implementation, and repositories
- **API**: REST API for file upload and management
- **Functions**: Azure Functions for background processing and SFTP polling

## Key Technologies

- .NET 9
- Entity Framework Core 9
- Azure SQL Database
- Azure Blob Storage
- Azure Functions
- Azure Service Bus
- SFTP integration with SSH.NET

## Domain Models

- **Account**: Consumer entities with configurations
- **AccountConfiguration**: Key-value configuration pairs for each account
- **EdiFile**: Uploaded EDI files with metadata
- **Transaction**: Parsed EDI transactions

## Key Features

- EDI file upload via API
- SFTP polling for automatic file retrieval
- File validation and parsing
- Account-specific configurations
- Azure cloud integration
- Background processing with Azure Functions

## Development Guidelines

- Use async/await patterns consistently
- Implement proper error handling and logging
- Follow repository pattern for data access
- Use dependency injection for service registration
- Implement proper validation for API endpoints
- Use Azure services for scalability and reliability
