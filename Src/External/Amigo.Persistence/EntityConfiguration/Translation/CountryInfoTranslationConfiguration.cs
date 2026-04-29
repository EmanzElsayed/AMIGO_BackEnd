using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{
    public class CountryInfoTranslationConfiguration : BaseEntityConfigurations<CountryInfoTranslation, Guid>
    {
        public override void Configure(EntityTypeBuilder<CountryInfoTranslation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(300);

            builder.Property(x => x.Language)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.CountryInfoId, x.Language })
                   .IsUnique();

            builder.HasIndex(x => x.Language);

            builder.HasOne(x => x.CountryInfo)
                   .WithMany(x => x.Translations)
                   .HasForeignKey(x => x.CountryInfoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    
    }
}
