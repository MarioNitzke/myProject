using MediatR;

namespace ITnetworkProjekt.Features.Common.Commands
{
    public class RemoveEntityWithIdCommand<TEntity, TViewModel> : IRequest<TViewModel>
        where TEntity : class
        where TViewModel : class
    {
        public int Id { get; set; }
    }
}

