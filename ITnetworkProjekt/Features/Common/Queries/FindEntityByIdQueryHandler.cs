using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using MediatR;

namespace ITnetworkProjekt.Features.Common.Queries
{
    public class FindEntityByIdQueryHandler<TEntity, TViewModel>
    : IRequestHandler<FindEntityByIdQuery<TEntity, TViewModel>, TViewModel>
        where TEntity : class
        where TViewModel : class
    {
        private readonly IBaseRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public FindEntityByIdQueryHandler(IServiceProvider serviceProvider, IMapper mapper)
        {

            _repository = serviceProvider.GetRequiredService<IBaseRepository<TEntity>>();
            _mapper = mapper;
        }

        public async Task<TViewModel> Handle(FindEntityByIdQuery<TEntity, TViewModel> request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FindById(request.Id);
            return _mapper.Map<TViewModel>(entity);
        }
    }
}