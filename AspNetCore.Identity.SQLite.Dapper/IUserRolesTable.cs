using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public interface IUserRolesTable<TUser, TKey, TRoleKey>
        where TKey : IEquatable<TKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        Task<IEnumerable<TUser>> FindByUserId(TKey userId, CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetRolesByUserId(TKey userId, CancellationToken cancellationToken);

        Task<int> Delete(TKey userId, CancellationToken cancellationToken);

        Task<int> Insert(TUser user, TRoleKey roleId, CancellationToken cancellationToken);

        Task<IList<TUser>> GetUsersByRoleName(string roleName, CancellationToken cancellationToken);

        Task RemoveUserFromRole(TKey userId, string roleName, CancellationToken cancellationToken);

        Task<bool> UserIsInRole(TKey userId, string roleName, CancellationToken cancellationToken);
    }
}