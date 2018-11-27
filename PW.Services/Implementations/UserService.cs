using AutoMapper;
using System.Linq;
using System;
using System.Collections.Generic;
using Common;
using PW.DataModel.Entities;
using PW.ViewModels;
using Common.Helpers;
using PW.DataAccess;
using PW.DataModel.Enums;

namespace PW.Services
{
    public class UserService : IUserService
    {

        private readonly IAccountService _accountService;

           PWContext db;


        public UserService(
            PWContext context,
           IAccountService accountService

        )
        {
            db = context;
            _accountService = accountService;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

             var user = db.Users.SingleOrDefault(x => x.Email == email);

             if (user == null)
                return null;

            if (!PasswordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public List<AutocomleteSelectModel> GetUserBySearchString(int currentUser, string searchStr, int count)
        {
            var query = db.Users
                .Where(a => a.Id != currentUser)
               .Where(x => x.UserName.ToLower().Contains(searchStr))
               .Take(count)
               .OrderByDescending(x => x.UserName );

            var result = query.Select(y => new AutocomleteSelectModel { Id = y.Id,  Value = y.UserName, Label = $"{y.UserName}, Email: {y.Email}" });

            return result.ToList<AutocomleteSelectModel>();
        }
        public User GetUserById(int userId)
        {
            return db.Users.FirstOrDefault(o => o.Id == userId);
        }

        public (User,string) Create(User user, string password)
        {
             if (string.IsNullOrWhiteSpace(password))
            {
                return (null, "Password is required");

            };
            User entity = db.Users.FirstOrDefault(o => o.Email == user.Email);
            if (entity == null)
            {


                byte[] passwordHash, passwordSalt;
                PasswordHasher.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                user.Type = UserType.Client;

                string message = _accountService.OpenNewAccount(user);

                message += AddFirstSumOnBalance(user);
                return (user, "Your account is created." + message);
            }
            else
            {
                return  (null, "The email is already exist. Choose another one.");
            }

    }

    

        protected internal string AddFirstSumOnBalance(User recipient)
        {
            decimal FirstSumOnBalance = 500;
            try
            {
                int systemUserId = _accountService.GetSystemAccount().UserId;
                _accountService.CreateTransaction(systemUserId, recipient.Id, FirstSumOnBalance);

                return $"Your Balance is {FirstSumOnBalance}";
            }
            catch (AppException ex)
            {
                return ex.Message;
            }
        }

    }


}
