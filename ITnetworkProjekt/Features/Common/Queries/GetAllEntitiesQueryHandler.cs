using AutoMapper;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using MediatR;

namespace ITnetworkProjekt.Features.Common.Queries
{
    public class GetAllEntitiesQueryHandler<TEntity, TViewModel>
    : IRequestHandler<GetAllEntitiesQuery<TEntity, TViewModel>, IEnumerable<TViewModel>>
    where TEntity : class
    where TViewModel : class
{
    private readonly IBaseRepository<TEntity> _repository;
    private readonly IMapper _mapper;

    public GetAllEntitiesQueryHandler(IBaseRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TViewModel>> Handle(GetAllEntitiesQuery<TEntity, TViewModel> request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAll();
        return _mapper.Map<IEnumerable<TViewModel>>(entities);
    }
}


    }
