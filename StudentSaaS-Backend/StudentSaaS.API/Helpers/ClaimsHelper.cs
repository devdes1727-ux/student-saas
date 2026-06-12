using System.Security.Claims;

namespace StudentSaaS.API.Helpers;

public static class ClaimsHelper
{
    public static int GetInstituteId(ClaimsPrincipal user)
    {
        var value = user.FindFirst("InstituteId")?.Value;

        return int.TryParse(value, out var id) ? id : 0;
    }

    public static int GetUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(value, out var id) ? id : 0;
    }

    public static string GetRole(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value ?? "";
    }
}