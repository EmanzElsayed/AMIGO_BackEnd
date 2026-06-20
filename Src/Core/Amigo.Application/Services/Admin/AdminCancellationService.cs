using Amigo.Application.Specifications.RefundSpecification;
using Amigo.Domain.DTO.Order;
using Amigo.Domain.DTO.Refund;
using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Services.Admin
{
    public class AdminCancellationService(IUnitOfWork _unitOfWork, IEmailService _emailService) 
        : IAdminCancellationService
    {
        public async Task<Result<PaginatedResponse<GetAllCancellationRequestsDTO>>> GetAllCancellationRequestsAsync(GetAllAdminCancellationRequestQuery requestQuery)
        {
            var cancellationRequests = await _unitOfWork.GetRepository<CancellationRequest, Guid>().GetAllAsync(new GetAllCancellationRequestsSpecification(requestQuery));
            var mappedData =  MapToDTO(cancellationRequests);

            var totalItems = await _unitOfWork.GetRepository<CancellationRequest, Guid>().GetCountSpecificationAsync(new CountGetAllCancellationRequestsSpecification(requestQuery));

            var totalPages = requestQuery.PageSize <= 0
                          ? 0
                          : (int)Math.Ceiling(totalItems / (double)requestQuery.PageSize);

            var response = new PaginatedResponse<GetAllCancellationRequestsDTO>
            {
                Data = mappedData,
                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
            return Result.Ok(response);

        }

        public async Task<Result> ApproveCancellationRequest(string Id )
        {

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid cancellationRequestId = guid;
            var request = await _unitOfWork.GetRepository<CancellationRequest, Guid>().GetByIdAsync(new GetCancellationRequestByIdSpecification(cancellationRequestId));

            if (request is null)
            {
                return Result.Fail(new NotFoundError("Not Found"));
            }

            if (request.Status != CancellationRequestStatus.Pending)
            {
                return Result.Fail(new ConfilctError($"Request already handeled => status : {request.Status} "));

            }
            var refundRepo = _unitOfWork.GetRepository<Refund, Guid>();

            var alreadyRefunded =
                    await refundRepo.AnyAsync( new GetRefundByBookingIdSpecification(request.BookingId)
                       );

            if (alreadyRefunded)
            {
                return Result.Fail(new ConfilctError($"BookingID Already Processed"));

            }
            var refund = new Refund
            {
                Id = Guid.NewGuid(),
                BookingId =request.BookingId,
                Booking = request.Booking,
                PaymentId = request.Booking.PaymentId,


                CancellationRequestId = request.Id,

                Amount = request.RefundAmount,

                Status = RefundStatus.Pending,

                RequestedAt = DateTime.UtcNow,

                IdempotencyKey =
                 $"refund-{request.BookingId}"
            };
            await refundRepo.AddAsync(refund);
            request.Status =
                CancellationRequestStatus.Approved;
            
            var outboxMessage = new OutboxMessage
           
            {
                Id = Guid.NewGuid(),
                Type = "RefundRequested",
                Payload = refund.Id.ToString(),
                Status = OutboxStatus.Pending,

                RetryCount = 0
            };
            await _unitOfWork.GetRepository<OutboxMessage, Guid>().AddAsync(outboxMessage);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok().WithSuccess(new Success("Cancellation Request Approved Successfully"));

            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);
            }
        }

        private  List<GetAllCancellationRequestsDTO> MapToDTO(IEnumerable<CancellationRequest> cancellationRequests)
        {
            return cancellationRequests.Select( request => 
            new GetAllCancellationRequestsDTO(
                    Id: request.Id,
                    BookingId: request.BookingId,
                    BookingNumber: request.Booking.BookingNumber ?? "",
                    TourId: request.Booking.OrderItem.TourId.Value,
                    TourTitle: request.Booking.OrderItem.TourTitle,
                    TourDate: request.Booking.OrderItem.TourDate.ToDateTime(request.Booking.OrderItem.StartTime),
                    UserName: request.Booking.CustomerName,
                    UserEmail: request.Booking.CustomerEmail,
                    PaidAmount: request.Booking.Payment.TotalAmount,
                    RefundAmount: request.RefundAmount,
                    RequestedAt: request.RequestedAt,
                    Status: request.Status.ToString(),
                    Reason: request.Reason,
                    CancellationPolicyType: request.cancelationPolicyType.ToString(),
                    RefundPercentage : request.RefundPercentage


                    )
                ).ToList();
        
        }

        public async Task<Result> RejectCancellationRequest(string Id, RejectCancellationRequestDTO requestDTO)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid cancellationRequestId = guid;
            var request = await _unitOfWork.GetRepository<CancellationRequest, Guid>().GetByIdAsync(new GetCancellationRequestByIdSpecification(cancellationRequestId));

            if (request is null)
            {
                return Result.Fail(new NotFoundError("Not Found"));
            }

            if (request.Status != CancellationRequestStatus.Pending)
            {
                return Result.Fail(new ConfilctError($"Request already handeled => status : {request.Status} "));

            }

            // UPDATE REQUEST

            request.Status =
                CancellationRequestStatus.Rejected;

            request.ProcessedAt =
                DateTime.UtcNow;
            // OPTIONAL REJECTION REASON

            if (!string.IsNullOrWhiteSpace(
                    requestDTO.RejectionReason))
            {
                request.AdminNotes =
                     requestDTO.RejectionReason;
            }

            // RETURN BOOKING BACK TO CONFIRMED

            if (request.Booking != null)
            {
                request.Booking.Status =
                    BookingStatus.Confirmed;
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();

                var rejectionReasonSection = string.IsNullOrWhiteSpace(requestDTO.RejectionReason)
                    ? string.Empty
                    : $@"
                        <div style='background:#fef2f2;padding:15px;border-radius:8px;margin:20px 0;border-left:4px solid #dc2626;'>
                            <p style='margin:0;font-weight:bold;color:#b91c1c;'>Reason for Rejection</p>
                            <p style='margin:8px 0 0 0;color:#444;'>{requestDTO.RejectionReason}</p>
                        </div>";
                        await _emailService.SendEmailAsync(
                        request.Booking.CustomerEmail,
                        "Cancellation Request Update",
                        $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>

                            <h2 style='color: #dc2626;'>Cancellation Request Declined</h2>

                            <p>Hello {request.Booking.CustomerName},</p>

                            <p>
                                We have reviewed your cancellation request for the booking below.
                            </p>

                            <div style='background:#f9fafb;padding:15px;border-radius:8px;margin:20px 0;border:1px solid #e5e7eb;'>

                                <p style='margin:0 0 10px 0;'>
                                    <strong>Booking Reference:</strong> {request.Booking?.BookingNumber}
                                </p>

                                <p style='margin:0;'>
                                    <strong>Tour:</strong> {request.Booking?.OrderItem?.TourTitle}
                                </p>

                            </div>

                            <p>
                                Unfortunately, your cancellation request has been declined and your booking remains
                                <strong>confirmed</strong>.
                            </p>

                            {rejectionReasonSection}

                            <p>
                                If you believe this decision was made in error or you need further clarification,
                                please contact our support team.
                            </p>

                            <p>
                                Thank you for your understanding.
                            </p>

                            <hr style='border:0;border-top:1px solid #eee;margin:20px 0;'>

                            <p style='font-size:12px;color:#999;'>
                                Amigo Arabe Tours Team
                            </p>

                        </div>"
                    );

                return Result.Ok()
                    .WithSuccess(
                        new Success(
                            "Cancellation request rejected successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension
                    .FromException(
                        details: ex.Message);
            }
        }
    }
}
