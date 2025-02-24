using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using MediatR;

namespace ITnetworkProjekt.Features.Common.Commands
{
    public class AddEntityCommandHandler<TEntity, TViewModel>
    : IRequestHandler<AddEntityCommand<TEntity, TViewModel>, TViewModel>
        where TEntity : class
        where TViewModel : class
    {
        private readonly IBaseRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public AddEntityCommandHandler(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TViewModel> Handle(AddEntityCommand<TEntity, TViewModel> request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TEntity>(request.ViewModel);    
            var addedEntity = await _repository.Insert(entity);   
            return _mapper.Map<TViewModel>(addedEntity);       
        }
    }
}
