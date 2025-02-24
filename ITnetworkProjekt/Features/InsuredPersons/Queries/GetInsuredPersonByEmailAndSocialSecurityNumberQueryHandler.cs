using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using ITnetworkProjekt.Models;
using MediatR;

namespace ITnetworkProjekt.Features.InsuredPersons.Queries
{
    public class GetInsuredPersonByEmailAndSocialSecurityNumberQueryHandler : IRequestHandler<GetInsuredPersonByEmailAndSocialSecurityNumberQuery, InsuredPersonViewModel?>
    {
        private readonly IInsuredPersonRepository _insuredPersonRepository;
        private readonly IMapper _mapper;

        public GetInsuredPersonByEmailAndSocialSecurityNumberQueryHandler(IInsuredPersonRepository insuredPersonRepository, IMapper mapper)
        {
            _insuredPersonRepository = insuredPersonRepository;
            _mapper = mapper;
        }

        public async Task<InsuredPersonViewModel?> Handle(GetInsuredPersonByEmailAndSocialSecurityNumberQuery request, CancellationToken cancellationToken)
        {
            var insuredPerson = await _insuredPersonRepository.FindByEmailAndSocialSecurityNumberAsync(request.Email, request.SocialSecurityNumber);
            if (insuredPerson != null)
            {
                return _mapper.Map<InsuredPersonViewModel?>(insuredPerson);
            }

            return null;
        }
    }
}
