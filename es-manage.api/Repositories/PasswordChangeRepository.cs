using es_manage.api.Context;
using es_manage.api.Models;
using es_manage.api.Services;
using System.Data;
using Dapper;
using Npgsql;

namespace es_manage.api.Repositories {
    public class PasswordChangeRepository {
        private readonly IDbConnection _db;
        private readonly HashingService _hashingService;
        private readonly AuthRepository _authRepository;

        public PasswordChangeRepository(IConfiguration configuration, HashingService hashingService, AuthRepository authRepository) {
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
            _hashingService = hashingService;
            _authRepository = authRepository;
        }

        public async Task<bool> ChangePasswordAsync(string username, string newPassword, string? oldPassword = null, bool isAdmin = false) {
            var user = await _authRepository.SearchUsername(username);
            if (user == null) {
                return false;
            }

            if (!isAdmin) {
                if (string.IsNullOrEmpty(oldPassword) || !_hashingService.VerifyPassword(oldPassword, user.Password, user.PasswordSalt)) {
                    throw new UnauthorizedAccessException("Password lama tidak sesuai");
                };
            }

            string newSalt;
            string newHashedPassword = _hashingService.HashPassword(newPassword, out newSalt);
            user.Password = newHashedPassword;
            user.PasswordSalt = newSalt;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = isAdmin ? "Admin" : username; // Jika user yang login adalah admin, maka modified by adalah admin, jika bukan, maka modified by adalah user yang login

            string updateQuery = @"UPDATE public.usermst SET password = @Password,
            passwordsalt = @PasswordSalt,
            modifiedon = @ModifiedOn,
            modifiedby = @ModifiedBy
            WHERE username = @UserName";

            int rowsUpdated = await _db.ExecuteAsync(updateQuery, user);

            return rowsUpdated > 0;
        }
    }
}