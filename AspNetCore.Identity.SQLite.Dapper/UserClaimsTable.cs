using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class UserClaimsTable<TUser, TUserKey> : IUserClaimsTable<TUser, TUserKey> 
        where TUser : IdentityUserCore<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        private readonly IDBRepositoryConfiguration _config;
        private readonly string claimsTable = "AspNetUserClaims";

        public UserClaimsTable(IDBRepositoryConfiguration config)
        {
            this._config = config;
            this.claimsTable = config.ClaimsTableName;
        }

        public async Task Delete(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = string.Format($"DELETE FROM {this.claimsTable} WHERE UserId = @UserId AND ClaimValue = @ClaimValue AND ClaimType = @ClaimType");

                connection.Open();

                foreach (var claim in claims)
                {
                    DynamicParameters paramteres = new DynamicParameters();
                    paramteres.Add("@UserId", user.Id);
                    paramteres.Add("@ClaimValue", claim.Value, DbType.String);
                    paramteres.Add("@ClaimType", claim.Type, DbType.String);

                    await connection.ExecuteAsync(new CommandDefinition(commandText, paramteres, cancellationToken: cancellationToken));
                }                
            }
        }

        public async Task<ClaimsIdentity> FindByUserId(TUserKey id, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                ClaimsIdentity identity = new ClaimsIdentity();

                string commandText = $"SELECT ClaimValue, ClaimType, UserId FROM {this.claimsTable} WHERE UserId == @UserId";

                connection.Open();

                DynamicParameters paramteres = new DynamicParameters();
                paramteres.Add("@UserId", id);

                var claims = await connection.QueryAsync<Claim>(
                    new CommandDefinition(commandText, paramteres, cancellationToken: cancellationToken));

                identity.AddClaims(claims);

                return identity;
            }
        }

        public Task Insert(Claim claim, TUserKey id, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                return this.Insert(claim, id, cancellationToken, connection);
            }
        }

        public Task Insert(IEnumerable<Claim> claims, TUserKey id, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                foreach (var claim in claims)
                {
                    this.Insert(claim, id, cancellationToken, connection);
                }

                return Task.FromResult(default(object));
            }
        }

        private Task Insert(Claim claim, TUserKey id, CancellationToken cancellationToken, SQLiteConnection connection)
        {
            string commandText = $"INSERT INTO {this.claimsTable} (ClaimValue, ClaimType, UserId) VALUES (@value, @type, @userId";

            connection.Open();
            DynamicParameters paramteres = new DynamicParameters();
            paramteres.Add("@value", claim.Value, DbType.String);
            paramteres.Add("@type", claim.Type, DbType.String);
            paramteres.Add("@userId", id);

            return connection.ExecuteAsync(
                new CommandDefinition(commandText, paramteres, cancellationToken: cancellationToken));
        }

        public Task ReplaceClaim(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            using (var connection = new SQLiteConnection(_config.ConnectionString))
            {
                string commandText = $"UPDATE {this.claimsTable} SET ClaimValue = @NewClaimValue, ClaimType = @NewClaimType " +
                    $"WHERE ClaimValue = @OldClaimValue AND ClaimType = @OldClaimType AND UserId = @UserId";

                connection.Open();
                DynamicParameters paramteres = new DynamicParameters();
                paramteres.Add("@NewClaimValue", newClaim.Value, DbType.String);
                paramteres.Add("@NewClaimType", newClaim.Type, DbType.String);
                paramteres.Add("@OldClaimValue", claim.Value, DbType.String);
                paramteres.Add("@OldClaimType", claim.Type, DbType.String);
                paramteres.Add("@UserId", user.Id);

                return connection.ExecuteAsync(
                    new CommandDefinition(commandText, paramteres, cancellationToken: cancellationToken));
            }            
        }
    }
}
