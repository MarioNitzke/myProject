using ITnetworkProjekt.Data;
using System.ComponentModel.DataAnnotations;

namespace ITnetworkProjekt.Validation
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string email && !string.IsNullOrEmpty(email))
            {
                var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext))!;
                bool emailExists = dbContext.InsuredPerson.Any(p => p.Email == email);

                if (emailExists)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
