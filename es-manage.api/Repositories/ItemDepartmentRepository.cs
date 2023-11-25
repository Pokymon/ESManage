// Tujuan: Berisi logika untuk operasi database pada tabel ItemDepartment

// Import library yang dibutuhkan
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using es_manage.api.Context;
using es_manage.api.Models;
using es_manage.api.Utilities;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace es_manage.api.Repositories {
    public class ItemDepartmentRepository
    {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        public ItemDepartmentRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        // Metode GetAll untuk mendapatkan semua data department
        public async Task<IEnumerable<ItemDepartmentModel>> GetAll()
        {
            try
            {
                var sql = "SELECT * FROM ItemDepartment WHERE Deleted = FALSE";
                return await _db.QueryAsync<ItemDepartmentModel>(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception("Kesalahan saat mendapatkan semua department", ex);
            }
        }

        // Metode GetById untuk mendapatkan data department berdasarkan ID
        public async Task<IEnumerable<ItemDepartmentModel>> GetById(string id)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data department berdasarkan ID.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM ItemDepartment WHERE Id = @Id";
                // var sql = @"SELECT * FROM ItemDepartment WHERE Id = @Id AND Deleted = false";
                return await _db.QueryAsync<ItemDepartmentModel>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception($"Error mencari Item Department id: {id}", ex);
            }
        }

        // Metode GetByCategory untuk mendapatkan data department berdasarkan CategoryName
        public async Task<IEnumerable<ItemDepartmentModel>> GetByCategory(string categoryName)
        {
            // Syntax SQL untuk mendapatkan data department berdasarkan CategoryName.
            //  Entry yang dihapus (soft-delete) tetap diambil
            var sql = @"SELECT * FROM ItemDepartment WHERE CategoryName = @CategoryName";
            //var sql = @"SELECT * FROM ItemDepartment WHERE CategoryName = @CategoryName AND Deleted = false";
            return await _db.QueryAsync<ItemDepartmentModel>(sql, new { CategoryName = categoryName });
        }

        // Metode GetByCategoryAndId untuk mendapatkan data department berdasarkan ID dan CategoryName
        public async Task<ItemDepartmentModel> GetByCategoryAndId(string id, string categoryName)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data department berdasarkan ID dan CategoryName.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM ItemDepartment WHERE Id = @Id AND CategoryName = @CategoryName";
                var department = await _db.QuerySingleOrDefaultAsync<ItemDepartmentModel>(sql, new { Id = id, CategoryName = categoryName });
                return department;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Create untuk membuat entry data department baru
        public async Task<ItemDepartmentModel> Create(ItemDepartmentModel department)
        {
            try {
                // Query SQL untuk mengecek apakah ada record dengan ID dan CategoryName yang sama tapi belum dihapus (soft-delete)
                var activeSql = @"SELECT * 
                    FROM ItemDepartment 
                    WHERE Id = @Id 
                        AND CategoryName = @CategoryName 
                        AND Deleted = false";
                var activeRecord = await _db.QueryFirstOrDefaultAsync<ItemDepartmentModel>(activeSql, department);

                // Jika ada record yang aktif dengan ID dan CategoryName yang sama
                if (activeRecord != null)
                {
                    throw new Exception("Sudah ada item department dengan ID dan CategoryName yang sama.");
                }

                // Query SQL untuk mengecek apakah ada record dengan ID dan CategoryName yang sama tapi sudah dihapus (soft-delete)
                var deletedSql = @"SELECT * FROM ItemDepartment
                        WHERE Id = @Id
                        AND CategoryName = @CategoryName
                        AND Deleted = true";
                var deletedRecord = await _db.QueryFirstOrDefaultAsync<ItemDepartmentModel>(deletedSql, department);

                // Jika ada record yang dihapus dengan ID dan CategoryName yang sama
                if (deletedRecord != null)
                {
                    department.CreatedOn = DateTime.UtcNow;
                    // Update entry yang sudah ada
                    var updateSql = @"
                        UPDATE ItemDepartment
                        SET Deleted = false,
                            CreatedBy = @CreatedBy,
                            CreatedOn = @CreatedOn,
                            ItemDepartmentParentId = @ItemDepartmentParentId,
                            ModifiedOn = NULL,
                            ModifiedBy = NULL
                        WHERE Id = @Id AND CategoryName = @CategoryName";
                    await _db.ExecuteAsync(updateSql, department);
                }
                else  // Jika tidak ada record dengan ID dan CategoryName yang sama
                {
                    // Jika ID kosong, maka buat ID baru
                    if (string.IsNullOrEmpty(department.ID)) {
                        // Cari nilai ID terbesar dari tabel ItemDepartment terus tambahkan 1
                        var maxIDSql = @"SELECT COALESCE(MAX(CAST(id AS INTEGER)), 0) FROM ItemDepartment";
                        var maxID = await _db.QuerySingleAsync<int>(maxIDSql);
                        department.ID = (maxID + 1).ToString();
                    }

                    department.CreatedOn = DateTime.UtcNow;
                    // Insert a new record
                    var insertSql = @"INSERT INTO ItemDepartment (Id, CategoryName, ItemDepartmentParentId, CreatedOn, CreatedBy)
                            VALUES (@Id, @CategoryName, @ItemDepartmentParentId, @CreatedOn, @CreatedBy)";
                    await _db.ExecuteAsync(insertSql, department);
                }

                // Return the latest state of the record from the database
                return await _db.QueryFirstOrDefaultAsync<ItemDepartmentModel>(activeSql, department);
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> Update(ItemDepartmentModel department)
        {
            try
            {
                department.ModifiedOn = DateTime.UtcNow;
                var sql = @"UPDATE ItemDepartment SET ItemDepartmentParentId = @ItemDepartmentParentId,
                            Deleted = @Deleted, ModifiedOn = @ModifiedOn, ModifiedBy = @ModifiedBy
                            WHERE Id = @Id AND CategoryName = @CategoryName";

                var rowsAffected = await _db.ExecuteAsync(sql, department);
                if (rowsAffected > 0)
                {
                    return rowsAffected;
                }
                else
                {
                    throw new Exception("Tidak ada data yang diubah");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> Delete(string id, string categoryName)
        {
            try
            {
                var sql = @"UPDATE ItemDepartment SET Deleted = TRUE WHERE Id = @Id AND CategoryName = @CategoryName";

                var rowsAffected = await _db.ExecuteAsync(sql, new { Id = id, CategoryName = categoryName });
                if (rowsAffected > 0)
                {
                    return rowsAffected;
                }
                else
                {
                    throw new Exception("Tidak ada data yang dihapus");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}