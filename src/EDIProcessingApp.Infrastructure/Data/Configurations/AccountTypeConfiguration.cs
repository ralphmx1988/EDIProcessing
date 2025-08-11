using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Infrastructure.Data.Configurations;

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.ToTable("AccountTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedDate);

        // Unique constraint on Code
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_AccountTypes_Code");

        // Index on Name for searching
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_AccountTypes_Name");

        // Index on IsActive for filtering
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_AccountTypes_IsActive");

        // Configure relationship with Accounts
        builder.HasMany(x => x.Accounts)
            .WithOne(x => x.AccountType)
            .HasForeignKey(x => x.AccountTypeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if accounts exist

        // Seed data for common account types
        builder.HasData(
            new AccountType
            {
                Id = 1,
                Name = "Customer",
                Code = "CUST",
                Description = "Customer accounts that receive goods or services",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AccountType
            {
                Id = 2,
                Name = "Vendor",
                Code = "VEND",
                Description = "Vendor/Supplier accounts that provide goods or services",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AccountType
            {
                Id = 3,
                Name = "Partner",
                Code = "PART",
                Description = "Business partner accounts for collaborative transactions",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AccountType
            {
                Id = 4,
                Name = "Distributor",
                Code = "DIST",
                Description = "Distributor accounts for product distribution networks",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AccountType
            {
                Id = 5,
                Name = "Logistics Provider",
                Code = "LOGIS",
                Description = "Third-party logistics and shipping provider accounts",
                IsActive = true,
                CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
