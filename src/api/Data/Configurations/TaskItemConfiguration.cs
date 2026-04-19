using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data.Configurations;

public class TaskItemConfiguration() : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Notes).HasMaxLength(4000);
        entity.Property(e => e.Status).HasConversion<int>();
        entity.Property(e => e.Priority).HasConversion<int>();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}
