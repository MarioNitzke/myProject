using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using ITnetworkProjekt.Models;
using MediatR;

namespace ITnetworkProjekt.Features.Insurances.Queries
{
    public class GetInsurancesByIdsQueryHandler : IRequestHandler<GetInsurancesByIdsQuery, List<InsuranceViewModel>?>
    {
        private readonly IInsuranceRepository _insuranceRepository;
        private readonly IMapper _mapper;

        public GetInsurancesByIdsQueryHandler(IInsuranceRepository insuranceRepository, IMapper mapper)
        {
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
        }

        public async Task<List<InsuranceViewModel>?> Handle(GetInsurancesByIdsQuery request, CancellationToken cancellationToken)
        {
            var insurances = await _insuranceRepository.GetInsurancesByIdsAsync(request.InsuranceIds);
            return _mapper.Map<List<InsuranceViewModel>>(insurances);
        }
    }
}
