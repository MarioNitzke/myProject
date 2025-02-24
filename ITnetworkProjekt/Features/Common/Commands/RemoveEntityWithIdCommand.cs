using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using MediatR;

namespace ITnetworkProjekt.Features.Common.Commands
{
    public class RemoveEntityWithIdCommandHandler<TEntity, TViewModel>
        : IRequestHandler<RemoveEntityWithIdCommand<TEntity, TViewModel>, TViewModel>
        where TEntity : class
        where TViewModel : class
    {
        private readonly IBaseRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public RemoveEntityWithIdCommandHandler(IServiceProvider serviceProvider, IMapper mapper)
        {
            _repository = serviceProvider.GetRequiredService<IBaseRepository<TEntity>>();
            _mapper = mapper;
        }

        public async Task<TViewModel> Handle(RemoveEntityWithIdCommand<TEntity, TViewModel> request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FindById(request.Id);
            if (entity == null)
            {
                return null;
            }

            await _repository.Delete(entity);
            return _mapper.Map<TViewModel>(entity);
        }
    }
}

