using Amigo.Domain.Entities.Identity;
using Amigo.Domain.Enum;

namespace Amigo.Application.Specifications.Identity;

public class OTPVerifySpecification : BaseSpecification<OTP, Guid>
{
    public OTPVerifySpecification(string email, string code, OtpPurpose purpose)
        : base(x => x.Email == email && x.Code == code && x.purpose == purpose && x.ExpireAt > DateTime.UtcNow)
    {
    }
}
