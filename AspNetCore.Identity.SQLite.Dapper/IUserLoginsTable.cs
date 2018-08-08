using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public interface IUserLoginsTable<TUser, TKey>
        where TKey : IEquatable<TKey>
    {
        Task<int> Delete(TKey userId);

        Task<int> Delete(TKey userId, string loginProvider, string providerKey);

        Task<int> Insert(TUser user, UserLoginInfo login);

        Task<TKey> FindUserIdByLogin(UserLoginInfo userLogin);

        Task<IEnumerable<UserLoginInfo>> FindByUserId(TKey userId);

        TKey FindByProviderAndKey(string loginProvider, string providerKey);
    }
}