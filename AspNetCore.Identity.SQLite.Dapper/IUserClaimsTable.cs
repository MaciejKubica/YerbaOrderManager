using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public interface IUserClaimsTable<TUser, TKey> where TUser : IdentityUserCore<TKey>  
        where TKey : IEquatable<TKey>
    {
        Task ReplaceClaim(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken);

        Task Delete(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken);

        Task<ClaimsIdentity> FindByUserId(TKey id, CancellationToken cancellationToken);

        Task Insert(Claim claim, TKey id, CancellationToken cancellationToken);

        Task Insert(IEnumerable<Claim> claims, TKey id, CancellationToken cancellationToken);
    }
}