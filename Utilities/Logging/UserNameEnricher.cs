using Serilog.Core;
using Serilog.Events;

namespace Utilities.Logging;

public class UserNameEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserNameEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        var userNameProperty = new LogEventProperty("UserName", new ScalarValue(userName));
        logEvent.AddPropertyIfAbsent(userNameProperty);
    }
}