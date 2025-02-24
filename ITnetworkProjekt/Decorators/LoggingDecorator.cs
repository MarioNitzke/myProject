using MediatR;

namespace ITnetworkProjekt.Decorators
{
    public class LoggingDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly ILogger<LoggingDecorator<TRequest, TResponse>> _logger;

        public LoggingDecorator(IRequestHandler<TRequest, TResponse> inner, ILogger<LoggingDecorator<TRequest, TResponse>> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Zahájení zpracování {RequestName}", typeof(TRequest).Name);
            var response = await _inner.Handle(request, cancellationToken);
            _logger.LogInformation("Dokončeno zpracování {RequestName}", typeof(TRequest).Name);
            return response;
        }
    }
}
