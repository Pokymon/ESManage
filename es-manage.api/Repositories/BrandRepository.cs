// Tujuan: Berisi logika untuk operation database pada tabel Brand

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
    public class BrandRepository {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        public BrandRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        // Metode GetAll untuk mendapatkan semua data brand
        public async Task<IEnumerable<BrandModel>> GetAll()
        {
            try
            {
                var sql = "SELECT * FROM Brand WHERE Deleted = FALSE";
                return await _db.QueryAsync<BrandModel>(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode GetById untuk mendapatkan data brand berdasarkan ID
        public async Task<BrandModel> GetById(string id)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data brand berdasarkan ID.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM Brand WHERE Id = @Id";
                // var sql = @"SELECT * FROM Brand WHERE Id = @Id AND Deleted = false";
                return await _db.QuerySingleOrDefaultAsync<BrandModel>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode GetByName untuk mendapatkan data brand berdasarkan Name
        public async Task<BrandModel> GetByName(string name)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data brand berdasarkan Name.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM Brand WHERE Name = @Name";
                // var sql = @"SELECT * FROM Brand WHERE Name = @Name AND Deleted = false";
                return await _db.QuerySingleOrDefaultAsync<BrandModel>(sql, new { Name = name });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Create untuk menambahkan data brand
        public async Task<BrandModel> Create(BrandModel brand)
        {
            try
            {
                // Cek apakah brand dengan nama yang sama sudah ada
                var sql = @"SELECT * FROM Brand WHERE Name = @Name";
                var existingBrand = await _db.QuerySingleOrDefaultAsync<BrandModel>(sql, new { Name = brand.Name });

                if (existingBrand != null)
                {
                    // Kalau brand dengan nama yang sama sudah ada, cek apakah brand tersebut soft-delete. Jika tidak, maka throw exception
                    if (!existingBrand.Deleted)
                    {
                        throw new Exception("Sudah ada brand dengan nama yang sama.");
                    }

                    // Kalau brand dengan nama yang sama sudah ada dan soft-delete, maka update data yang sudah ada
                    existingBrand.Name = brand.Name;
                    existingBrand.Deleted = false;
                    existingBrand.CreatedBy = brand.CreatedBy;
                    existingBrand.CreatedOn = DateTime.Now;

                    sql = @"UPDATE Brand 
                        SET Name = @Name, Deleted = @Deleted, CreatedBy = @CreatedBy, CreatedOn = @CreatedOn
                        WHERE Id = @Id";

                    await _db.ExecuteAsync(sql, existingBrand);
                    return existingBrand;
                }

                // Jika tidak ada brand dengan nama yang sama, maka buat brand baru
                brand.CreatedOn = DateTime.Now;

                var maxIDSql = @"SELECT COALESCE(MAX(CAST(id AS INTEGER)), 0) FROM Brand";
                var maxID = await _db.QuerySingleAsync<int>(maxIDSql);
                brand.ID = (maxID + 1).ToString();

                sql = @"INSERT INTO Brand (Id, Name, Deleted, CreatedOn, CreatedBy)
                    VALUES (@Id, @Name, @Deleted, @CreatedOn, @CreatedBy)";

                await _db.ExecuteAsync(sql, brand);
                return brand;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Update untuk mengubah data brand
        public async Task<BrandModel> Update(string id, BrandModel brand)
        {
            try
            {
                if (id != brand.ID) {
                    throw new Exception("ID di URL tidak sama dengan ID di body request.");
                }
                else {
                    // Cek apakah item dengan ID tersebut sudah dihapus (soft-delete)
                    var deletedItem = await _db.QueryFirstOrDefaultAsync<BrandModel>(
                        "SELECT * FROM Brand WHERE Id = @Id AND Deleted != TRUE", brand);

                    if (deletedItem == null)
                    {
                        throw new Exception("Brand tidak ditemukan.");
                    }

                    brand.ModifiedOn = DateTime.Now;
                    // Syntax SQL untuk mengubah data brand
                    var updateSql = @"UPDATE Brand 
                    SET Name = @Name, Deleted = @Deleted, ModifiedOn = @ModifiedOn, ModifiedBy = @ModifiedBy 
                    WHERE Id = @Id";

                    int updatedRows = await _db.ExecuteAsync(updateSql, brand);

                    if (updatedRows == 0)
                    {
                        throw new Exception($"Tidak ada brand ditemukan dengan ID: {brand.ID}");
                    }

                    return brand;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Delete untuk menghapus data brand
        public async Task<BrandModel> Delete(string id)
        {
            try
            {
                // Cek apakah item dengan ID tersebut sudah dihapus (soft-delete)
                var deletedItem = await _db.QueryFirstOrDefaultAsync<BrandModel>(
                    "SELECT * FROM Brand WHERE Id = @Id AND Deleted != TRUE", new { Id = id });

                if (deletedItem == null)
                {
                    throw new Exception("Brand tidak ditemukan.");
                }

                var sql = @"SELECT * FROM Brand WHERE Id = @Id AND Deleted = FALSE";
                var brand = await _db.QuerySingleOrDefaultAsync<BrandModel>(sql, new { Id = id });
                brand.ModifiedOn = DateTime.Now;
                brand.Deleted = true;
                if (brand == null)
                {
                    throw new Exception($"Tidak ada brand ditemukan dengan id: {id}");
                }
                sql = @"UPDATE Brand SET Deleted = true WHERE Id = @Id";
                var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });
                if (affectedRows == 0)
                {
                    throw new Exception($"Tidak ada brand ditemukan dengan id: {id}");
                }

                return brand;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}