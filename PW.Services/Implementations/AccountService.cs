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

        PWContext db;

        private readonly IAccountTransactionService _accountTransactionService;


        public AccountService( PWContext context, IAccountTransactionService accountTransactionService)
        {
            db = context;
            _accountTransactionService = accountTransactionService;

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
            if ((payee.Id != GetSystemAccount().Id) && (payee.Balance < amount))
            {
                return (null, "Transaction is not succeed: transaction amount is greater than the current balance.");
            }
            //1. вариант без транзакций---------------------------

                    recipient.Balance = recipient.Balance + amount;
                    payee.Balance = payee.Balance - amount;
                    db.SaveChanges();
                    AccountTransaction accountTransaction =_accountTransactionService.Add(payee, recipient, amount);
            //-----------------------------------
            //2. вариант с хранимой процедурой----------------------
            /* using (SqlConnection connection = new SqlConnection(""))//ConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString))
            CREATE PROCEDURE AddTransaction
     @payeeUserId int,
     @recipientUserId int,
     @amount float,
     @TransactionId int out
 AS
 DECLARE  @payeeBalance int,
          @recipientBalance int,
          @payeeAccountId int,
          @recipientAccountId int

 BEGIN TRANSACTION
 select @payeeBalance=Balance-@amount,
        @payeeAccountId=Id 
 from [dbo].[Accounts]
 where  [dbo].[Accounts].UserId=@payeeUserId

 select @recipientBalance=Balance+@amount,
        @recipientAccountId=Id  
 from [dbo].[Accounts] 
 where  [dbo].[Accounts].UserId=@recipientUserId


 Update [dbo].[Accounts] Set Balance=Balance-@amount WHERE  [dbo].[Accounts].UserId=@payeeUserId
 IF (@@error <> 0) ROLLBACK
 Update [dbo].[Accounts] Set Balance=Balance+@amount WHERE  [dbo].[Accounts].UserId=@recipientUserId
 IF (@@error <> 0) ROLLBACK
 Insert INTO AccountTransactions 
       ([Amount]
       ,[PayeeBalance]
       ,[PayeeId]
       ,[RecipientBalance]
       ,[RecipientId]
       ,[TimeOfTransaction])
  Values (@amount, @payeeBalance, @payeeAccountId, @recipientBalance, @recipientAccountId, GETDATE())
  IF (@@error <> 0) ROLLBACK
 COMMIT


 GO

             */

            /* 3. вариант с CreateCommand()
             
              using (SqlConnection connection = new SqlConnection(""))//ConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString))
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

        public Account GetSystemAccount()
        {
            return db.Accounts.FirstOrDefault(o => o.User.Type== DataModel.Enums.UserType.System);
        }

        public Account GetAccountById(int accountId)
        {
            return db.Accounts.FirstOrDefault(o => o.Id == accountId);
        }
    }
}
