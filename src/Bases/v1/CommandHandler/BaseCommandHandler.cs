using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Store.Framework.Core.Bases.v1.CommandHandler
{
    public abstract class BaseCommandHandler<TCommand, TResponse>
        : IRequestHandler<TCommand, TResponse>
            where TCommand : IRequest<TResponse>
    {
        public IMapper Mapper { get; private set; }

        public ILogger Logger { get; private set; }

        public IHttpContextAccessor HttpContext { get; private set; }

        protected BaseCommandHandler(ILogger logger, IMapper mapper, IHttpContextAccessor httpContext)
        {
            Mapper = mapper;
            Logger = logger;
            HttpContext = httpContext;
        }

        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
    }
}