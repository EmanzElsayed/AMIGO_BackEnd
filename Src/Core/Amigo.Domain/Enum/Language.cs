namespace Amigo.Domain.Enum;

public enum Language
{
    English = 1,
    // en
    [Display(Name = "Español")]
    Espanol = 2,          // es
    [Display(Name = "Français")]

    Francais = 3,           // fr
    Italiano = 4,          // it
    [Display(Name = "Português (PT)")]
    Portuguese_Portugal = 5, // pt-PT
    [Display(Name = "Português (BR)")]
    Portuguese = 6    // pt-BR
}
