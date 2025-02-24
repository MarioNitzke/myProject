using ITnetworkProjekt.Models;
using MediatR;
using System.Security.Claims;

namespace ITnetworkProjekt.Features.Insurances.Queries
{
    public class GetInsuranceForLoggedUserQuery : IRequest<InsuranceViewModel?>
    {
        public int InsuranceId { get; set; }
        public ClaimsPrincipal User { get; set; } = null!;
    }
}
