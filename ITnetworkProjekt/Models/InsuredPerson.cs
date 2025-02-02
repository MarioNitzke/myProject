using System.ComponentModel.DataAnnotations;

namespace ITnetworkProjekt.Models
{
    public class InsuredPerson
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vyplňte jméno")]
        [Display(Name = "Jméno:")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte příjmení")]
        [Display(Name = "Příjmení:")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte datum narození:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Datum narození:")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vyplňte telefonní číslo")]
        [Display(Name = "Telefonní číslo:")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte emailovou adresu")]
        [EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte adresu")]
        [Display(Name = "Adresa:")]
        public string Address { get; set; } = "";

        [Display(Name = "Datum vytvoření")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vyplňte rodné číslo")]
        [Display(Name = "Rodné číslo:")]
        [StringLength(11)]
        [RegularExpression(@"^\s*\d{6}/\d{4}\s*$", ErrorMessage = "Neplatné rodné číslo.")]
        public string SocialSecurityNumber { get; set; } = "";

        [MaxLength(450)]
        public string? UserId { get; set; } = "";
        public virtual ICollection<Insurance>? Insurances { get; set; }
    }
}


