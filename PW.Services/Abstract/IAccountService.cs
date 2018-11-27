using PW.DataModel.Entities;
using PW.ViewModels;
using System.Collections.Generic;

namespace PW.Services
{
    public interface IAccountService
    {
        decimal GetBalanceofAccount(int accountId);
        decimal GetBalanceofUser(int userId);
        (AccountTransaction, string) CreateTransaction(int payeeId, int recipientId, decimal amount);
        string OpenNewAccount(User user);
        Account GetAccountOfUser(int userId);
        Account GetSystemAccount();
        Account GetAccountById(int accountId);
        AccountTransactionsForUser GetModelAccountTransactionsForUser(AccountTransaction transaction, User payee, User recipient,int userId);
    }
}
