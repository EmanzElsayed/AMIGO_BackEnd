namespace Amigo.Domain.Enum;

public enum Language
{
    English = 1,          // en
    Spanish = 2,          // es
    French = 3,           // fr
    Italian = 4,          // it
    [Display(Name = "Portuguese (Portugal)")]
    Portuguese_Portugal = 5, // pt-PT
    [Display(Name = "Portuguese (Brazil)")]
    Portuguese = 6    // pt-BR
}
