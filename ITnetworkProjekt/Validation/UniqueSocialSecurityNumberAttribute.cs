using System.ComponentModel.DataAnnotations;
using ITnetworkProjekt.Data;

namespace ITnetworkProjekt.Validation
{
    public class UniqueSocialSecurityNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string ssn && !string.IsNullOrEmpty(ssn))
            {
                var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
                bool ssnExists = dbContext.InsuredPerson.Any(p => p.SocialSecurityNumber == ssn);

                if (ssnExists)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
