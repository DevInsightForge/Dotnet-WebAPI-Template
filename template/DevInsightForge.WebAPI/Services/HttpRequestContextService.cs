using System.Security.Claims;
using DevInsightForge.Application.Abstractions.InternalServices;

namespace DevInsightForge.WebAPI.Services;

public class HttpRequestContextService(IHttpContextAccessor httpContextAccessor) : IRequestContextService
{
    private readonly Lazy<Guid?> _userId = new(() =>
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var userIdValue = principal?.FindFirstValue(ClaimTypes.Sid);

        if (userIdValue is not null && Guid.TryParse(userIdValue, out var userId))
        {
            return userId;
        }

        return null;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    public Guid? RequestUserId => _userId.Value;
}

