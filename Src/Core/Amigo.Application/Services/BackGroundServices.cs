using Amigo.Application.Abstraction.Services;
using Amigo.Application.Specifications.AvailableSlotsSpecification;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.GetBackGroundServicesSpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Security.Cryptography;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Amigo.Application.Services;

/// <summary>
/// Production Background Worker
/// Runs periodic maintenance jobs:
/// 1. Delete pending reservations
/// 2. Expire unpaid orders
/// </summary>
public sealed class BookingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<BookingBackgroundService> logger,
    
    IConfiguration config)
    : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);
    private readonly IConfiguration _config = config;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Booking Background Service Started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunJobs(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Background job failed");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        logger.LogInformation("Booking Background Service Stopped");
    }

    private async Task RunJobs(CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var voucherService = scope.ServiceProvider.GetRequiredService<IVoucherService>();
        await ExpireReservations(unitOfWork);
        await ExpireOrders(unitOfWork);
        await SlodOutSlots(unitOfWork);
        //await CreateVouchers(unitOfWork);
        await SendVoucherEmails(voucherService, unitOfWork);
        await SendTourReminderEmails(voucherService, unitOfWork);

    }

    // =====================================================
    // 1) Expire Pending Reservations
    // =====================================================
    private async Task ExpireReservations(IUnitOfWork _unitOfWork)
    {
        var now = DateTime.UtcNow;

        var repo = _unitOfWork.GetRepository<SlotReservation, Guid>();

        var reservations = await repo.GetAllAsync(
            new GetExpiredPendingReservationsSpecification(now));

        var slotGroups = reservations
       .GroupBy(x => x.SlotId)
       .Select(g => new
       {
           SlotId = g.Key,
           Total = g.Sum(x => x.Quantity)
       })
       .ToList();


        var updates = slotGroups
                .Select(x => (x.SlotId, x.Total))
                .ToList();

        await _unitOfWork.SlotsRepo
            .BulkDecreaseReservedCountAsync(updates); 

        repo.RemoveRange(reservations);



        await _unitOfWork.SaveChangesAsync();
    }

    // =====================================================
    // 2) Expire Orders still unpaid
    // =====================================================
    private async Task ExpireOrders(IUnitOfWork _unitOfWork)
    {
        var now = DateTime.UtcNow;

        var repo = _unitOfWork.GetRepository<Order, Guid>();

        var orders = await repo.GetAllAsync(
            new GetPendingOrdersBeforeDateSpecification(now));

        foreach (var order in orders)
        {
            order.Status = OrderStatus.Failed;
        }

        await _unitOfWork.SaveChangesAsync();
    }
    // =====================================================
    // 3) SoldOutSlots 
    // =====================================================
    private async Task SlodOutSlots(IUnitOfWork _unitOfWork)
    {
        

        var repo = _unitOfWork.GetRepository<AvailableSlots, Guid>();

        var availableSlots = await repo.GetAllAsync(
            new GetAllAvailableSlots());

        if (!availableSlots.Any())
            return;

        var avialableSlotsIds = availableSlots.Select(availableSlots => availableSlots.Id).ToList();

        var reservationRepo = _unitOfWork.GetRepository<SlotReservation, Guid>();

        var reservationsCount = await reservationRepo.GetGroupedCountAsync(
            x => avialableSlotsIds.Contains(x.SlotId),
            x => x.SlotId
        );

        foreach (var slot in availableSlots)
        {
            if (slot.TourSchedule.StartDate.ToDateTime(slot.StartTime) <= DateTime.UtcNow)
            {
                slot.AvailableTimeStatus = AvailableDateTimeStatus.SoldOut;
                
            }
            else

            {
                var count = reservationsCount.ContainsKey(slot.Id)
                      ? reservationsCount[slot.Id]
                      : 0;
                slot.ReservedCount = count;
                if (count >= slot.MaxCapacity)
                {
                    slot.AvailableTimeStatus = AvailableDateTimeStatus.SoldOut;
                    slot.SetModifiedDate(DateTime.UtcNow);
                }
            }
              
        }

        await _unitOfWork.SaveChangesAsync();
    }




    private async Task SendVoucherEmails(
    IVoucherService _voucherService,
    IUnitOfWork _unitOfWork
    )
    {
        var bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();
        var bookings = await bookingRepo.GetAllAsync(new GetBookingNotSendVoucherSpecifciation());
        if (!bookings.Any()) return;

        var semaphore = new SemaphoreSlim(5);

       
       

        var tasks = bookings.Select(async booking =>
        {
            await semaphore.WaitAsync();

            try
            {
                var voucherToken = GenerateToken();
                var validationUrl = $"{_config["FrontendAPIs:ValidateVoucher"]}/voucher?token={voucherToken}";


                booking.QRCodeBase64 = GenerateQrCode(validationUrl);
                booking.VoucherToken  = voucherToken;
                await _voucherService.SendVoucherEmail(booking);

                booking.IsVoucherSentByEmail = true;
                booking.VoucherSentAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Background job failed");
            }
            finally
            {
                semaphore.Release();
            }
        });


        await Task.WhenAll(tasks);
    }



    private async Task SendTourReminderEmails(
    IVoucherService voucherService,
    IUnitOfWork unitOfWork)
    {
        var now = DateTime.UtcNow;

        var from = now.AddHours(23);
        var to = now.AddHours(25);

        var bookings = await unitOfWork
            .GetRepository<Booking, Guid>()
            .GetAllAsync(new GetBookingsForReminderSpecification(from, to));

        foreach (var booking in bookings)
        {
            await voucherService.SendReminderEmail(booking);

            booking.ReminderSent = true;
        }

        await unitOfWork.SaveChangesAsync();
    }

    
   

    private string GenerateQrCode(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(20);

        return Convert.ToBase64String(qrBytes);
    }

    private string GenerateToken()
    { 
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}