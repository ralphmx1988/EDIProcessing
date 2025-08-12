using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Infrastructure.Data.Configurations;

public class EdiFileConfiguration : IEntityTypeConfiguration<EdiFile>
{
    public void Configure(EntityTypeBuilder<EdiFile> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(x => x.FileType)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.EdiTransactionTypeId)
            .IsRequired(false); // Optional foreign key
            
        builder.Property(x => x.TransactionType)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");
            
        builder.Property(x => x.FileLocation)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(x => x.ReceivedAt)
            .IsRequired();
            
        builder.HasIndex(x => x.FileName);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ReceivedAt);

        // Relationships
        builder.HasOne(x => x.Account)
            .WithMany(x => x.EdiFiles)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(x => x.EdiTransactionType)
            .WithMany(x => x.EdiFiles)
            .HasForeignKey(x => x.EdiTransactionTypeId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.File)
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
