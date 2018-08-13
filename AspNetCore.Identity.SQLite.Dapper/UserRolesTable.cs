using Dapper;
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
    public class UserRolesTable<TUser, TUserKey, TRoleKey> : IUserRolesTable<TUser, TUserKey, TRoleKey> 
        where TUser : IdentityUserCore<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        private readonly IDBRepositoryConfiguration _config;
        private readonly string userRolesTableName = "AspNetUserRoles";

        public UserRolesTable(IDBRepositoryConfiguration config)
        {
            this._config = config;
            this.userRolesTableName = config.UserRolesTableName;
        }

        public Task<int> Delete(TUserKey userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"DELETE FROM {this.userRolesTableName} WHERE UserId = @UserId");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                return connection.ExecuteAsync(new CommandDefinition(commandText, parameters, cancellationToken: cancellationToken));
            }
        }

        public Task<IEnumerable<TUser>> FindByUserId(TUserKey userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT u.* FROM {this.userRolesTableName} as r JOIN {_config.UserTableName} as u ON r.UserId == u.Id WHERE u.Id == @Id ");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", userId);

                return connection.QueryAsync<TUser>(new CommandDefinition(commandText, parameters, cancellationToken: cancellationToken));
            }
        }

        public Task<IEnumerable<string>> GetRolesByUserId(TUserKey userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT r.name FROM {this.userRolesTableName} as ur JOIN {_config.RoleTableName} as r ON r.Id == ur.RoleId WHERE ur.UserId == @Id ");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", userId);

                return connection.QueryAsync<string>(new CommandDefinition(commandText, parameters, cancellationToken: cancellationToken));
            }
        }

        public Task<IList<TUser>> GetUsersByRoleName(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> Insert(TUser user, TRoleKey roleId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"INSERT INTO {this.userRolesTableName} (UserId, RoleId) VALUES (@UserId, @RoleId)");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", user.Id);
                parameters.Add("@RoleId", roleId);

                return connection.ExecuteAsync(commandText, parameters);
            }
        }

        public Task RemoveUserFromRole(TUserKey id, string roleName, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"DELETE ur FROM {this.userRolesTableName} ur JOIN {_config.RoleTableName} r ON ur.RoleId == r.Id WHERE ur.UserId = @UserId AND r.Name = @RoleName");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", id);
                parameters.Add("@RoleName", roleName);

                return connection.ExecuteAsync(new CommandDefinition(commandText, parameters, cancellationToken: cancellationToken));
            }
        }

        public async Task<bool> UserIsInRole(TUserKey userId, string roleName, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT Count(*) FROM {this.userRolesTableName} ur JOIN {_config.RoleTableName} r ON ur.RoleId == r.Id WHERE ur.UserId = @UserId AND r.Name LIKE @RoleName");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@RoleName", roleName);

                return await connection.ExecuteScalarAsync<int>(new CommandDefinition(commandText, parameters, cancellationToken: cancellationToken)) > 0;
            }
        }
    }
}
