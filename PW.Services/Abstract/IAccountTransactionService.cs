using PW.DataModel.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace PW.Services
{
    public interface IAccountTransactionService
    {
        AccountTransaction Add(Account payee, Account recipient, decimal amount);
        List<AccountTransaction> GetTransactionsForAccount(Account account);
    }
}
