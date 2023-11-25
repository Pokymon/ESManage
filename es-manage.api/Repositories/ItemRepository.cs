// Tujuan: Berisi logika untuk operation database pada tabel ItemRepository

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
    public class ItemRepository {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        public ItemRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        public async Task<IEnumerable<ItemModel>> GetAll()
        {
            try {
                var sql = "SELECT * FROM Item WHERE Deleted = FALSE";
                return await _db.QueryAsync<ItemModel>(sql);
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<ItemModel> GetById(string id)
        {
            try {
                var sql = @"SELECT * FROM Item WHERE Id = @Id";
                return await _db.QuerySingleOrDefaultAsync<ItemModel>(sql, new { Id = id });
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<ItemModel> Create(ItemModel item)
        {
            try {
                var brandExists = await _db.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM Brand WHERE Id = @BrandId)", new { BrandId = item.BrandId });
                var categoryExists = await _db.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM ItemDepartment WHERE Id = @CategoryId AND CategoryName = @CategoryName)", new { CategoryId = item.CategoryId, CategoryName = item.CategoryName });

                if (!brandExists || !categoryExists)
                    throw new Exception("BrandId atau CategoryId tidak ada / tidak valid");

                var maxIDSql = @"SELECT COALESCE(MAX(CAST(id AS INTEGER)), 0) FROM Item";
                var maxID = await _db.QuerySingleAsync<int>(maxIDSql);
                item.ID = (maxID + 1).ToString();
                item.CreatedOn = DateTime.Now;

                var sql = @"
                    INSERT INTO Item (Id, ItemName, CategoryId, CategoryName, BrandId, Uom, TaxType, TaxRate, MinimumRetailPrice, BalanceQty, AvgCostPrice, RetailPrice, CostPrice, Deleted, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
                    VALUES (@Id, @ItemName, @CategoryId, @CategoryName, @BrandId, @Uom, @TaxType, @TaxRate, @MinimumRetailPrice, @BalanceQty, @AvgCostPrice, @RetailPrice, @CostPrice, @Deleted, @CreatedOn, @CreatedBy, @ModifiedOn, @ModifiedBy)
                    RETURNING *";

                return await _db.QuerySingleAsync<ItemModel>(sql, item);
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<ItemModel> Update(string id, ItemModel item)
        {
            try {
                var itemExists = await _db.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM Item WHERE Id = @Id)", new { Id = id });
                var brandExists = await _db.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM Brand WHERE Id = @BrandId)", new { BrandId = item.BrandId });
                var categoryExists = await _db.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM ItemDepartment WHERE Id = @CategoryId AND CategoryName = @CategoryName)", new { CategoryId = item.CategoryId, CategoryName = item.CategoryName });

                if (!itemExists || !brandExists || !categoryExists)
                    return null;

                item.ModifiedOn = DateTime.Now;
                var sql = @"
                    UPDATE Item 
                    SET ItemName = @ItemName,
                        CategoryId = @CategoryId,
                        CategoryName = @CategoryName,
                        BrandId = @BrandId,
                        Uom = @Uom,
                        TaxType = @TaxType,
                        TaxRate = @TaxRate,
                        MinimumRetailPrice = @MinimumRetailPrice,
                        BalanceQty = @BalanceQty,
                        AvgCostPrice = @AvgCostPrice,
                        RetailPrice = @RetailPrice,
                        CostPrice = @CostPrice,
                        Deleted = @Deleted,
                        CreatedOn = @CreatedOn,
                        CreatedBy = @CreatedBy,
                        ModifiedOn = @ModifiedOn,
                        ModifiedBy = @ModifiedBy
                    WHERE Id = @Id
                    RETURNING *";

                return await _db.QuerySingleAsync<ItemModel>(sql, item);
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(string id)
        {
            try {
                var itemExists = await _db.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM Item WHERE Id = @Id)", new { Id = id });

                if (!itemExists)
                    return false;

                var sql = @"
                    UPDATE Item
                    SET Deleted = TRUE
                    WHERE Id = @Id";

                await _db.ExecuteAsync(sql, new { Id = id });
                return true;
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}