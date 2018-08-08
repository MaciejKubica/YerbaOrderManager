using System;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class UserTable<TUser, TKey> : IUserTable<TUser, TKey>
        where TUser : IdentityUserCore<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IDBRepositoryConfiguration _config;
        private readonly string userTableName = "AspUsers";

        public UserTable(IDBRepositoryConfiguration config)
        {
            this._config = config;
            this.userTableName = _config.UserTableName;
        }

        /// <summary>
        /// Returns the user's name given a user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<string> GetUserName(TKey userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT Name FROM {this.userTableName} WHERE Id = @id");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@id", userId);

                return connection.ExecuteScalarAsync<string>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }


        /// <summary>
        /// Returns a User ID given a user name
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public Task<string> GetUserId(string userName, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT Id FROM {this.userTableName} WHERE Name = @Name");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Name", userName, DbType.String);

                return connection.ExecuteScalarAsync<string>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }

        /// <summary>
        /// Return all user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<TUser> GetUsers()
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.userTableName}");

                connection.Open();
                return connection.Query<TUser>(commandText);
            }
        }

        /// <summary>
        /// Returns an TUser given the user's id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public Task<TUser> GetUserById(TKey userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.userTableName} WHERE Id = @Id");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Id", userId);

                return connection.QueryFirstAsync<TUser>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }

        /// <summary>
        /// Returns a list of TUser instances given a user name
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns></returns>
        public Task<IEnumerable<TUser>> GetUserByName(string userName, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.userTableName} WHERE UPPER(Name) LIKE @Name");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Name", userName, DbType.String);

                return connection.QueryAsync<TUser>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }

        /// <summary>
        /// Return the user's password hash
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public Task<string> GetPasswordHash(TKey userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT Password FROM {this.userTableName} WHERE Id = @Id");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Id", userId);

                return connection.ExecuteScalarAsync<string>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }

        /// <summary>
        /// Sets the user's password hash
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task<int> SetPasswordHash(TKey userId, string passwordHash, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"UPDATE {this.userTableName} SET Password = @Password WHERE Id = @Id");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Id", userId);
                orderItemParams.Add("@Password", passwordHash, DbType.String);

                return connection.ExecuteScalarAsync<int>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }

        public Task<TKey> Insert(TUser user, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", user.Email, DbType.AnsiString);
                parameter.Add("@Name", user.Name, DbType.AnsiString);
                parameter.Add("@Password", user.PasswordHash, DbType.AnsiString);
                parameter.Add("@LockoutEndDateUtc", user.LockoutEndDateUtc, DbType.DateTime);
                parameter.Add("@LockoutEnabled", user.LockoutEnabled, DbType.Boolean);
                parameter.Add("@AccessFailedCount", 0, DbType.Int32);

                return connection.ExecuteScalarAsync<TKey>(
                    new CommandDefinition(
                    $"INSERT INTO {this.userTableName} (Email, Name, Password, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, OrderTokenLocker) " +
                    $"VALUES (@Email, @Name, @Password, @LockoutEndDateUtc, @LockoutEnabled, @AccessFailedCount, 0); SELECT last_insert_rowid()", parameter, cancellationToken: cancellationToken));
            }
        }

        public Task<int> Update(TUser user, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", user.Id, DbType.Int32);
                parameter.Add("@Email", user.Email, DbType.AnsiString);
                parameter.Add("@Name", user.Name, DbType.AnsiString);
                parameter.Add("@Password", user.PasswordHash, DbType.AnsiString);
                parameter.Add("@LockoutEndDateUtc", user.LockoutEndDateUtc, DbType.DateTime);
                parameter.Add("@LockoutEnabled", user.LockoutEnabled, DbType.Boolean);
                parameter.Add("@AccessFailedCount", user.AccessFailedCount, DbType.Int32);

                return connection.ExecuteAsync(
                    new CommandDefinition(
                           $"UPDATE {this.userTableName} SET Email = @Email, Name = @Name, Password = @Password, LockoutEndDateUtc = @LockoutEndDateUtc, LockoutEnabled = @LockoutEnabled, AccessFailedCount = @AccessFailedCount " +
                           $"WHERE Id = @Id ", parameter, cancellationToken: cancellationToken));
            }
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        private Task<int> Delete(TKey userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = $"DELETE FROM {this.userTableName} WHERE Id = @Id";

                if (!_config.ShouldRemoveUserTable)
                {
                    commandText = $"UPDATE {this.userTableName} SET IsDeleted = 1 WHERE Id = @Id";
                }

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Id", userId);

                return connection.ExecuteAsync(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> Delete(TUser user, CancellationToken cancellationToken)
        {
            return Delete(user.Id, cancellationToken);
        }

        public Task<IEnumerable<TUser>> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.userTableName} WHERE UPPER(Email) LIKE @Email");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Email", email, DbType.String);

                return connection.QueryAsync<TUser>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }

        public Task<TUser> GetUserByIdAsString(string userId, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.userTableName} WHERE Id = @Id");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Id", userId);

                return connection.QueryFirstAsync<TUser>(new CommandDefinition(commandText, orderItemParams, cancellationToken: cancellationToken));
            }
        }
    }
}
