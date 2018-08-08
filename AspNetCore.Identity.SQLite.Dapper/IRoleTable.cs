using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public interface IRoleTable<TRole, TKey>
         where TKey : IEquatable<TKey>
    {
        Task<int> Delete(TKey roleId, CancellationToken cancelationToken);

        Task<int> Insert(TRole role, CancellationToken cancelationToken);

        Task<int> Update(TRole role, CancellationToken cancelationToken);        

        IEnumerable<TRole> GetRoles();        

        Task<TRole> GetRoleId(string roleName, CancellationToken cancelationToken);

        Task<TRole> GetRoleName(TKey roleId, CancellationToken cancelationToken);

        Task<TRole> GetRoleByNormalizeName(string normalizedRoleName, CancellationToken cancellationToken);
    }
}