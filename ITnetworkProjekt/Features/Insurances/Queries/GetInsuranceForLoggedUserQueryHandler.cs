using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using ITnetworkProjekt.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ITnetworkProjekt.Features.Insurances.Queries
{
    public class GetInsuranceForLoggedUserQueryHandler
            : IRequestHandler<GetInsuranceForLoggedUserQuery, InsuranceViewModel?>
    {
        private readonly IInsuranceRepository _insuranceRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public GetInsuranceForLoggedUserQueryHandler(
            IInsuranceRepository insuranceRepository,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<InsuranceViewModel?> Handle(
            GetInsuranceForLoggedUserQuery query,
            CancellationToken cancellationToken)
        {
            if (query.User.IsInRole(UserRoles.Admin))
            {
                // Admin vidí cokoliv
                var adminInsurance = await _insuranceRepository.FindById(query.InsuranceId);
                return _mapper.Map<InsuranceViewModel?>(adminInsurance);
            }

            int? currentUserId =
                await _insuranceRepository.GetInsuredPersonIdOfCurrentUserAsync(
                    _userManager.GetUserId(query.User));

            if (currentUserId == null)
            {
                return null;
            }

            var insurance = await _insuranceRepository.FindById(query.InsuranceId);
            if (insurance != null && insurance.InsuredPersonId == currentUserId)
            {
                return _mapper.Map<InsuranceViewModel?>(insurance);
            }
            else
            {
                return null;
            }
        }
    }
}
