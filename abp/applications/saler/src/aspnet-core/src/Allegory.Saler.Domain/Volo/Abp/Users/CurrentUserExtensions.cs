using System.Security.Claims;

namespace Volo.Abp.Users;

public static class CurrentUserExtensions
{
    public static int? GetClientId(this ICurrentUser currentUser)
    {
        var clientId = currentUser.FindClaim(SalerClaimTypes.ClientId)?.Value;
        return clientId == null ? null : int.Parse(clientId);
    }
}
