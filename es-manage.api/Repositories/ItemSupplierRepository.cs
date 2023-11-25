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
    public class ItemSupplierRepository {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        public ItemSupplierRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        // Metode GetAll untuk mendapatkan semua data supplier
        public async Task<IEnumerable<ItemSupplierModel>> GetAll()
        {
            try
            {
                var sql = "SELECT * FROM ItemSupplier WHERE Deleted = FALSE";
                return await _db.QueryAsync<ItemSupplierModel>(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode GetById untuk mendapatkan data supplier berdasarkan ID
        public async Task<ItemSupplierModel> GetById(string id)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data supplier berdasarkan ID.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM ItemSupplier WHERE Id = @Id";
                // var sql = @"SELECT * FROM Supplier WHERE Id = @Id AND Deleted = false";
                return await _db.QuerySingleOrDefaultAsync<ItemSupplierModel>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode GetByItemId untuk mendapatkan data supplier berdasarkan ItemId
        public async Task<IEnumerable<ItemSupplierModel>> GetByItemId(string itemId)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data supplier berdasarkan ItemId.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM ItemSupplier WHERE ItemId = @ItemId";
                // var sql = @"SELECT * FROM Supplier WHERE ItemId = @ItemId AND Deleted = false";
                return await _db.QueryAsync<ItemSupplierModel>(sql, new { ItemId = itemId });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode GetBySupplierId untuk mendapatkan data supplier berdasarkan SupplierId
        public async Task<IEnumerable<ItemSupplierModel>> GetBySupplierId(string supplierId)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data supplier berdasarkan SupplierId.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM ItemSupplier WHERE SupplierId = @SupplierId";
                // var sql = @"SELECT * FROM Supplier WHERE SupplierId = @SupplierId AND Deleted = false";
                return await _db.QueryAsync<ItemSupplierModel>(sql, new { SupplierId = supplierId });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Create untuk menambahkan data supplier baru
        public async Task<ItemSupplierModel> Create(ItemSupplierModel model)
        {
            try
            {
                // QuerySQL untuk cek apakah ada data supplier dengan ItemId dan SupplierId yang sama tapi belum dihapus (soft-delete)
                var activeSql = @"SELECT * 
                FROM ItemSupplier 
                WHERE ItemId = @ItemId 
                    AND SupplierId = @SupplierId 
                    AND Deleted = false";

                var activeRecord = await _db.QueryFirstOrDefaultAsync<ItemSupplierModel>(activeSql, model);

                // Jika ada record aktif dengan ItemId dan SupplierId yang sama
                if (activeRecord != null){
                    throw new Exception("ItemSupplier dengan ItemId dan SupplierId yang sama sudah ada");
                }

                // Query SQL untuk mengecek apakah ada record dengan ID dan CategoryName yang sama tapi sudah dihapus (soft-delete)
                var deletedSql = @"SELECT * FROM ItemSupplier
                    WHERE ItemId = @ItemId 
                        AND SupplierId = @SupplierId 
                        AND Deleted = true";
                var deletedRecord = await _db.QueryFirstOrDefaultAsync<ItemSupplierModel>(deletedSql, model);

                // Jika ada record yang sudah dihapus (soft-delete) dengan ItemId dan SupplierId yang sama
                if (deletedRecord != null) {
                    model.CreatedOn = DateTime.Now;
                    // Query SQL untuk mengupdate record yang sudah dihapus (soft-delete) dengan ItemId dan SupplierId yang sama
                    var updateSql = @"UPDATE ItemSupplier SET 
                        Deleted = false, 
                        CreatedOn = @CreatedOn, 
                        CreatedBy = @CreatedBy, 
                        ReceiptDate = @ReceiptDate, 
                        ReturnDate = @ReturnDate, 
                        ModifiedOn = null, 
                        ModifiedBy = null
                        WHERE ItemId = @ItemId 
                            AND SupplierId = @SupplierId 
                            AND Deleted = true";
                    await _db.ExecuteAsync(updateSql, model);
                }
                else // Jika tidak ada record dengan ItemId dan SupplierId yang sama
                {
                    // Cari nilai ID terbesar dari tabel ItemDepartment terus tambahkan 1
                    var maxIDSql = @"SELECT COALESCE(MAX(CAST(id AS INTEGER)), 0) FROM ItemSupplier";
                    var maxID = await _db.QuerySingleAsync<int>(maxIDSql);
                    model.ID = (maxID + 1).ToString();

                    model.CreatedOn = DateTime.Now;
                    var insertSql = @"INSERT INTO ItemSupplier 
                        (Id, ItemId, SupplierId, CreatedOn, CreatedBy, ReceiptDate, ReturnDate, ModifiedOn, ModifiedBy, Deleted) 
                        VALUES (@Id, @ItemId, @SupplierId, @CreatedOn, @CreatedBy, @ReceiptDate, @ReturnDate, @ModifiedOn, @ModifiedBy, @Deleted)";
                    await _db.ExecuteAsync(insertSql, model);
                }

                // Return the latest state of the record from the database
                return await _db.QueryFirstOrDefaultAsync<ItemSupplierModel>(activeSql, model);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Update untuk mengupdate data supplier

        public async Task<ItemSupplierModel> Update(string id, ItemSupplierModel model)
        {
            try
            {
                if (id != model.ID)
                {
                    throw new Exception("ID di URL dan body tidak sama");
                }
                else {
                    // Cek apakah item dengan ID tersebut sudah dihapus (soft-delete)
                    var deletedItem = await _db.QueryFirstOrDefaultAsync<ItemSupplierModel>(
                        "SELECT * FROM ItemSupplier WHERE Id = @Id AND Deleted != TRUE", model);

                    if (deletedItem == null)
                    {
                        throw new Exception("ItemSupplier tidak ditemukan.");
                    }

                    model.ModifiedOn = DateTime.UtcNow;
                    var updateSql = @"UPDATE ItemSupplier SET 
                        CreatedBy = @CreatedBy,
                        ModifiedOn = @ModifiedOn, 
                        ModifiedBy = @ModifiedBy, 
                        ReceiptDate = @ReceiptDate, 
                        ReturnDate = @ReturnDate,
                        Deleted = @Deleted
                        WHERE Id = @Id";

                    int updatedRows = await _db.ExecuteAsync(updateSql, model);

                    if (updatedRows == 0)
                    {
                        throw new Exception($"Tidak data yang diubah pada ID: {model.ID}");
                    }

                    return model;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Metode Delete untuk menghapus data supplier
        public async Task<ItemSupplierModel> Delete(string id)
        {
            try
            {
                // Cek apakah item dengan ID tersebut sudah dihapus (soft-delete)
                var deletedItem = await _db.QueryFirstOrDefaultAsync<ItemSupplierModel>(
                    "SELECT * FROM ItemSupplier WHERE Id = @Id AND Deleted != TRUE", new { Id = id });

                if (deletedItem == null)
                {
                    throw new Exception("ItemSupplier tidak ditemukan.");
                }

                var sql = @"SELECT * FROM ItemSupplier WHERE Id = @Id AND Deleted = FALSE";
                var itemSupplier = await _db.QuerySingleOrDefaultAsync<ItemSupplierModel>(sql, new { Id = id });
                itemSupplier.ModifiedOn = DateTime.Now;
                itemSupplier.Deleted = true;
                if (itemSupplier == null)
                {
                    throw new Exception($"Tidak ada ItemSupplier ditemukan dengan id: {id}");
                }
                sql = @"UPDATE ItemSupplier SET Deleted = true WHERE Id = @Id";
                var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });
                if (affectedRows == 0)
                {
                    throw new Exception($"Tidak ada ItemSupplier ditemukan dengan id: {id}");
                }

                return itemSupplier;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}