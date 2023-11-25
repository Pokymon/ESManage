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

namespace es_manage.api.Repositories
{
    public class RoleRepository
    {
        private readonly AppDbContext _context;
        private readonly IDbConnection _db;

        public RoleRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _db = new NpgsqlConnection(configuration.GetConnectionString("Main"));
        }

        public async Task<IEnumerable<RoleModel>> GetAll()
        {
            try
            {
                var sql = @"SELECT * FROM role WHERE deleted = false";
                return await _db.QueryAsync<RoleModel>(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoleModel> GetById(string id)
        {
            try
            {
                var sql = @"SELECT * FROM ""role"" WHERE ""id"" = @Id";
                return await _db.QuerySingleOrDefaultAsync<RoleModel>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoleModel> GetByName(string name)
        {
            try
            {
                var sql = @"SELECT * FROM ""role"" WHERE ""rolename"" = @Name";
                return await _db.QuerySingleOrDefaultAsync<RoleModel>(sql, new { Name = name });
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoleModel> Create(RoleModel role)
        {
            try
            {
                // Query SQL untuk mengecek apakah sudah ada record Role dengan nama yang sama dan belum dihapus
                var activeSql = @"SELECT * 
                FROM Role 
                WHERE RoleName = @RoleName 
                    AND Deleted = FALSE";

                var activeRecord = await _db.QueryFirstOrDefaultAsync<RoleModel>(activeSql, role);

                if (activeRecord != null){
                    throw new Exception("Role dengan nama yang sama sudah ada");
                }

                var maxIDSql = @"SELECT COALESCE(MAX(CAST(id AS INTEGER)), 0) FROM Role";
                var maxID = await _db.QuerySingleAsync<int>(maxIDSql);
                role.Id = (maxID + 1).ToString();
                role.CreatedOn = DateTime.Now;
                role.Deleted = false;

                var sql = @"INSERT INTO ""role"" (""id"", ""rolename"", ""createdon"", ""createdby"")
                            VALUES (@Id, @RoleName, @CreatedOn, @CreatedBy)";

                await _db.ExecuteAsync(sql, role);
                return role;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoleModel> Update(string id, RoleModel role)
        {
            try
            {
                if (id != role.Id)
                {
                    throw new Exception("ID in URL does not match ID in request body.");
                }

                var existingRole = await _db.QuerySingleOrDefaultAsync<RoleModel>(
                    "SELECT * FROM \"role\" WHERE \"id\" = @Id", new { Id = role.Id });

                if (existingRole == null)
                {
                    throw new Exception("Role not found.");
                }

                role.CreatedOn = existingRole.CreatedOn;
                role.CreatedBy = existingRole.CreatedBy;
                role.ModifiedOn = DateTime.Now;

                var updateSql = @"UPDATE ""role"" 
                                SET ""rolename"" = @RoleName, ""modifiedon"" = @ModifiedOn, ""modifiedby"" = @ModifiedBy
                                WHERE ""id"" = @Id";

                int updatedRows = await _db.ExecuteAsync(updateSql, role);

                if (updatedRows == 0)
                {
                    throw new Exception($"No role found with ID: {role.Id}");
                }

                return role;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoleModel> Delete(string id)
        {
            try
            {
                var existingRole = await _db.QuerySingleOrDefaultAsync<RoleModel>(
                    "SELECT * FROM Role WHERE Id = @Id", new { Id = id });

                if (existingRole == null)
                {
                    throw new Exception("Role not found.");
                }

                var sql = "SELECT * FROM Role WHERE Id = @Id AND Deleted = FALSE";
                var role = await _db.QuerySingleOrDefaultAsync<RoleModel>(sql, new { Id = id });

                if (role == null)
                {
                    throw new Exception($"Tidak ada role ditemukan dengan id: {id}");
                }

                role.ModifiedOn = DateTime.Now;
                role.Deleted = true;

                sql = "UPDATE Role SET Deleted = true WHERE Id = @Id";
                var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });

                if (affectedRows == 0)
                {
                    throw new Exception($"Tidak ada role ditemukan dengan id: {id}");
                }

                return role;
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
