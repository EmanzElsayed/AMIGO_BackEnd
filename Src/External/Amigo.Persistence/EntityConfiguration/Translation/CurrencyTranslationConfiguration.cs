using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{
    public class CurrencyTranslationConfiguration
        : BaseEntityConfigurations<CurrencyTranslation, Guid>
    {
        public override void Configure(EntityTypeBuilder<CurrencyTranslation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(300);

            builder.Property(x => x.Language)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.CurrencyId, x.Language })
                   .IsUnique();

            builder.HasIndex(x => x.Language);

            builder.HasOne(x => x.Currency)
                   .WithMany(x => x.Translations)
                   .HasForeignKey(x => x.CurrencyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
