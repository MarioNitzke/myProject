using ITnetworkProjekt.Data.Repositories.Interfaces;
using MediatR;

namespace ITnetworkProjekt.Features.InsuredPersons.Queries
{
    public class GetPersonNameByIdQueryHandler : IRequestHandler<GetPersonNameByIdQuery, string?>
    {
        private readonly IInsuredPersonRepository _insuredPersonRepository;

        public GetPersonNameByIdQueryHandler(IInsuredPersonRepository insuredPersonRepository)
        {
            _insuredPersonRepository = insuredPersonRepository;
        }

        public async Task<string?> Handle(GetPersonNameByIdQuery request, CancellationToken cancellationToken)
        {
            var insuredPerson = await _insuredPersonRepository.FindById(request.Id);
            if (insuredPerson != null)
            {
                return $"{insuredPerson.FirstName} {insuredPerson.LastName}";
            }

            return null;
        }
    }
}