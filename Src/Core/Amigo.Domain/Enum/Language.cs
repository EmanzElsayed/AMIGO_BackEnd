namespace Amigo.Domain.Enum;

public enum Language
{
    English = 1,          // en
    Español = 2,          // es
    Français = 3,           // fr
    Italiano = 4,          // it
    [Display(Name = "Português (PT)")]
    Portuguese_Portugal = 5, // pt-PT
    [Display(Name = "Português (BR)")]
    Portuguese = 6    // pt-BR
}
