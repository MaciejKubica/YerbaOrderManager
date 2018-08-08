using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class DBRepositoryConfiguration : IDBRepositoryConfiguration
    {
        private readonly string userTableName;
        private readonly string userLoginsName;
        private readonly string roleTableName;
        private readonly string userRolesTableName;
        private readonly string claimsTableName;
        private string connectionString;
        private bool shouldRemoveUser;

        public DBRepositoryConfiguration(string connectionString,
            string userTableName,
            string userLoginsName,
            string roleTableName,
            string userRolesTableName,
            string claimsTableName,
            bool shouldRemoveUser)
        {
            this.userTableName = userTableName;
            this.userLoginsName = userLoginsName;
            this.roleTableName = roleTableName;
            this.userRolesTableName = userRolesTableName;
            this.claimsTableName = claimsTableName;
            this.connectionString = connectionString;
            this.shouldRemoveUser = shouldRemoveUser;
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        public string UserTableName => this.userTableName;

        public string UserLogins => this.userLoginsName;

        public string RoleTableName => this.roleTableName;

        public string UserRolesTableName => this.userRolesTableName;

        public string ClaimsTableName => this.claimsTableName;

        public bool ShouldRemoveUserTable => this.shouldRemoveUser;
    }
}
