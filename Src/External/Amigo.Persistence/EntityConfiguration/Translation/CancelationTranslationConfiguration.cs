using Amigo.Domain.Entities.TranslationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{

    public class CancelationTranslationConfiguration
        : BaseEntityConfigurations<CancellationTranslation, Guid>
    {
        public override void Configure(EntityTypeBuilder<CancellationTranslation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Description)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(x => x.Language)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.CancellationId, x.Language })
                   .IsUnique();

            builder.HasIndex(x => x.Language);

            builder.HasOne(x => x.Cancellation)
                   .WithMany()
                   .HasForeignKey(x => x.CancellationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
