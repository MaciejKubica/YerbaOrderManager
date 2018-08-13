using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class UserStore<TUser, TRole, TUserKey, TRoleKey> : IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserStore<TUser>,
        IUserClaimStore<TUser>
        where TUser : IdentityUserCore<TUserKey>
        where TRole : IdentityRole<TRoleKey>
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        private readonly IDBRepositoryConfiguration config;
        private readonly IUserTable<TUser, TUserKey> userTable;
        private readonly IUserLoginsTable<TUser, TUserKey> userLoginTable;
        private readonly IRoleTable<TRole, TRoleKey> roleTable;
        private readonly IUserRolesTable<TUser, TUserKey, TRoleKey> userRolesTable;
        private readonly IUserClaimsTable<TUser, TUserKey> userClaimsTable;

        public UserStore(IDBRepositoryConfiguration config,
            IUserTable<TUser, TUserKey> userTable = null,
            IUserLoginsTable<TUser, TUserKey> userLoginTable = null,
            IRoleTable<TRole, TRoleKey> roleTable = null,
            IUserRolesTable<TUser, TUserKey, TRoleKey> userRolesTable = null,
            IUserClaimsTable<TUser, TUserKey> userClaimsTable = null)
        {
            this.config = config;

            if (userTable != null)
            {
                this.userTable = userTable;
            }
            else
            {
                this.userTable = new UserTable<TUser, TUserKey>(this.config);
            }

            if (userLoginTable != null)
            {
                this.userLoginTable = userLoginTable;
            }
            else
            {
                this.userLoginTable = new UserLoginsTable<TUser, TUserKey>(this.config);
            }

            if (roleTable != null)
            {
                this.roleTable = roleTable;
            }
            else
            {
                this.roleTable = new RoleTable<TRole, TRoleKey>(this.config);
            }

            if (userRolesTable != null)
            {
                this.userRolesTable = userRolesTable;
            }
            else
            {
                this.userRolesTable = new UserRolesTable<TUser, TUserKey, TRoleKey>(this.config);
            }

            if (userClaimsTable != null)
            {
                this.userClaimsTable = userClaimsTable;
            }
            else
            {
                this.userClaimsTable = new UserClaimsTable<TUser, TUserKey>(this.config);
            }
        }

        public IQueryable<TUser> Users => userTable.GetUsers().AsQueryable();

        public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claims == null)
            {
                throw new ArgumentNullException("user");
            }

            await userClaimsTable.Insert(claims, user.Id, cancellationToken);
        }

        public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            await userLoginTable.Insert(user, login);
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            var role = await roleTable.GetRoleId(roleName, cancellationToken);
            if (role != null)
            {
                await userRolesTable.Insert(user, role.Id, cancellationToken);
            }
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException("user");
                }

                var foundUser = await userTable.GetUserByEmail(user.Email, cancellationToken);

                if (foundUser != null)
                {
                    return IdentityResult.Success;
                }

                TUserKey insertResult = await userTable.Insert(user, cancellationToken);

                user.Id = insertResult;
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ex.HResult.ToString(), Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user != null)
            {
                int deleteResult = await userTable.Delete(user, cancellationToken);

                if (deleteResult > 0)
                {
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError() { Code = deleteResult.ToString(), Description = "Error during user deletion" });
                }
            }
            else
            {
                return IdentityResult.Failed(new IdentityError() { Code = "404", Description = "User is null" });
            }
        }

        public void Dispose()
        {

        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            return (await userTable.GetUserByEmail(normalizedEmail, cancellationToken)).FirstOrDefault();
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            return await userTable.GetUserByIdAsString(userId, cancellationToken) as TUser;
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            TUserKey userId = userLoginTable.FindByProviderAndKey(loginProvider, providerKey);

            return await userTable.GetUserById(userId, cancellationToken) as TUser;
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedUserName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            return (await userTable.GetUserByName(normalizedUserName, cancellationToken)).FirstOrDefault();
        }

        public async Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            var userFromDB = await userTable.GetUserById(user.Id, cancellationToken);

            if (userFromDB.AccessFailedCount == user.AccessFailedCount)
            {
                return userFromDB.AccessFailedCount;
            }
            else
            {
                throw new Exception("There was a mismatch between user and DB data");
            }
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            ClaimsIdentity identity = await userClaimsTable.FindByUserId(user.Id, cancellationToken);

            return identity.Claims.ToList();
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            var userFromDB = await userTable.GetUserById(user.Id, cancellationToken);

            return userFromDB.LockoutEnabled;
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user.LockoutEnabled && user.LockoutEndDateUtc == null)
            {
                return Task.FromResult(new DateTimeOffset?(DateTime.MaxValue));
            }

            return Task.FromResult(user.LockoutEndDateUtc);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return (await userLoginTable.FindByUserId(user.Id))?.ToList();
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.Email.ToUpper());
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.Name.ToUpper());
        }

        public async Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return await userTable.GetPasswordHash(user.Id, cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return (await userRolesTable.GetRolesByUserId(user.Id, cancellationToken))?.ToList();
        }

        public async Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            return string.Empty;
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Name);
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return await userRolesTable.GetUsersByRoleName(roleName, cancellationToken);
        }

        public async Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return !string.IsNullOrEmpty(await userTable.GetPasswordHash(user.Id, cancellationToken));
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            var userFromDB = await userTable.GetUserById(user.Id, cancellationToken);

            if (userFromDB.AccessFailedCount == user.AccessFailedCount)
            {
                user.AccessFailedCount++;
                userFromDB.AccessFailedCount++;
                await userTable.Update(user, cancellationToken);

                return userFromDB.AccessFailedCount;
            }
            else
            {
                throw new Exception("There was a mismatch between user and DB data");
            }
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException(nameof(roleName));
            }

            return await userRolesTable.UserIsInRole(user.Id, roleName, cancellationToken);
        }

        public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            await userClaimsTable.Delete(user, claims, cancellationToken);
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException(nameof(roleName));
            }

            await userRolesTable.RemoveUserFromRole(user.Id, roleName, cancellationToken);
        }

        public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException("login");
            }

            await userLoginTable.Delete(user.Id, loginProvider, providerKey);
        }

        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            await userClaimsTable.ReplaceClaim(user, claim, newClaim, cancellationToken);
        }

        public async Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            await userTable.Update(user, cancellationToken);
        }

        public async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            await userTable.Update(user, cancellationToken);
        }

        public async Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            await Task.FromResult(default(object));
        }

        public async Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            await userTable.Update(user, cancellationToken);
        }

        public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (lockoutEnd.HasValue)
            {
                user.LockoutEndDateUtc = lockoutEnd.Value.UtcDateTime;
                await userTable.Update(user, cancellationToken);
            }
        }

        public async Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            await Task.FromResult(default(object));
        }

        public async Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            await Task.FromResult(default(object));
        }

        public async Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            TUser foundUser = null;
            try
            {
                foundUser = await userTable.GetUserById(user.Id, cancellationToken);
            }
            catch
            {
                // if there was some problem or user not found
            }

            user.PasswordHash = passwordHash;

            if (foundUser != null)
            {
                await userTable.Update(user, cancellationToken);
            }
            else
            {
                user.Id = await userTable.Insert(user, cancellationToken);
            }
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(object));
        }

        public async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.Name = userName;
            await userTable.Update(user, cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException("user");
                }

                int updateResult = await userTable.Update(user, cancellationToken);

                if (updateResult > 0)
                {
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError() { Code = updateResult.ToString(), Description = "Error during update user" });
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ex.HResult.ToString(), Description = ex.Message });
            }
        }
    }
}
