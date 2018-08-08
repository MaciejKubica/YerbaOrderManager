using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public interface IDBRepositoryConfiguration
    {
        string ConnectionString { get; set; }
        string UserTableName { get; }
        string UserLogins { get; }
        string RoleTableName { get; }
        string UserRolesTableName { get; }
        string ClaimsTableName { get; }

        bool ShouldRemoveUserTable { get; }
    }
}
