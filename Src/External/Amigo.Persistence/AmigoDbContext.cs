namespace Amigo.Persistence
{
    public class AmigoDbContext(DbContextOptions<AmigoDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

        // Translation Entities
        public DbSet<DestinationTranslation> DestinationTranslations { get; set; }
        public DbSet<WebhookEventLog> WebhookEventLogs { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<ReviewTranslation> ReviewTranslations { get; set; }
        public DbSet<CancellationTranslation> CancelationTranslations { get; set; }
        public DbSet<TourTranslation> TourTranslations { get; set; }
        public DbSet<InclusionTranslation> InclusionTranslations { get; set; }
        public DbSet<CurrencyTranslation> CurrencyTranslations { get; set; }
        public DbSet<CountryInfoTranslation> CountryInfoTranslations { get; set; }


        // Core Entities

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Destination> Destinations { get; set; }

        public DbSet<Currency> Currency { get; set; }
        public DbSet<CountryInfo> CountryInfo { get; set; }

        public DbSet<AvailableSlots> AvailableSlots { get; set; }
        public DbSet<SlotReservation> SlotReservations { get; set; }

        public DbSet<TourSchedule> TourSchedules { get; set; }
        public DbSet<Cancellation> Cancelations { get; set; }
        public DbSet<Price> Prices { get; set; }

        public DbSet<TourImage> TourImages { get; set; }
        public DbSet<TourInclusion> TourInclusion { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewImages
        {
            get; set;
        }


        // Booking & Order

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderedPrice> OrderedPrices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CancellationRequest> CancellationRequests { get; set; }
        public DbSet<Voucher> vouchers { get; set; }
        public DbSet<Refund> Refunds { get; set; }


        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }

        // ---------------------------
        // Cart / Favorites
        // ---------------------------
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartPrice> CartPrices { get; set; }

        public DbSet<Favorites> Favorites { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AmigoDbContext).Assembly);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<UserRefreshToken>().ToTable("RefreshTokens");
        }

    }
}
