using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.TransactionType)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.PartnerId)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");
            
        builder.Property(x => x.JsonData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
            
        builder.Property(x => x.ProcessedAt)
            .IsRequired();

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.PartnerId);
        builder.HasIndex(x => x.ProcessedAt);

        // Relationships
        builder.HasOne(x => x.File)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Account)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
