using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using MediatR;

namespace ITnetworkProjekt.Features.Common.Commands
{
    public class UpdateEntityCommandHandler<TEntity, TViewModel>
        : IRequestHandler<UpdateEntityCommand<TEntity, TViewModel>, TViewModel?>
        where TEntity : class
        where TViewModel : class
    {
        private readonly IBaseRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public UpdateEntityCommandHandler(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TViewModel?> Handle(UpdateEntityCommand<TEntity, TViewModel> request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TEntity>(request.ViewModel);

            try
            {
                var updatedEntity = await _repository.Update(entity);
                return _mapper.Map<TViewModel>(updatedEntity);
            }
            catch (InvalidOperationException)
            {
                var idProperty = typeof(TEntity).GetProperty("Id");
                if (idProperty == null)
                {
                    throw new InvalidOperationException($"{typeof(TEntity).Name} must have a property named 'Id'.");
                }

                var id = idProperty.GetValue(entity);
                if (id == null)
                {
                    throw new InvalidOperationException("Entity ID cannot be null.");
                }

                var exists = await _repository.ExistsWithId((int)id);
                if (!exists)
                {
                    return null;
                }

                throw;
            }
        }
    }
}
