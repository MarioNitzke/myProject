using MediatR;

namespace ITnetworkProjekt.Features.Common.Commands
{
    public class AddEntityCommand<TEntity, TViewModel> : IRequest<TViewModel>
            where TEntity : class
            where TViewModel : class
    {
        public TViewModel ViewModel { get; }

        public AddEntityCommand(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
