using AutoMapper;
using System.Linq;
using System;
using System.Collections.Generic;
using Common;
using PW.DataModel.Entities;
using PW.ViewModels;
using System.Data.SqlClient;
using System.Configuration;
using PW.DataAccess;

namespace PW.Services
{
    public class AccountService : IAccountService
    {
        public event AccountStateHandler CreateAccount;
        public event AccountStateHandler FailedCreatingAccount;

        PWContext db;

        private readonly IAccountTransactionService _accountTransactionService;


        public AccountService( PWContext context, IAccountTransactionService accountTransactionService)
        {
            db = context;
            _accountTransactionService = accountTransactionService;

        }

        private void CallEvent(AccountEventArgs e, AccountStateHandler handler)
        {
            if (handler != null && e != null)
                handler(this, e);
        }

        protected virtual void OnCreateAccount(AccountEventArgs e)
        {
            CallEvent(e, CreateAccount);
        }
        protected virtual void OnFailedCreatingAccount(AccountEventArgs e)
        {
            CallEvent(e, FailedCreatingAccount);
        }

        public decimal GetBalanceofAccount(int accountId)
        {
            Account entity = db.Accounts.FirstOrDefault(o => o.Id == accountId);
            return (entity != null) ? entity.Balance : 0;
        }
        public decimal GetBalanceofUser(int userId)
        {
            Account entity = db.Accounts.FirstOrDefault(o => o.UserId == userId);
            return (entity != null) ? entity.Balance : 0;
        }

        public Account GetAccountOfUser(int userId)
        {
            return db.Accounts.FirstOrDefault(o => o.UserId == userId);
        }

        public AccountTransactionsForUser GetModelAccountTransactionsForUser(AccountTransaction transaction, User payee, User recipient, int userId)
        {
            AccountTransactionsForUser transactionsModel = new AccountTransactionsForUser();
            transactionsModel.Amount = transaction.Amount;
            transactionsModel.TimeOfTransaction = transaction.TimeOfTransaction.ToString("g");
            transactionsModel.PayeeId = payee.Id;
            transactionsModel.PayeeName = payee.UserName;
            transactionsModel.RecipientId = recipient.Id;
            transactionsModel.RecipientName = recipient.UserName;
            transactionsModel.Balance = (userId == payee.Id) ? transaction.PayeeBalance : transaction.RecipientBalance;
            transactionsModel.Repeat = (userId == payee.Id) ? true : false;

            return transactionsModel;
        }


        public (AccountTransaction, string) CreateTransaction(int payeeUserId, int recipientUserId, decimal amount)
        {
            if (amount<=0)
            {
                return (null, "Transaction is not succeed: transaction amount must be greater than zero.");
            }
            Account recipient = GetAccountOfUser(recipientUserId);         
            if (recipient == null)
            {
                return (null, "Transaction is not succeed: Recipient does not exist.");
            }
            Account payee = GetAccountOfUser(payeeUserId);
            if ((payee.Id != DbInitializer.SystemAccount.Id) && (payee.Balance < amount))
            {
                OnFailedCreatingAccount(new AccountEventArgs("Transaction is not succeed: transaction amount is greater than the current balance."));
                return (null, "Transaction is not succeed: transaction amount is greater than the current balance.");
            }
 

            recipient.Balance = recipient.Balance + amount;
            payee.Balance = payee.Balance - amount;
            db.SaveChanges();
            AccountTransaction accountTransaction =_accountTransactionService.Add(payee, recipient, amount);
            /*  using (SqlConnection connection = new SqlConnection(""))//ConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString))
              {
                  connection.Open();
                  SqlTransaction transaction = connection.BeginTransaction();

                  SqlCommand command = connection.CreateCommand();
                  command.Transaction = transaction;
                  try
                  {
                      command.CommandText = $"Update Accounts Set Balance=Balance-{amount} where Accounts.UserId={payeeId}";
                      command.ExecuteNonQuery();
                      command.CommandText = $"Update Accounts Set Balance=Balance+{amount} where Accounts.UserId={recipientId}";
                      command.ExecuteNonQuery();
                      transaction.Commit();
                  }
                  catch (Exception ex)
                  {
                      transaction.Rollback();
                      OnNewTransaction(new AccountEventArgs(recipientId, $"Your transaction is not complited. " + ex.Message));
                      return;
                  }
              } */

            return (accountTransaction, "Transaction is complited.");
        }



        public string OpenNewAccount(User user)
        {
            try
            {
                Account entity = new Account();
                entity.User = user;
                db.Accounts.Add(entity);
                db.SaveChanges();
                return "Your Account is opened.";
            }
            catch
            {
                return "Failed to create account.";
            }
        }

        public Account GetAccountById(int accountId)
        {
            return db.Accounts.FirstOrDefault(o => o.Id == accountId);
        }
    }
}
