using Amigo.Domain.Entities.TranslationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{
   
    public class ReviewTranslationConfiguration
        : BaseEntityConfigurations<ReviewTranslation, Guid>
    {
        public override void Configure(EntityTypeBuilder<ReviewTranslation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Comment)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(x => x.Language)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.ReviewId, x.Language })
                   .IsUnique();

            builder.HasIndex(x => x.Language);

            builder.HasOne(x => x.Review)
                   .WithMany()
                   .HasForeignKey(x => x.ReviewId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
