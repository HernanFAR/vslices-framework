using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Relations;

internal class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("T_Question");

        builder.HasKey(x => x.Id)
            .IsClustered(false);

        builder.Property<int>("ClusteredKey")
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasIndex("ClusteredKey")
            .IsClustered();

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Ignore(e => e.DeleteById);

    }
}
