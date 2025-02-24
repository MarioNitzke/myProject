using ITnetworkProjekt.Models;
using MediatR;

namespace ITnetworkProjekt.Features.Insurances.Queries
{
    public class GetInsurancesByIdsQuery : IRequest<List<InsuranceViewModel>?>
    {
        public List<int> InsuranceIds { get; }

        public GetInsurancesByIdsQuery(List<int> insuranceIds)
        {
            InsuranceIds = insuranceIds;
        }
    }
}
