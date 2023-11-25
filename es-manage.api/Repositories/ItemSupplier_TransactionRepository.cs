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
    public class ItemSupplier_TransactionRepository {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        public ItemSupplier_TransactionRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        public enum TransactionType
        {
            pembelian,
            penerimaan,
            pengembalian,
            pengiriman
        }

        // Metode GetAll untuk mendapatkan semua data ItemSupplier_Transaction
        public async Task<IEnumerable<ItemSupplier_TransactionModel>> GetAll()
        {
            try
            {
                var sql = @"SELECT * FROM itemsupplier_transaction WHERE Deleted = FALSE";
                return await _db.QueryAsync<ItemSupplier_TransactionModel>(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Method to get a transaction by ID
        public async Task<ItemSupplier_TransactionModel> GetById(string id)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data supplier berdasarkan ID.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM itemsupplier_transaction WHERE Id = @Id";
                // var sql = @"SELECT * FROM itemsupplier_transaction WHERE Id = @Id AND Deleted = false";
                return await _db.QuerySingleOrDefaultAsync<ItemSupplier_TransactionModel>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Method to get transactions by ItemSupplierId
        public async Task<IEnumerable<ItemSupplier_TransactionModel>> GetByItemSupplierId(string itemSupplierId)
        {
            try
            {
                // Syntax SQL untuk mendapatkan data supplier berdasarkan ItemSupplierId.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM itemsupplier_transaction WHERE itemsupplierid = @ItemSupplierId";
                //var sql = @"SELECT * FROM itemsupplier_transaction WHERE itemsupplierid = @ItemSupplierId AND Deleted = FALSE";
                return await _db.QueryAsync<ItemSupplier_TransactionModel>(sql, new { ItemSupplierId = itemSupplierId });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Method to get transactions by TransactionType
        public async Task<IEnumerable<ItemSupplier_TransactionModel>> GetByTransactionType(string transactionType)
        {
            try
            {
                // Konversi string ke enum TransactionType
                if (!Enum.TryParse(transactionType, true, out TransactionType type))
                {
                    throw new ArgumentException($"TransactionType: {transactionType} tidak valid");
                }
                // Validasi TransactionType
                if (type != TransactionType.pembelian &&
                    type != TransactionType.penerimaan &&
                    type != TransactionType.pengembalian &&
                    type != TransactionType.pengiriman)
                {
                    throw new Exception("TransactionType tidak valid");
                }

                // Syntax SQL untuk mendapatkan data supplier berdasarkan TransactionType.
                //  Entry yang dihapus (soft-delete) tetap diambil
                var sql = @"SELECT * FROM itemsupplier_transaction WHERE transactiontype = @TransactionType";
                //var sql = @"SELECT * FROM itemsupplier_transaction WHERE transactiontype = @TransactionType AND Deleted = FALSE";
                return await _db.QueryAsync<ItemSupplier_TransactionModel>(sql, new { TransactionType = type.ToString() });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Method to create a new transaction
        public async Task<ItemSupplier_TransactionModel> Create( ItemSupplier_TransactionPlusItemSupplier transaction)
        {
            try
            {
                // Query SQL untuk mengecek apakah ada transaksi dengan ID yang sama dan sudah dihapus (soft-delete)
                var sqlCheck = @"SELECT * FROM itemsupplier_transaction WHERE id = @Id AND deleted = TRUE";
                var deletedRecord = await _db.QuerySingleOrDefaultAsync<ItemSupplier_TransactionModel>(sqlCheck, transaction);

                // Jika ada record yang sudah dihapus (soft-delete) dengan id yang sama, maka record tersebut akan diupdate
                if (deletedRecord != null)
                {
                    // Validasi TransactionType
                    if (transaction.TransactionType != TransactionType.pembelian.ToString() &&
                        transaction.TransactionType != TransactionType.penerimaan.ToString() &&
                        transaction.TransactionType != TransactionType.pengembalian.ToString() &&
                        transaction.TransactionType != TransactionType.pengiriman.ToString())
                    {
                        throw new Exception("TransactionType tidak valid");
                    }

                    // Mendapatkan ItemSupplier Id
                    var sqlFindItemSupplier = @"SELECT id FROM itemsupplier WHERE (itemid = @ItemId AND supplierid = @SupplierId)";
                    var supplierId = await _db.QuerySingleAsync<string>(sqlFindItemSupplier, transaction);
                    // Query SQL untuk mengupdate record yang sudah dihapus (soft-delete)
                    ItemSupplier_TransactionModel newTransaction = new ItemSupplier_TransactionModel
                    {
                        ID = transaction.ID,
                        ItemSupplierId = supplierId.ToString(),
                        TransactionType = transaction.TransactionType,
                        TransactionDate = transaction.TransactionDate,
                        Quantity = transaction.Quantity,
                        Notes = transaction.Notes,
                        Deleted = transaction.Deleted,
                        CreatedOn = DateTime.Now,
                        CreatedBy = transaction.CreatedBy,
                        ModifiedOn = transaction.ModifiedOn,
                        ModifiedBy = transaction.ModifiedBy
                    };
                    var sqlUpdate = @"UPDATE itemsupplier_transaction SET itemsupplierid = @ItemSupplierId, 
                    transactiontype = @TransactionType, 
                    transactiondate = @TransactionDate, 
                    quantity = @Quantity, 
                    notes = @Notes, 
                    deleted = false, 
                    createdon = @CreatedOn, 
                    createdby = @CreatedBy, 
                    modifiedon = null, 
                    modifiedby = null
                    WHERE id = @Id";
                    await _db.ExecuteAsync(sqlUpdate, newTransaction);
                    return newTransaction;
                }
                else {
                    // Validasi TransactionType
                    if (transaction.TransactionType != TransactionType.pembelian.ToString() &&
                        transaction.TransactionType != TransactionType.penerimaan.ToString() &&
                        transaction.TransactionType != TransactionType.pengembalian.ToString() &&
                        transaction.TransactionType != TransactionType.pengiriman.ToString())
                    {
                        throw new Exception("TransactionType tidak valid");
                    }

                    // Cari nilai ID terbesar dari tabel ItemSupplier_transactiom terus tambahkan 1
                    var maxIDSql = @"SELECT COALESCE(MAX(CAST(id AS INTEGER)), 0) FROM ItemSupplier_Transaction";
                    var maxID = await _db.QuerySingleAsync<int>(maxIDSql);
                    transaction.ID = (maxID + 1).ToString();

                    var sqlFindItemSupplier = @"SELECT id FROM itemsupplier WHERE (itemid = @ItemId AND supplierid = @SupplierId)";
                    var supplierId = await _db.QuerySingleAsync<string>(sqlFindItemSupplier, transaction);
                    ItemSupplier_TransactionModel newTransaction = new ItemSupplier_TransactionModel
                    {
                        ID = transaction.ID,
                        ItemSupplierId = supplierId.ToString(),
                        TransactionType = transaction.TransactionType,
                        TransactionDate = transaction.TransactionDate,
                        Quantity = transaction.Quantity,
                        Notes = transaction.Notes,
                        Deleted = transaction.Deleted,
                        CreatedOn = DateTime.Now,
                        CreatedBy = transaction.CreatedBy,
                        ModifiedOn = transaction.ModifiedOn,
                        ModifiedBy = transaction.ModifiedBy
                    };
                    var insertSql = @"INSERT INTO itemsupplier_transaction (id, itemsupplierid, transactiontype, transactiondate, quantity, notes, deleted, createdon, createdby, modifiedon, modifiedby)
                    VALUES (@Id, @ItemSupplierId, @TransactionType, @TransactionDate, @Quantity, @Notes, false, @CreatedOn, @CreatedBy, null, null)";

                    await _db.ExecuteAsync(insertSql, newTransaction);
                }

                // Return the latest state of the record from the database
                return await _db.QueryFirstOrDefaultAsync<ItemSupplier_TransactionModel>("SELECT * FROM itemsupplier_transaction WHERE id = @Id", transaction);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Method to update a transaction
        public async Task<ItemSupplier_TransactionModel> Update(string id, ItemSupplier_TransactionModel transaction)
        {
            try
            {
                if (id != transaction.ID)
                {
                    throw new Exception("ID di URL dan Request tidak sama");
                }
                else {
                    // Validasi TransactionType
                    if (transaction.TransactionType != TransactionType.pembelian.ToString() &&
                        transaction.TransactionType != TransactionType.penerimaan.ToString() &&
                        transaction.TransactionType != TransactionType.pengembalian.ToString() &&
                        transaction.TransactionType != TransactionType.pengiriman.ToString())
                    {
                        throw new Exception("TransactionType tidak valid");
                    }

                    // Query SQL untuk mengecek apakah ada transaksi dengan ID yang sama dan sudah dihapus (soft-delete)
                    var sqlCheck = @"SELECT * FROM itemsupplier_transaction WHERE id = @Id AND deleted != TRUE";
                    var deletedRecord = await _db.QuerySingleOrDefaultAsync<ItemSupplier_TransactionModel>(sqlCheck, transaction);

                    if (deletedRecord == null)
                    {
                        throw new Exception("ItemSupplier_Transaction tidak ditemukan");
                    }

                    transaction.ModifiedOn = DateTime.Now;
                    var updateSql = @"UPDATE itemsupplier_transaction SET itemsupplierid = @ItemSupplierId, 
                    transactiontype = @TransactionType, 
                    transactiondate = @TransactionDate, 
                    quantity = @Quantity, 
                    notes = @Notes, 
                    deleted = @Deleted, 
                    createdby = @CreatedBy, 
                    modifiedon = @ModifiedOn, 
                    modifiedby = @ModifiedBy
                    WHERE id = @Id";

                    int updatedRows = await _db.ExecuteAsync(updateSql, transaction);

                    if (updatedRows == 0)
                    {
                        throw new Exception($"Tidak data yang diubah pada ID: {transaction.ID}");
                    }
                }
                return transaction;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        // Method to delete a transaction
        public async Task<ItemSupplier_TransactionModel> Delete(string id)
        {
            try
            {
                // Cek apakah ada record dengan ID yang sama sudah dihapus (soft-delete)
                // Cek apakah item dengan ID tersebut sudah dihapus (soft-delete)
                var deletedItem = await _db.QueryFirstOrDefaultAsync<ItemSupplier_TransactionModel>(
                    "SELECT * FROM ItemSupplier_Transaction WHERE Id = @Id AND Deleted != TRUE", new { Id = id });

                if (deletedItem == null)
                {
                    throw new Exception("ItemSupplier_Transaction tidak ditemukan.");
                }

                var sql = @"SELECT * FROM ItemSupplier_Transaction WHERE Id = @Id AND Deleted = FALSE";
                var itemSupplier_Transaction = await _db.QuerySingleOrDefaultAsync<ItemSupplier_TransactionModel>(sql, new { Id = id });
                itemSupplier_Transaction.ModifiedOn = DateTime.Now;
                itemSupplier_Transaction.Deleted = true;

                if (itemSupplier_Transaction == null)
                {
                    throw new Exception($"Tidak ada ItemSupplier_Transaction ditemukan dengan id: {id}");
                }

                sql = @"UPDATE ItemSupplier_Transaction SET Deleted = true WHERE Id = @Id";
                var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });
                if (affectedRows == 0)
                {
                    throw new Exception($"Tidak ada ItemSupplier ditemukan dengan id: {id}");
                }

                return itemSupplier_Transaction;

            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}