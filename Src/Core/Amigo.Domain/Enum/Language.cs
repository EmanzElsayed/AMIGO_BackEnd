namespace Amigo.Domain.Enum;

[Flags]
public enum Language
{
    None = 0,
    English = 1,

    [Display(Name = "Español")]
    Espanol = 2,         
    [Display(Name = "Français")]

    Francais = 3,          
    Italiano = 4,          
    [Display(Name = "Português (PT)")]
    Portuguese_Portugal = 5, 
    [Display(Name = "Português (BR)")]
    Portuguese = 6    
}
