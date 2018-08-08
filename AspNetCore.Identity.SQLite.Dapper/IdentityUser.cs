using System;

namespace AspNetCore.Identity.SQLite.Dapper
{
    public class IdentityUserCore<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// User ID
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        ///     The salted/hashed form of the user password
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTimeOffset? LockoutEndDateUtc { get; set; }

        /// <summary>
        ///     Is lockout enabled for this user
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        ///     Used to record failures for the purposes of lockout
        /// </summary>
        public virtual int AccessFailedCount { get; set; }
    }
}
