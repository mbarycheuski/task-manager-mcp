using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data.Configurations;

public class ApiKeyConfiguration() : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(e => e.ClientName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.KeyHash).IsRequired().HasMaxLength(128);
        entity.Property(e => e.Salt).IsRequired().HasMaxLength(64);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
        entity.Property(e => e.IsActive).HasDefaultValue(true);
    }
}
