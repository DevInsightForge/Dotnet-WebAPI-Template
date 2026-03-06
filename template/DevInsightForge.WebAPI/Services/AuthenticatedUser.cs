using DevInsightForge.Application.Common.Interfaces.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DevInsightForge.WebAPI.Services;

public class AuthenticatedUser(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUser
{
    public Guid? UserId
    {
        get
        {
            string? userIdString = httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sid);

            if (userIdString is not null && Guid.TryParse(userIdString, out var userIdGuid))
            {
                return userIdGuid;
            }

            return null;
        }
    }
}
