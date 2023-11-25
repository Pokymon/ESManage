using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace es_manage.api.Services
{
    public class RoleClaimService : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var roleClaim = principal.FindFirst(ClaimTypes.Role);
            if (roleClaim != null && roleClaim.Value == "1")
            {
                // Hapus claim role "1"
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                claimsIdentity.RemoveClaim(roleClaim);

                // Ganti dengan claim role "Administrator"
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            }

            return Task.FromResult(principal);
            }
    }
}