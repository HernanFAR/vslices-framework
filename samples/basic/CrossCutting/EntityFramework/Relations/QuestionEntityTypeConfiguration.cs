using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrossCutting.EntityFramework.Relations;

public class QuestionEntityTypeConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable(Question.Database.Name);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(Question.TitleField.TitleMaxLength);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(Question.ContentField.ContentMaxLength);
    }
}
