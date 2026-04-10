using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{
    public class PriceTranslationConfigurations : BaseEntityConfigurations<PriceTranslation, Guid>

    {
        public override void Configure(EntityTypeBuilder<PriceTranslation> builder)
        {

            base.Configure(builder);

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasMaxLength(400);

            builder.Property(x => x.Language)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.PriceId, x.Language })
                   .IsUnique();

            builder.HasIndex(x => x.Language);

            builder.HasOne(x => x.Price)
                   .WithMany(x => x.Translations)
                   .HasForeignKey(x => x.PriceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
