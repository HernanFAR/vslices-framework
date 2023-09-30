using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrossCutting.EntityFramework.Relations;

public class IdempotencyEventEntityTypeConfiguration : IEntityTypeConfiguration<IdempotencyEvent>
{
    public void Configure(EntityTypeBuilder<IdempotencyEvent> builder)
    {
        builder.ToTable(IdempotencyEvent.Database.TableName);

        builder.HasKey(x => new { x.EventId, x.HandlerId });
    }
}
