using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EDIProcessingApp.Infrastructure.Data;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(EDIProcessingDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed AccountTypes first if they don't exist
        if (!await context.AccountTypes.AnyAsync())
        {
            var accountTypes = new List<AccountType>
            {
                new AccountType
                {
                    Name = "Customer",
                    Code = "CUST",
                    Description = "Customer accounts that receive goods or services",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new AccountType
                {
                    Name = "Vendor",
                    Code = "VEND",
                    Description = "Vendor/Supplier accounts that provide goods or services",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new AccountType
                {
                    Name = "Partner",
                    Code = "PART",
                    Description = "Business partner accounts for collaborative transactions",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new AccountType
                {
                    Name = "Distributor",
                    Code = "DIST",
                    Description = "Distributor accounts for product distribution networks",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new AccountType
                {
                    Name = "Logistics Provider",
                    Code = "LOGIS",
                    Description = "Third-party logistics and shipping provider accounts",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            };

            await context.AccountTypes.AddRangeAsync(accountTypes);
            await context.SaveChangesAsync();
            Console.WriteLine("AccountTypes seeded successfully!");
        }

        // Check if account data already exists
        if (await context.Accounts.AnyAsync())
        {
            return; // Data already seeded
        }

        // Get seeded account types
        var customerType = await context.AccountTypes.FirstAsync(at => at.Code == "CUST");
        var vendorType = await context.AccountTypes.FirstAsync(at => at.Code == "VEND");
        var partnerType = await context.AccountTypes.FirstAsync(at => at.Code == "PART");

        // Seed sample accounts
        var accounts = new List<Account>
        {
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "ABC Manufacturing",
                Code = "ABC001",
                Description = "Primary manufacturing partner",
                AccountTypeId = customerType.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "XYZ Logistics",
                Code = "XYZ002",
                Description = "Logistics and shipping partner",
                AccountTypeId = partnerType.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "Demo Supplier",
                Code = "DEMO001",
                Description = "Demo account for testing",
                AccountTypeId = vendorType.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Accounts.AddRangeAsync(accounts);
        await context.SaveChangesAsync();

        // Seed sample configurations for the first account
        var demoAccount = accounts.First();
        var configurations = new List<AccountConfiguration>
        {
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "EDI.FileFormat",
                ConfigurationValue = "X12",
                ConfigurationType = "EDI",
                Description = "Supported EDI file format",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "EDI.TransactionTypes",
                ConfigurationValue = "850,810,856,855,820,862,997",
                ConfigurationType = "EDI",
                Description = "Supported X12 transaction types",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "EDI.EdifactMessageTypes",
                ConfigurationValue = "ORDERS,INVOIC,DESADV,ORDRSP,PAYMUL,REMADV,DELFOR,CONTRL",
                ConfigurationType = "EDI",
                Description = "Supported EDIFACT message types",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "SFTP.HostName",
                ConfigurationValue = "sftp.example.com",
                ConfigurationType = "SFTP",
                Description = "SFTP server hostname",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "SFTP.UserName",
                ConfigurationValue = "ediuser",
                ConfigurationType = "SFTP",
                Description = "SFTP username",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "SFTP.Password",
                ConfigurationValue = "ZGVtb3Bhc3N3b3Jk", // base64 encoded "demopassword"
                ConfigurationType = "SFTP",
                Description = "SFTP password",
                IsEncrypted = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "SFTP.IncomingPath",
                ConfigurationValue = "/incoming",
                ConfigurationType = "SFTP",
                Description = "SFTP incoming files path",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "SFTP.ProcessedPath",
                ConfigurationValue = "/processed",
                ConfigurationType = "SFTP",
                Description = "SFTP processed files path",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "API.EndpointUrl",
                ConfigurationValue = "https://api.example.com/webhook",
                ConfigurationType = "API",
                Description = "Webhook endpoint for notifications",
                IsEncrypted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AccountConfiguration
            {
                Id = Guid.NewGuid(),
                AccountId = demoAccount.Id,
                ConfigurationKey = "API.ApiKey",
                ConfigurationValue = "YWJjZGVmZ2hpams=", // base64 encoded "abcdefghijk"
                ConfigurationType = "API",
                Description = "API key for webhook authentication",
                IsEncrypted = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.AccountConfigurations.AddRangeAsync(configurations);
        await context.SaveChangesAsync();

        Console.WriteLine("Sample data seeded successfully!");
    }
}
