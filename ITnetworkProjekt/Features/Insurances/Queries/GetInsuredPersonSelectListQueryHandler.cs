using ITnetworkProjekt.Data.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITnetworkProjekt.Features.Insurances.Queries
{
    public class GetInsuredPersonSelectListQueryHandler : IRequestHandler<GetInsuredPersonSelectListQuery, SelectList>
    {
        private readonly IInsuranceRepository _insuranceRepository;

        public GetInsuredPersonSelectListQueryHandler(IInsuranceRepository insuranceRepository)
        {
            _insuranceRepository = insuranceRepository;
        }

        public async Task<SelectList> Handle(GetInsuredPersonSelectListQuery request, CancellationToken cancellationToken)
        {
            var insuredPersons = await _insuranceRepository.GetInsuredPersonsAsync(request.SelectedId);

            var list = insuredPersons
                .Select(p => new
                {
                    p.Id,
                    FullName = $"{p.SocialSecurityNumber} {p.LastName} {p.FirstName}"
                })
                .ToList();

            return new SelectList(list, "Id", "FullName", request.SelectedId);
        }
    }
}
