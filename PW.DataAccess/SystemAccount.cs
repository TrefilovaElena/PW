using Common.Helpers;
using PW.DataModel.Entities;
using PW.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PW.DataAccess
{
   public class SystemAccount : Account
    {
        public SystemAccount()
        {
            byte[] passwordHash, passwordSalt;
            PasswordHasher.CreatePasswordHash("1", out passwordHash, out passwordSalt);
            User systemUser = new User { Id = 1, UserName = "System", Email = "1@1.com", Type = UserType.System, PasswordHash = passwordHash, PasswordSalt = passwordSalt };

            Id = 1;
            UserId = 1;
            User = systemUser;
            Balance = 0;
        }
    }
}
