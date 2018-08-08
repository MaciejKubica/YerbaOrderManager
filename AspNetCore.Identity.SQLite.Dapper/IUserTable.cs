using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public interface IUserTable<TUser, TKey>
        where TKey : IEquatable<TKey>
    {
        Task<string> GetUserName(TKey userId, CancellationToken cancellationToken);

        Task<string> GetUserId(string userName, CancellationToken cancellationToken);

        IEnumerable<TUser> GetUsers();

        Task<IEnumerable<TUser>> GetUserByEmail(string email, CancellationToken cancellationToken);

        Task<TUser> GetUserById(TKey userId, CancellationToken cancellationToken);

        Task<TUser> GetUserByIdAsString(string userId, CancellationToken cancellationToken);

        Task<IEnumerable<TUser>> GetUserByName(string userName, CancellationToken cancellationToken);

        Task<string> GetPasswordHash(TKey userId, CancellationToken cancellationToken);

        Task<int> SetPasswordHash(TKey userId, string passwordHash, CancellationToken cancellationToken);

        Task<TKey> Insert(TUser user, CancellationToken cancellationToken);

        Task<int> Update(TUser user, CancellationToken cancellationToken);

        Task<int> Delete(TUser user, CancellationToken cancellationToken);
    }
}