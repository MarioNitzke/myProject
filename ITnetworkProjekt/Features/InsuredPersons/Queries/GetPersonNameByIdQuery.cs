using MediatR;

namespace ITnetworkProjekt.Features.InsuredPersons.Queries
{
    public class GetPersonNameByIdQuery : IRequest<string?>
    {
        public int Id { get; }

        public GetPersonNameByIdQuery(int id)
        {
            Id = id;
        }
    }
}