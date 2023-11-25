// Tujuan: Berisi logika untuk operation database pada tabel UserMst

// Import library yang dibutuhkan
using es_manage.api.Context;
using es_manage.api.Models;
using es_manage.api.Utilities;
using es_manage.api.Services;
using System.Data;
using Dapper;
using Npgsql;

// Membuat namespace
namespace es_manage.api.Repositories {
    // Membuat class UserRepository
    public class UserRepository
    {
        // Membuat private readonly AppDbContext dan IDbConnection
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        // Membuat constructor UserRepository yang menerima parameter AppDbContext dan IConfiguration
        public UserRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        // Membuat metode GetAll untuk mengambil semua data user
        public async Task<IEnumerable<UserMst>> GetAll()
        {
            try
            {
                var sql = "SELECT * FROM UserMst WHERE DeletedAt IS NULL";
                return await _db.QueryAsync<UserMst>(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                var pesanError = "Kesalahan saat mendapatkan semua user, " + ex.Message;
                throw new Exception(pesanError, ex);
            }
        }

        // Membuat metode Get untuk mengambil data user berdasarkan ID berupa UUID
        public async Task<UserMst> Get(Guid id)
        {
            try
            {
                var sql = "SELECT * FROM UserMst WHERE ID = @ID AND DeletedAt IS NULL";
                return await _db.QuerySingleOrDefaultAsync<UserMst>(sql, new { ID = id });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                var pesanError = "Kesalahan saat mendapatkan user, " + ex.Message;
                throw new Exception(pesanError, ex);
            }
        }

        public async Task<UserMst> Create(UserMst user)
        {
            try
            {
                // Check if the username is already taken
                var existingUser = await _db.QueryFirstOrDefaultAsync<UserMst>(
                    "SELECT * FROM UserMst WHERE UserName = @UserName AND DeletedAt IS NULL", 
                    new { user.UserName });

                if (existingUser != null)
                {
                    throw new InvalidOperationException("Username sudah diambil. Gunakan username lain.");
                }

                user.ID = Guid.NewGuid();
                user.CreatedOn = DateTime.UtcNow;

                // Use HashingService to hash the password
                HashingService hashingService = new HashingService();
                string salt;
                user.Password = hashingService.HashPassword(user.Password, out salt);
                user.PasswordSalt = salt;

                // The INSERT statement and the rest of your code...
                // Note that you should add a new field in the INSERT statement for PasswordSalt
                var sql = @"INSERT INTO UserMst (ID, UserName, DisplayName, Password, PasswordSalt, RoleID, CreatedOn, CreatedBy)
                            VALUES (@ID, @UserName, @DisplayName, @Password, @PasswordSalt, @RoleID, @CreatedOn, @CreatedBy)
                            RETURNING *";
                return await _db.QuerySingleAsync<UserMst>(sql, user);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                var pesanError = "Kesalahan saat menambahkan user, " + ex.Message;
                throw new Exception(pesanError, ex);
            }
        }

        // Membuat metode Update untuk mengubah data user berdasarkan ID berupa UUID
        public async Task<UserMst> Update(Guid id, UserMst user)
        {
            try
            {
                user.ModifiedOn = DateTime.UtcNow;
                var sql = @"UPDATE UserMst
                            SET UserName = @UserName,
                                DisplayName = @DisplayName,
                                RoleID = @RoleID,
                                ModifiedOn = @ModifiedOn,
                                ModifiedBy = @ModifiedBy
                            WHERE ID = @ID AND DeletedAt IS NULL
                            RETURNING *";
                return await _db.QuerySingleAsync<UserMst>(sql, user);
            }
            catch (Exception ex)
            {
                var pesanError = "Kesalahan saat mengupdate user, " + ex.Message;
                throw new Exception(pesanError, ex);
            }
        }

        // Membuat metode Delete (soft-delete) untuk menghapus data user berdasarkan ID berupa UUID
        public async Task Delete(Guid id)
        {
            try
            {
                var sql = "UPDATE UserMst SET DeletedAt = NOW() WHERE ID = @ID";
                await _db.ExecuteAsync(sql, new { ID = id });
            }
            catch (Exception ex)
            {
                var pesanError = "Kesalahan saat menghapus user, " + ex.Message;
                throw new Exception(pesanError, ex);
            }
        }

        // Membuat metode ValidateUser untuk validasi user saat login
        /*public async Task<UserMst?> ValidateUser(string username, string password) {
            var user = await _db.QuerySingleOrDefaultAsync<UserMst>("SELECT * FROM UserMst WHERE UserName = @UserName AND Password = @Password AND DeletedAt IS NULL", new { UserName = username, Password = password });
            return user;
        }*/
    }
}