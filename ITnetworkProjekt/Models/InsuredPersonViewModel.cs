using System.ComponentModel.DataAnnotations;
using ITnetworkProjekt.Validation;

namespace ITnetworkProjekt.Models
{
    public class InsuredPersonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "FirstNameRequired")]
        [Display(Name = "FirstNameLabel", ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "LastNameRequired")]
        [Display(Name = "LastNameLabel", ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        public string LastName { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "DateOfBirthRequired")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "DateOfBirthLabel", ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "PhoneNumberRequired")]
        [Display(Name = "PhoneNumberLabel", ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "EmailRequired")]
        [UniqueEmail(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "EmailUnique")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "InvalidEmail")]
        [Display(Name = "EmailLabel", ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        public string Email { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "AddressRequired")]
        [Display(Name = "AddressLabel", ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        public string Address { get; set; } = "";

        [Display(Name = "CreatedDateLabel", ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "SocialSecurityNumberRequired")]
        [UniqueSocialSecurityNumber(ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "SocialSecurityNumberUnique")]
        [Display(Name = "SocialSecurityNumberLabel",
            ResourceType = typeof(Resources.Models.InsuredPersonViewModelResources))]
        [StringLength(11)]
        [RegularExpression(@"^\s*\d{6}/\d{4}\s*$",
            ErrorMessageResourceType = typeof(Resources.Models.InsuredPersonViewModelResources),
            ErrorMessageResourceName = "InvalidSocialSecurityNumber")]
        public string SocialSecurityNumber { get; set; } = "";

        [MaxLength(450)] public string? UserId { get; set; } = "";

        public List<int>? InsuranceIds { get; set; }
    }
}