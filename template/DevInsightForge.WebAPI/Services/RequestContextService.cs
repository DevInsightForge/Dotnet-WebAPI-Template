using DevInsightForge.Application.Abstructions.Core;
using System.Security.Claims;

namespace DevInsightForge.WebAPI.Services;

public class RequestContextService(IHttpContextAccessor httpContextAccessor) : IRequestContextService
{
    private readonly Lazy<Guid?> _userId = new(() =>
    {
        var principal = httpContextAccessor.HttpContext?.User;
        string? userIdString = principal?.FindFirstValue(ClaimTypes.Sid);

        if (userIdString is not null && Guid.TryParse(userIdString, out var userIdGuid))
        {
            return userIdGuid;
        }

        return null;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    public Guid? RequestUserId => _userId.Value;
}


