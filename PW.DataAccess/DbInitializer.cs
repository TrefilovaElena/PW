using Common.Helpers;
using PW.DataAccess;
using PW.DataModel.Entities;
using PW.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PW
{
    public static class DbInitializer
    {
       private static PWContext db;
        public static Account SystemAccount;


        public static void Initialize(IServiceProvider serviceProvider)
        {
              db = (PWContext)serviceProvider.GetService(typeof(PWContext)); 
              InitializeSystemUser();
        }
        private static void InitializeSystemUser()
        {
            if (!db.Users.Any())
            {
                byte[] passwordHash, passwordSalt;
                PasswordHasher.CreatePasswordHash("1", out passwordHash, out passwordSalt);
                Account systemAccount = new Account();
                systemAccount.User = new User { UserName = "System", Email = "1@1.com", Type = UserType.System, PasswordHash = passwordHash, PasswordSalt = passwordSalt };
                db.Accounts.Add(systemAccount);
                db.SaveChanges();
            }
            else
            {
                SystemAccount= db.Accounts.FirstOrDefault(x => x.User == db.Users.FirstOrDefault(o => o.Type == UserType.System));
            }
        }
       
    }
}
