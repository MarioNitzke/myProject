using System.ComponentModel.DataAnnotations;

namespace ITnetworkProjekt.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Models.LoginViewModelResources),
            ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Models.LoginViewModelResources),
            ErrorMessageResourceName = "InvalidEmail")]
        [Display(Name = "EmailLabel", ResourceType = typeof(Resources.Models.LoginViewModelResources))]
        public string Email { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.LoginViewModelResources),
            ErrorMessageResourceName = "PasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordLabel", ResourceType = typeof(Resources.Models.LoginViewModelResources))]
        public string Password { get; set; } = "";

        [Display(Name = "RememberMeLabel", ResourceType = typeof(Resources.Models.LoginViewModelResources))]
        public bool RememberMe { get; set; }
    }
}