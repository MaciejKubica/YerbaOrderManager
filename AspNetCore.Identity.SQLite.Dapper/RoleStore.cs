using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class RoleStore<TRole, TKey> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private IRoleTable<TRole, TKey> roleTable;

        public RoleStore(IRoleTable<TRole, TKey> roleTable)
        {
            this.roleTable = roleTable;
        }

        public IQueryable<TRole> Roles => roleTable.GetRoles().AsQueryable();

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            try
            {
                var roleTableResult = await roleTable.Insert(role, cancellationToken);
                if (roleTableResult > 0)
                {
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError() { Code = roleTableResult.ToString(), Description = "Error during role creation" });
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ex.HResult.ToString(), Description = ex.Message });
            }
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                var roleTableResult = await roleTable.Delete(role.Id, cancellationToken);
                if (roleTableResult > 0)
                {
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError() { Code = roleTableResult.ToString(), Description = "Error during role deletion" });
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ex.HResult.ToString(), Description = ex.Message });
            }
        }

        public void Dispose()
        {
           
        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            return await roleTable.GetRoleId(roleId, cancellationToken);
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            return await roleTable.GetRoleByNormalizeName(normalizedRoleName, cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult<string>(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult<string>(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult<string>(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            await roleTable.Update(role, cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                var roleTableResult = await roleTable.Update(role, cancellationToken);
                if (roleTableResult > 0)
                {
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError() { Code = roleTableResult.ToString(), Description = "Error during role update process" });
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ex.HResult.ToString(), Description = ex.Message });
            }
        }    
    }
}
