using MediatR;

namespace ITnetworkProjekt.Features.Common.Queries
{
    public class FindEntityByIdQuery<TEntity, TViewModel> : IRequest<TViewModel>
        where TEntity : class
        where TViewModel : class
    {
        public int Id { get; set; }
    }
}