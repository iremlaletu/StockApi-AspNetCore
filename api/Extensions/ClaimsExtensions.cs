

using System.Security.Claims;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;
        }
    }
}


// Extension method for Retrieves the 'givenname' (username) from the JWT claims.
// Useful for accessing the authenticated user's name.
// user.Identity.Name may be null but "given_name" claim is always present in the JWT which is in TokenService.cs
// new Claim(JwtRegisteredClaimNames.GivenName, user.UserName) -> TokenService.cs
// Usage: User.GetUsername() in any controller to get the username of the authenticated user.
