using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Domain;

namespace Sample.Infrastructure.EntityFramework.Relations;

internal class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("T_Question");

        builder.HasKey(x => x.Id);

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Ignore(e => e.DeleteById);

    }
}
