using System.Security.Claims;
using System.Security.Principal;

namespace DapperDashboard.Extensions
{
    public static class IdentityExtensions
    {
        public static int GetCustomerId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CustomerID");
            if (claim == null)
            {
                throw new System.Exception("CustomerID claim not found. User may not be authenticated properly.");
            }
            return int.Parse(claim.Value);
        }
    }
}