using Amigo.Domain.Entities;
using Amigo.Domain.Enum;

using Microsoft.AspNetCore.Mvc;


namespace Amigo.Presentation.Controllers;
[ApiController]
[Route("api/Tour")]
public class TourController: ControllerBase
{
    [HttpGet]
    public IActionResult GetTour()
    {
        var tour = new Tour
        {
            
            DestinationId = Guid.NewGuid(),
            Destination = new Destination
            {
                
                CountryCode = CountryCode.Uae, 
                Name = "New York",
                IsActive = true,
                ImageUrl = "https://example.com/ny.jpg",
                Latitude = 40.7128,
                Longitude = -74.0060
            },
            DurationMinutes = 180,
            DiscountPercentage = 10.0m,
            IsPetsAllowed = true,
            IsWheelchairAccessible = false,
            IsPrivate = false,
            InstantConfirmation = true,
            IsActive = true,
            MeetingPoint = "Central Park",
            MeetingPointLatitude = 40.785091,
            MeetingPointLongitude = -73.968285,
            Currency = Currency.Usd,
            RatingAverage = 4.7,
            ReviewCount = 123,
            BookingCount = 45,
            IsFeatured = true,
            Schedules = new List<TourSchedule>
            {
                new TourSchedule
                {
                   
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    MaxCapacity = 20,
                    IsActive = true
                }
            },
            IncludedItems = new List<TourIncluded>
            {
                new TourIncluded {  Description = "Guide" },
                new TourIncluded {  Description = "Snacks" }
            },
            NotIncludedItems = new List<TourNotIncluded>
            {
                new TourNotIncluded {  Description = "Transport" }
            },
            Prices = new List<TourPrice>
            {
                new TourPrice { Price = 99.99m, Currency = Currency.Usd }
            },
            Categories = new List<TourCategory>
            {
                new TourCategory { CategoryId = Guid.NewGuid() }
            },
            CancellationPolicies = new List<CancellationPolicy>
            {
                new CancellationPolicy { HoursBefore = 24, RefundPercentage = 80 }
            },
            Translations = new List<TourTranslation>
            {
                new TourTranslation
                {
                 
                    Language = Language.English,
                    Title = "NYC City Tour",
                    Description = "A wonderful tour in New York City.",
                    DestinationInfo = "Central Park and more."
                }
            }
        };
        return Ok(tour);
    }
}