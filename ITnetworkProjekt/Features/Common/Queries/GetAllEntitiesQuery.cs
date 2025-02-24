using MediatR;

namespace ITnetworkProjekt.Features.Common.Queries
{
    public class GetAllEntitiesQuery<TEntity, TViewModel> : IRequest<IEnumerable<TViewModel>>
        where TEntity : class
        where TViewModel : class
    {
    }
}