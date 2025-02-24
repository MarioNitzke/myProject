using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using ITnetworkProjekt.Models;
using MediatR;

namespace ITnetworkProjekt.Features.InsuredPersons.Queries
{
    public class
        GetInsuredPersonForLoggedUserQueryHandler : IRequestHandler<GetInsuredPersonForLoggedUserQuery,
        InsuredPersonViewModel?>
    {
        private readonly IInsuredPersonRepository _insuredPersonRepository;
        private readonly IMapper _mapper;

        public GetInsuredPersonForLoggedUserQueryHandler(
            IInsuredPersonRepository insuredPersonRepository,
            IMapper mapper)
        {
            _insuredPersonRepository = insuredPersonRepository;
            _mapper = mapper;
        }

        public async Task<InsuredPersonViewModel?> Handle(GetInsuredPersonForLoggedUserQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = await _insuredPersonRepository.GetInsuredPersonIdOfCurrentUserAsync(request.UserId);
            if (currentUserId != null)
            {
                var insuredPerson = await _insuredPersonRepository.FindById(currentUserId.Value);
                return _mapper.Map<InsuredPersonViewModel?>(insuredPerson);
            }

            return null;
        }
    }
}