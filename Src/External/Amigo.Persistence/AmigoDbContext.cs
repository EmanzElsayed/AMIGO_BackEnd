namespace Amigo.Persistence
{
    public class AmigoDbContext(DbContextOptions<AmigoDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

        // Translation Entities
        public DbSet<DestinationTranslation> DestinationTranslations { get; set; }
        public DbSet<ReviewTranslation> ReviewTranslations { get; set; }
        public DbSet<CancellationTranslation> CancelationTranslations { get; set; }
        public DbSet<TourTranslation> TourTranslations { get; set; }

        // Core Entities

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Destination> Destinations { get; set; }

        public DbSet<AvailableSlots> AvailableSlots { get; set; }
        public DbSet<TourSchedule> TourSchedules { get; set; }
        public DbSet<Cancellation> Cancelations { get; set; }
        public DbSet<Price> Prices { get; set; }

        public DbSet<TourImage> TourImages { get; set; }
        public DbSet<TourIncluded> TourIncluded { get; set; }
        public DbSet<TourNotIncluded> TourNotIncluded { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewImages
        {
            get; set;
        }


        // Booking & Order

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<PeopleBooking> PeopleBookings { get; set; }
        public DbSet<PeopleBookingDetails> PeopleBookingDetails { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }

        // ---------------------------
        // Basket / Favorites
        // ---------------------------
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
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
