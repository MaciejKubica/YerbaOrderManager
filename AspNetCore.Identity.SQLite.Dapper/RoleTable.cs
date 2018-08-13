using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class RoleTable<TRole, TRoleKey> : IRoleTable<TRole, TRoleKey>
        where TRole : IdentityRole<TRoleKey>
        where TRoleKey: IEquatable<TRoleKey>
    {
        private readonly IDBRepositoryConfiguration _config;
        private readonly string roleTableName = "AspNetRoles";

        public RoleTable(IDBRepositoryConfiguration config)
        {
            this._config = config;
            this.roleTableName = _config.RoleTableName;
        }

        public Task<int> Delete(TRoleKey roleId, CancellationToken cancelationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                connection.Open();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@RoleId", roleId);

                return connection.ExecuteAsync(
                           $"DELETE FROM {this.roleTableName} WHERE Id = @RoleId ", parameter);
            }
        }

        public Task<int> Insert(TRole role, CancellationToken cancelationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                connection.Open();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@RoleId", role.Id);
                parameter.Add("@Name", role.Name, DbType.AnsiString);

                return connection.ExecuteAsync(
                           $"INSERT INTO {this.roleTableName} (Id, Name) VALUES (@RoleId, @Name) ", parameter);
            }
        }

        /// <summary>
        /// Returns a role name given the roleId
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns>Role name</returns>
        public Task<TRole> GetRoleName(TRoleKey roleId, CancellationToken cancelationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.roleTableName} WHERE Id = @RoleId");

                connection.Open();
                DynamicParameters paramteres = new DynamicParameters();
                paramteres.Add("@RoleId", roleId);

                return connection.QueryFirstAsync<TRole>(commandText, paramteres);
            }
        }

        /// <summary>
        /// Returns the role Id given a role name
        /// </summary>
        /// <param name="roleName">Role's name</param>
        /// <returns>Role's Id</returns>
        public Task<TRole> GetRoleId(string roleName, CancellationToken cancelationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.roleTableName} WHERE Name LIKE @RoleName");

                connection.Open();
                DynamicParameters paramteres = new DynamicParameters();
                paramteres.Add("@RoleName", roleName, DbType.String);

                return connection.QueryFirstAsync<TRole>(commandText, paramteres);
            }
        }
     
        public IEnumerable<TRole> GetRoles()
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.roleTableName}");

                connection.Open();

                return connection.Query<TRole>(commandText);
            }
        }

        public Task<int> Update(TRole role, CancellationToken cancelationToken)
        {

            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"UPDATE {this.roleTableName} SET Name = @Name WHERE Id = @Id");

                connection.Open();
                DynamicParameters paramteres = new DynamicParameters();
                paramteres.Add("@Id", role.Id);
                paramteres.Add("@name", role.Name, DbType.String);

                return connection.ExecuteAsync(commandText, paramteres);
            }          
        }
        
        public Task<TRole> GetRoleByNormalizeName(string normalizedRoleName, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.roleTableName} WHERE UPPER(Name) LIKE @RoleName");

                connection.Open();
                DynamicParameters paramteres = new DynamicParameters();
                paramteres.Add("@RoleName", normalizedRoleName, DbType.String);

                return connection.QueryFirstAsync<TRole>(commandText, paramteres);
            }
        }
    }
}
