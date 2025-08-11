using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Infrastructure.Data.Configurations;

public class AccountConfigurationEntityConfiguration : IEntityTypeConfiguration<AccountConfiguration>
{
    public void Configure(EntityTypeBuilder<AccountConfiguration> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ConfigurationKey)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.ConfigurationValue)
            .IsRequired();
            
        builder.Property(x => x.Description)
            .HasMaxLength(500);
            
        builder.Property(x => x.ConfigurationType)
            .HasMaxLength(50)
            .HasDefaultValue("General");
            
        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Composite index for account and configuration key
        builder.HasIndex(x => new { x.AccountId, x.ConfigurationKey })
            .IsUnique();

        // Relationship with Account
        builder.HasOne(x => x.Account)
            .WithMany(x => x.Configurations)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
