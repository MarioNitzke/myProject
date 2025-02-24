using ITnetworkProjekt.Models;
using MediatR;

namespace ITnetworkProjekt.Features.InsuredPersons.Queries
{
    public class GetInsuredPersonForLoggedUserQuery : IRequest<InsuredPersonViewModel?>
    {
        public string UserId { get; }

        public GetInsuredPersonForLoggedUserQuery(string userId)
        {
            UserId = userId;
        }
    }
}
