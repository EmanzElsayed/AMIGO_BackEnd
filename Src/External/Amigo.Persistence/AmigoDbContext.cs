using Amigo.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amigo.Persistence;

public class AmigoDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AmigoDbContext(DbContextOptions<AmigoDbContext> options) : base(options)
    {
    }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<DestinationTranslation> DestinationTranslations => Set<DestinationTranslation>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourTranslation> TourTranslations => Set<TourTranslation>();
    public DbSet<TourMedia> TourMedia => Set<TourMedia>();
    public DbSet<TourIncluded> TourIncluded => Set<TourIncluded>();
    public DbSet<TourNotIncluded> TourNotIncluded => Set<TourNotIncluded>();
    public DbSet<TourSchedule> TourSchedules => Set<TourSchedule>();
    public DbSet<TourAvailability> TourAvailabilities => Set<TourAvailability>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryTranslation> CategoryTranslations => Set<CategoryTranslation>();
    public DbSet<TourCategory> TourCategories => Set<TourCategory>();
    public DbSet<PriceCategory> PriceCategories => Set<PriceCategory>();
    public DbSet<TourPrice> TourPrices => Set<TourPrice>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();
    public DbSet<BookingTraveler> BookingTravelers => Set<BookingTraveler>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CancellationPolicy> CancellationPolicies => Set<CancellationPolicy>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewImage> ReviewImages => Set<ReviewImage>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<InterestedTour> InterestedTours => Set<InterestedTour>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AmigoDbContext).Assembly);
    }
}
