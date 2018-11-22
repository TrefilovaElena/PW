using PW.DataAccess;
using PW.DataModel.Entities;
using PW.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PW.Services
{
    public class AccountTransactionService : IAccountTransactionService
    {
        PWContext db;
        public event AccountStateHandler CreateTransaction;
        public event AccountStateHandler FailedCreatingTransaction;

        public AccountTransactionService(PWContext context)
        {
            db = context;
        }
        public AccountTransaction Add(Account payee, Account recipient, decimal amount)
        {
            AccountTransaction accountTransaction = new AccountTransaction();
            accountTransaction.PayeeId = payee.Id;
            accountTransaction.RecipientId = recipient.Id;
            accountTransaction.PayeeBalance = payee.Balance;
            accountTransaction.RecipientBalance = recipient.Balance;
            accountTransaction.TimeOfTransaction = DateTime.Now;
            accountTransaction.Amount = amount;

            db.AccountTransactions.Add(accountTransaction);
            db.SaveChanges();
            return accountTransaction;

        }
        private void CallEvent(AccountEventArgs e, AccountStateHandler handler)
        {
            if (handler != null && e != null)
                handler(this, e);
        }

        protected virtual void OnCreateTransaction(AccountEventArgs e)
        {
            CallEvent(e, CreateTransaction);
        }
        protected virtual void OnFailedCreatingTransaction(AccountEventArgs e)
        {
            CallEvent(e, FailedCreatingTransaction);
        }

        public List<AccountTransaction> GetTransactionsForAccount(Account account)
       
        {    var q = db.AccountTransactions                
                .Where(a => (a.Payee.Id == account.Id) || (a.Recipient.Id == account.Id))
                 .OrderByDescending(x => x.TimeOfTransaction); 

         /*   var q =
       from accountTransaction in db.AccountTransactions
       join payeeAccount in db.Accounts on accountTransaction.PayeeId equals payeeAccount.Id
       join payeeUser in db.Users on payeeAccount.UserId equals payeeUser.Id
       join recipientAccounts in db.Accounts on accountTransaction.RecipientId equals recipientAccounts.Id
       join recipientUser in db.Users on recipientAccounts.UserId equals recipientUser.Id
       select new AccountTransactionsForUser { Payee = payeeAccount }; */

                return q.Select(y => y).ToList<AccountTransaction>();
        }
    }

}
 