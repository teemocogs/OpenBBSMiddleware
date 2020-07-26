using System;
using System.Collections.Generic;

namespace DBModel.SqlModels
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool VerifyType0 { get; set; }
        public bool VerifyType1 { get; set; }
        public bool VerifyType2 { get; set; }
        public bool VerifyType3 { get; set; }
        public bool VerifyType4 { get; set; }
        public bool VerifyType5 { get; set; }
        public bool VerifyType6 { get; set; }
        public bool VerifyType7 { get; set; }
        public bool VerifyType8 { get; set; }
        public bool VerifyType9 { get; set; }

        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
    }
}
