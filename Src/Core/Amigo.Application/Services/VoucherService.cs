using Amigo.Application.Specifications.VoucherConfiguration;
using Amigo.Domain.DTO.Voucher;
using Amigo.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class VoucherService(IWebHostEnvironment env
        ,IEmailService emailService
        ,IUnitOfWork _unitOfWork)
        :IVoucherService
    {
        private string? _cachedTemplate;
        private string? _cachedReminderTemplate;

        private readonly IWebHostEnvironment _env = env;


        public async Task SendReminderEmail(Booking booking)
        {
            var html = BuildReminderHtml(booking);

            await emailService.SendEmailAsync(
               booking.CustomerEmail,
               "Reminder: Your tour is tomorrow – Amigo Tours",
               html
               );
        }
        public async Task SendVoucherEmail(Booking booking, Voucher voucher)
        {
            var html = BuildVoucherHtml(booking, voucher);
            var qrBytes = Convert.FromBase64String(voucher.QRCodeBase64);

            await emailService.SendEmailAsync(
                booking.CustomerEmail,
                "Your Voucher - Amigo Arabe Tours",
                html,qrBytes);
        }

        public async Task<Result<GetValidateVoucherDTO>> ValidateVoucher(string token)
        {
            var voucher = await _unitOfWork.GetRepository<Voucher, Guid>().GetByIdAsync(new GetVoucherWithTokenSpecification(token));
            if (voucher is null)
            {
                return Result.Fail(new NotFoundError("This Voucher Not Found"));

            }
            var result = new GetValidateVoucherDTO(voucher.VoucherNumber, voucher.Status.ToString(), voucher.IssuedAt);
            return Result.Ok(result);
        }


        private string BuildVoucherHtml(Booking booking, Voucher voucher)
        {
            var template = LoadTemplate();

            return template
                .Replace("{{CustomerName}}", booking.CustomerName)
                .Replace("{{CustomerEmail}}", booking.CustomerEmail)
                .Replace("{{TourName}}", booking.OrderItem.TourTitle)
                .Replace("{{TourDate}}", booking.OrderItem.TourDate.ToString("dd MMM yyyy"))
                .Replace("{{StartTime}}", booking.OrderItem.StartTime.ToString("hh:mm tt"))
                .Replace("{{DestinationName}}", booking.OrderItem.DestinationName)
                .Replace("{{MeetingPoint}}", booking.OrderItem.MeetingPoint ?? "")
                .Replace("{{NameAndAddressOfAccomodation}}", booking.NameAndAddressOfAccomodation ?? "")
                .Replace("{{TravelerCount}}", booking.Travelers.Count.ToString())
                .Replace("{{TravelersRows}}", GenerateTravelersRows(booking.Travelers.ToList()))
                .Replace("{{QRCode}}", voucher.QRCodeBase64);
        }

        private string BuildReminderHtml(Booking booking)
        {
            var template = LoadReminderTemplate();

            return template
                .Replace("{{CustomerName}}", booking.CustomerName)
                .Replace("{{TourName}}", booking.OrderItem.TourTitle)
                .Replace("{{TourDate}}", booking.OrderItem.TourDate.ToString("dd MMM yyyy"))
                .Replace("{{StartTime}}", booking.OrderItem.StartTime.ToString(@"hh\:mm"))
                .Replace("{{DestinationName}}", booking.OrderItem.DestinationName)
                .Replace("{{MeetingPoint}}", booking.OrderItem.MeetingPoint ?? "");
        }
        private string GenerateTravelersRows(List<Traveler> travelers)
        {
            if (travelers == null || travelers.Count == 0)
                return "<div style='color:#999;font-size:13px;'>No travelers found</div>";

            var sb = new StringBuilder();

            foreach (var t in travelers)
            {
               sb.Append($@"
                    <div style='border:1px solid #eee; border-radius:10px; padding:12px 15px; margin-bottom:10px; background:#fff;'>

                        <div style='font-weight:600; font-size:14px; color:#222; margin-bottom:4px;'>
                            {Escape(t.FullName)}
                        </div>

                        <div style='font-size:12px; color:#777; display:flex; gap:10px;'>
                            <span>🌍 {Escape(t.Nationality)}</span>
                            <span>•</span>
                            <span>🧾 {Escape(t.Type.ToString())}</span>
                        </div>

                    </div>");
            }

            return sb.ToString();
        }
        private string Escape(string input)
        {
            return System.Net.WebUtility.HtmlEncode(input);
        }
        private string LoadTemplate()
        {
            if (_cachedTemplate != null)
                return _cachedTemplate;

            var path = Path.Combine(_env.ContentRootPath, "Templates", "voucher.html");
            _cachedTemplate = File.ReadAllText(path);

            return _cachedTemplate;
        }
        private string LoadReminderTemplate()
        {
            if (_cachedReminderTemplate != null)
                return _cachedReminderTemplate;

            var path = Path.Combine(_env.ContentRootPath, "Templates", "reminder.html");
            _cachedReminderTemplate = File.ReadAllText(path);

            return _cachedReminderTemplate;
        }
    }
}
