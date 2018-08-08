using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class UserLoginsTable<TUser, TKey> : IUserLoginsTable<TUser, TKey>
        where TUser : IdentityUserCore<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IDBRepositoryConfiguration _config;
        private readonly string userLoginsTable = "AspNetUserLogins";

        public UserLoginsTable(IDBRepositoryConfiguration config)
        {
            this._config = config;
            this.userLoginsTable = this._config.UserLogins;
        }

        public Task<int> Delete(TKey userId)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"DELETE FROM {this.userLoginsTable} WHERE Id = @Id");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Id", userId);

                return connection.ExecuteAsync(commandText, orderItemParams);
            }
        }

        public Task<int> Delete(TKey userId, string loginProvider, string providerKey)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"DELETE FROM {this.userLoginsTable} WHERE Id = @Id AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey");

                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@Id", userId);
                orderItemParams.Add("@LoginProvider", loginProvider, DbType.String);
                orderItemParams.Add("@ProviderKey", providerKey, DbType.String);

                return connection.ExecuteAsync(commandText, orderItemParams);
            }
        }

        public TKey FindByProviderAndKey(string loginProvider, string providerKey)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                connection.Open();

                string commandText = string.Format($"SELECT Id FROM {this.userLoginsTable} WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey");

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@LoginProvider", loginProvider, DbType.String);
                parameters.Add("@ProviderKey", providerKey, DbType.String);

                return connection.ExecuteScalar<TKey>(commandText, parameters);
            }
        }

        public Task<IEnumerable<UserLoginInfo>> FindByUserId(TKey userId)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT * FROM {this.userLoginsTable} WHERE Id = @Id");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", userId);

                return connection.QueryAsync<UserLoginInfo>(commandText, parameters);
            }
        }

        public Task<TKey> FindUserIdByLogin(UserLoginInfo userLogin)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"SELECT UserId FROM {this.userLoginsTable} WHERE LoginProvider = @loginProvider and ProviderKey = @providerKey");

                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@loginProvider", userLogin.LoginProvider, DbType.String);
                parameters.Add("@providerKey", userLogin.ProviderKey, DbType.String);

                return connection.QueryFirstAsync<TKey>(commandText, parameters);
            }
        }

        public Task<int> Insert(TUser user, UserLoginInfo login)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                connection.Open();
                string commandText = string.Format($"INSERT INTO {this.userLoginsTable} (LoginProvider, ProviderKey, UserId) VALUES (@LoginProvider, @ProviderKey, @UserId)");

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@LoginProvider", login.LoginProvider, DbType.AnsiString);
                parameter.Add("@ProviderKey", login.ProviderKey, DbType.AnsiString);
                parameter.Add("@UserId", user.Id);             

                return connection.ExecuteAsync(commandText, parameter);
            }
        }
    }
}
