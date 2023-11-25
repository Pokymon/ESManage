using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using es_manage.api.Repositories;
using es_manage.api.Models;
using System.Security.Claims;
using System.Linq;

namespace es_manage.api.Controllers;

[ApiController]
[Authorize]
[Route("api/password")]
public class PasswordChangeController : ControllerBase {
    private readonly PasswordChangeRepository _passwordChangeRepository;

    public PasswordChangeController(PasswordChangeRepository passwordChangeRepository) {
        _passwordChangeRepository = passwordChangeRepository;
    }

    [HttpPost("change")]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequestModel model) {
        string currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        // Ambil role dari claim
        string roleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        string userRole = User.Claims.FirstOrDefault(c => c.Type == roleClaimType)?.Value ?? "";

        // Role 1 adalah Administrator
        bool isAdmin = userRole == "1";

        // Ambil username dari claim
        // Kemuadian bandingkan dengan username yang dikirim dari request client
        string currentUserName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

        try {
            bool result = await _passwordChangeRepository.ChangePasswordAsync(
                model.UserName,
                model.NewPassword,
                model.OldPassword,
                isAdmin && model.UserName != currentUserName // Jika user yang login adalah admin yang valid, maka password bisa diubah tanpa memvalidasi password lama
            );

            if (result) {
                return Ok(new { message = "Password berhasil diupdate." });
            } else {
                return BadRequest(new { message = "Password gagal diupdate." });
            }
        } catch (UnauthorizedAccessException) {
            return Unauthorized(new { message = "Password lama tidak sesuai." });
        } catch (Exception ex) {
            // Logging error
            // Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
            return StatusCode(500, new { message = "An error occurred.", error = ex.Message});
        }
    }
}