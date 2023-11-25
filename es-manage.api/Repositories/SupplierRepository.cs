// Tujuan: Berisi logika untuk operation database pada tabel Supplier

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
    public class SupplierRepository {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        public SupplierRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        // Metode GetAll untuk mendapatkan semua data supplier
        public async Task<IEnumerable<SupplierModel>> GetAll()
        {
            try
            {
                var sql = "SELECT * FROM Supplier WHERE Deleted = FALSE";
                return await _db.QueryAsync<SupplierModel>(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode GetById untuk mendapatkan data supplier berdasarkan ID
        public async Task<SupplierModel> GetById(string id)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data supplier berdasarkan ID.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM Supplier WHERE Id = @Id";
                // var sql = @"SELECT * FROM Supplier WHERE Id = @Id AND Deleted = false";
                return await _db.QuerySingleOrDefaultAsync<SupplierModel>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode GetByName untuk mendapatkan data supplier berdasarkan Name
        public async Task<SupplierModel> GetByName(string name)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data supplier berdasarkan Name.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM Supplier WHERE supplierName = @supplierName";
                // var sql = @"SELECT * FROM Supplier WHERE supplierName = @supplierName AND Deleted = false";
                return await _db.QuerySingleOrDefaultAsync<SupplierModel>(sql, new { supplierName = name });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Create untuk menambahkan data supplier
        public async Task<SupplierModel> Create(SupplierModel supplier)
        {
            try
            {
                // Cek apakah supplier dengan nama yang sama sudah ada
                var sql = @"SELECT * FROM Supplier WHERE supplierName = @supplierName";
                var existingSupplier = await _db.QuerySingleOrDefaultAsync<SupplierModel>(sql, new { supplierName = supplier.SupplierName });

                if (existingSupplier != null)
                {
                    // Kalau supplier dengan nama yang sama sudah ada, cek apakah supplier tersebut soft-delete. Jika tidak, maka throw exception
                    if (!existingSupplier.Deleted)
                    {
                        throw new Exception("Sudah ada supplier dengan nama yang sama.");
                    }

                    // Kalau supplier dengan nama yang sama sudah ada dan soft-delete, maka update data yang sudah ada
                    existingSupplier.SupplierName = supplier.SupplierName;
                    existingSupplier.Deleted = false;
                    existingSupplier.CreatedBy = supplier.CreatedBy;
                    existingSupplier.CreatedOn = DateTime.Now;

                    sql = @"UPDATE Supplier 
                        SET supplierName = @supplierName, Deleted = @Deleted, CreatedBy = @CreatedBy, CreatedOn = @CreatedOn
                        WHERE Id = @Id";

                    await _db.ExecuteAsync(sql, existingSupplier);
                    return existingSupplier;
                }

                // Jika tidak ada supplier dengan nama yang sama, maka buat supplier baru
                supplier.CreatedOn = DateTime.Now;

                var maxIDSql = @"SELECT COALESCE(MAX(CAST(id AS INTEGER)), 0) FROM Supplier";
                var maxID = await _db.QuerySingleAsync<int>(maxIDSql);
                supplier.ID = (maxID + 1).ToString();

                sql = @"INSERT INTO Supplier (ID, supplierName, Deleted, CreatedOn, CreatedBy)
                    VALUES (@ID, @supplierName, @Deleted, @CreatedOn, @CreatedBy)";

                await _db.ExecuteAsync(sql, supplier);
                return supplier;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Update untuk mengubah data supplier
        public async Task<SupplierModel> Update(string id, SupplierModel supplier)
        {
            try
            {
                if (id != supplier.ID) {
                    throw new Exception("ID di URL tidak sama dengan ID di body request.");
                }
                else {
                    // Cek apakah item dengan ID tersebut sudah dihapus (soft-delete)
                    var deletedItem = await _db.QueryFirstOrDefaultAsync<SupplierModel>(
                        "SELECT * FROM Supplier WHERE Id = @Id AND Deleted != TRUE", supplier);

                    if (deletedItem == null)
                    {
                        throw new Exception("Supplier tidak ditemukan.");
                    }

                    supplier.ModifiedOn = DateTime.Now;
                    // Syntax SQL untuk mengubah data supplier
                    var updateSql = @"UPDATE Supplier 
                    SET supplierName = @supplierName, Deleted = @Deleted, ModifiedOn = @ModifiedOn, ModifiedBy = @ModifiedBy 
                    WHERE Id = @Id";

                    int updatedRows = await _db.ExecuteAsync(updateSql, supplier);

                    if (updatedRows == 0)
                    {
                        throw new Exception($"Tidak ada supplier ditemukan dengan ID: {supplier.ID}");
                    }

                    return supplier;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Delete untuk menghapus data supplier
        public async Task<SupplierModel> Delete(string id)
        {
            try
            {
                // Cek apakah item dengan ID tersebut sudah dihapus (soft-delete)
                var deletedItem = await _db.QueryFirstOrDefaultAsync<SupplierModel>(
                    "SELECT * FROM Supplier WHERE Id = @Id AND Deleted != TRUE", new { Id = id });

                if (deletedItem == null)
                {
                    throw new Exception("Supplier tidak ditemukan.");
                }

                var sql = @"SELECT * FROM Supplier WHERE Id = @Id AND Deleted = FALSE";
                var supplier = await _db.QuerySingleOrDefaultAsync<SupplierModel>(sql, new { Id = id });
                supplier.ModifiedOn = DateTime.Now;
                supplier.Deleted = true;
                if (supplier == null)
                {
                    throw new Exception($"Tidak ada supplier ditemukan dengan id: {id}");
                }
                sql = @"UPDATE Supplier SET Deleted = true WHERE Id = @Id";
                var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });
                if (affectedRows == 0)
                {
                    throw new Exception($"Tidak ada supplier ditemukan dengan id: {id}");
                }

                return supplier;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}