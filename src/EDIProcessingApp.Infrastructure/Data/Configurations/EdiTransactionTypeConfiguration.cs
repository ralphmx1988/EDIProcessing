using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Infrastructure.Data.Configurations;

public class EdiTransactionTypeConfiguration : IEntityTypeConfiguration<EdiTransactionType>
{
    public void Configure(EntityTypeBuilder<EdiTransactionType> builder)
    {
        builder.ToTable("EdiTransactionTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.X12Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.DocumentName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.EdifactName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Direction)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Both");

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedDate)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(e => e.X12Code)
            .IsUnique()
            .HasDatabaseName("IX_EdiTransactionTypes_X12Code");

        builder.HasIndex(e => e.EdifactName)
            .HasDatabaseName("IX_EdiTransactionTypes_EdifactName");

        builder.HasIndex(e => e.DocumentName)
            .HasDatabaseName("IX_EdiTransactionTypes_DocumentName");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_EdiTransactionTypes_IsActive");

        // Seed data based on the image
        builder.HasData(
            new EdiTransactionType 
            { 
                Id = 1, 
                X12Code = "850", 
                DocumentName = "Purchase Order", 
                EdifactName = "ORDERS", 
                Description = "Order request",
                Direction = "Both",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EdiTransactionType 
            { 
                Id = 2, 
                X12Code = "810", 
                DocumentName = "Invoice", 
                EdifactName = "INVOIC", 
                Description = "Billing information",
                Direction = "Both",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EdiTransactionType 
            { 
                Id = 3, 
                X12Code = "856", 
                DocumentName = "Advance Ship Notice", 
                EdifactName = "DESADV", 
                Description = "Shipping notification",
                Direction = "Inbound",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EdiTransactionType 
            { 
                Id = 4, 
                X12Code = "855", 
                DocumentName = "Purchase Order Acknowledgement", 
                EdifactName = "ORDRSP", 
                Description = "PO confirmation",
                Direction = "Outbound",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EdiTransactionType 
            { 
                Id = 5, 
                X12Code = "820", 
                DocumentName = "Payment Order/Remittance", 
                EdifactName = "PAYMUL", 
                Description = "Payment/remittance information",
                Direction = "Both",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EdiTransactionType 
            { 
                Id = 6, 
                X12Code = "862", 
                DocumentName = "Shipping Schedule", 
                EdifactName = "DELFOR", 
                Description = "Delivery schedule",
                Direction = "Both",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new EdiTransactionType 
            { 
                Id = 7, 
                X12Code = "997", 
                DocumentName = "Functional Acknowledgment", 
                EdifactName = "CONTRL", 
                Description = "Receipt confirmation of EDI message",
                Direction = "Both",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
