using ITnetworkProjekt.Models;
using MediatR;

namespace ITnetworkProjekt.Features.InsuredPersons.Queries
{
    public class GetInsuredPersonByEmailAndSocialSecurityNumberQuery : IRequest<InsuredPersonViewModel?>
    {
        public string Email { get; }
        public string SocialSecurityNumber { get; }

        public GetInsuredPersonByEmailAndSocialSecurityNumberQuery(string email, string socialSecurityNumber)
        {
            Email = email;
            SocialSecurityNumber = socialSecurityNumber;
        }
    }
}
