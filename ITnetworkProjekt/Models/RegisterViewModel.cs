using System.ComponentModel.DataAnnotations;

namespace ITnetworkProjekt.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vyplňte emailovou adresu")]
        [EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte rodné číslo")]
        [Display(Name = "Rodné číslo:")]
        [StringLength(11)]
        [RegularExpression(@"^\s*\d{6}/\d{4}\s*$", ErrorMessage = "Neplatné rodné číslo.")]
        public string SocialSecurityNumber { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte heslo")]
        [StringLength(100, ErrorMessage = "{0} musí mít délku alespoň {2} a nejvíc {1} znaků.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte heslo")]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [Compare(nameof(Password), ErrorMessage = "Zadaná hesla se musí shodovat.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
