namespace Amigo.Domain.Entities.Identity;

[Table($"{nameof(OTP)}", Schema = SchemaConstants.auth_schema)]
public class OTP: BaseEntity<Guid>
{
    public string Email { get; set; }
    public string Code { get; set; }
    public DateTime ExpireAt { get; set; }
    public OtpPurpose purpose { get; set; }
    private OTP() { }
    public OTP(string email, string code, DateTime expireAt, OtpPurpose purpose)
    {
        Email = email;
        Code = code;
        ExpireAt = expireAt;
        this.purpose = purpose;
    }
}
