namespace Amigo.Domain.DTO.Authentication;

public record IdentifyEmailResponseDTO(
    string Status,
    bool OtpSent,
    string Message
);
