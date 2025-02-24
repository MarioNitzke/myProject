using MediatR;

namespace ITnetworkProjekt.Features.Common.Commands
{
    public class UpdateEntityCommand<TEntity, TViewModel> : IRequest<TViewModel?>
        where TEntity : class
        where TViewModel : class
    {
        public TViewModel ViewModel { get; }

        public UpdateEntityCommand(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
