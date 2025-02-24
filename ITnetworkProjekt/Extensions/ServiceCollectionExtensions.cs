using ITnetworkProjekt.Features.Common.Commands;
using ITnetworkProjekt.Features.Common.Queries;
using MediatR;

namespace ITnetworkProjekt.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityHandlers<T, TViewModel>(this IServiceCollection services)
            where T : class
            where TViewModel : class
        {
            services.AddTransient<IRequestHandler<FindEntityByIdQuery<T, TViewModel>, TViewModel>, FindEntityByIdQueryHandler<T, TViewModel>>();
            services.AddTransient<IRequestHandler<GetAllEntitiesQuery<T, TViewModel>, IEnumerable<TViewModel>>, GetAllEntitiesQueryHandler<T, TViewModel>>();
            services.AddTransient<IRequestHandler<AddEntityCommand<T, TViewModel>, TViewModel>, AddEntityCommandHandler<T, TViewModel>>();
            services.AddTransient<IRequestHandler<UpdateEntityCommand<T, TViewModel>, TViewModel?>, UpdateEntityCommandHandler<T, TViewModel>>();
            services.AddTransient<IRequestHandler<RemoveEntityWithIdCommand<T, TViewModel>, TViewModel>, RemoveEntityWithIdCommandHandler<T, TViewModel>>();

            return services;
        }
    }
}
