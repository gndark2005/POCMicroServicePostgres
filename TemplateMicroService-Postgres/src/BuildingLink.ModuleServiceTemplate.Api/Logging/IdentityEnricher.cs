using System.Diagnostics.CodeAnalysis;
using BuildingLink.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace BuildingLink.ModuleServiceTemplate.Api.Logging
{
    [ExcludeFromCodeCoverage]
    public class IdentityEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (!(_httpContextAccessor.HttpContext?.User.Identity.IsAuthenticated ?? false))
            {
                return;
            }

            var identityContext = new IdentityContext(_httpContextAccessor.HttpContext?.User);

            logEvent.AddOrUpdateProperty(new LogEventProperty(
                "PropertyId", new ScalarValue(identityContext.PropertyId)));

            logEvent.AddOrUpdateProperty(new LogEventProperty(
                "UserId", new ScalarValue(identityContext.UserId)));
        }
    }
}
