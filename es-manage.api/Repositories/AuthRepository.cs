using es_manage.api.Context;
using es_manage.api.Models;
using es_manage.api.Services;
using System.Data;
using Dapper;
using Npgsql;

namespace es_manage.api.Repositories {
    public class AuthRepository {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;
        private readonly HashingService _hashingService; // Add HashingService

        public AuthRepository(AppDbContext context, IConfiguration configuration, HashingService hashingService) // Inject HashingService
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
            _hashingService = hashingService; // Assign to local variable
        }

        // Carikan user berdasarkan username
        public async Task<UserMst?> SearchUsername(string username) {
        // Ambil data user berdasarkan username tanpa memvalidasi password. (Yang penting username-nya ada dan tidak dihapus)
        var user = await _db.QuerySingleOrDefaultAsync<UserMst>(
            "SELECT * FROM UserMst WHERE UserName = @UserName AND DeletedAt IS NULL", 
            new { UserName = username });
        return user;
        }

        public bool ValidatePassword(UserMst user, string password) {
            // Pakai HashingService untuk memvalidasi password
            return _hashingService.VerifyPassword(password, user.Password, user.PasswordSalt);
        }

        public async Task<UserMst?> ValidateUser(string username, string password) {
            // Carikan user berdasarkan username tanpa memvalidasi password
            var user = await _db.QuerySingleOrDefaultAsync<UserMst>("SELECT * FROM UserMst WHERE UserName = @UserName AND DeletedAt IS NULL", new { UserName = username });

            // Verifikasi password hanya jika user ditemukan
            if (user != null && _hashingService.VerifyPassword(password, user.Password, user.PasswordSalt)) {
                return user; // User valid
            }

            return null; // User invalid
        }
    }
}