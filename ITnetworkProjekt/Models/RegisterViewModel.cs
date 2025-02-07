using System.ComponentModel.DataAnnotations;

namespace ITnetworkProjekt.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "InvalidEmail")]
        [Display(Name = "EmailLabel", ResourceType = typeof(Resources.Models.RegisterViewModelResources))]
        public string Email { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "SocialSecurityNumberRequired")]
        [Display(Name = "SocialSecurityNumberLabel", ResourceType = typeof(Resources.Models.RegisterViewModelResources))]
        [StringLength(11)]
        [RegularExpression(@"^\s*\d{6}/\d{4}\s*$",
            ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "InvalidSocialSecurityNumber")]
        public string SocialSecurityNumber { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "PasswordLength", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordLabel", ResourceType = typeof(Resources.Models.RegisterViewModelResources))]
        public string Password { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPasswordLabel", ResourceType = typeof(Resources.Models.RegisterViewModelResources))]
        [Compare(nameof(Password), ErrorMessageResourceType = typeof(Resources.Models.RegisterViewModelResources),
            ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; } = "";
    }
}