
using System.Collections.Generic;
using Common.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PW.DataModel.Entities;
using PW.Helpers;
using PW.Services;
using PW.ViewModels;


namespace PW.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private IAccountService _accountService;
        private IAccountTransactionService _accountTransactionService;
        public string LoggedInUser => User.Identity.Name;
        private IUserService _userService;

        public AccountController(IUserService userService, IAccountService accountService, IAccountTransactionService accountTransactionService)
        {
            _accountTransactionService = accountTransactionService;
            _accountService = accountService;
            _userService = userService;
        }

        [HttpPost("{recipientId}/{amount}")]
        public IActionResult CreateNewTransaction(int recipientId, decimal amount)
        {
            try
            {
                int currentUserId = int.Parse(LoggedInUser);

                var transactionTuple = _accountService.CreateTransaction(currentUserId, recipientId, amount);
                AccountTransaction transaction = transactionTuple.Item1;
                string message =  transactionTuple.Item2;

                if (transaction!= null)
                {
                    User payee, recipient;
                    payee = _userService.GetUserById(_accountService.GetAccountById(transaction.PayeeId).UserId);
                    recipient = _userService.GetUserById(_accountService.GetAccountById(transaction.RecipientId).UserId);
                    AccountTransactionsForUser modelTransaction = _accountService.GetModelAccountTransactionsForUser(transaction, payee, recipient, currentUserId);
                    return new ObjectResult(new
                    {
                        Transaction = modelTransaction,
                        Message = message,
                        Success = true
                    });
                }
                else
                    return new ObjectResult(new
                    {
                        Message = message,
                        Success = false
                    });
            }
            catch (AppException ex)
            {
                return new ObjectResult(new
                {
                    Message = "Transaction is not succeed." + ex.Message,
                    Success = false
                });
            }
        }
        

        [HttpGet("/api/balance")]
        public IActionResult GetBalance()
        {
            try
            {
                int currentUserId = int.Parse(LoggedInUser);
                decimal balance = _accountService.GetBalanceofUser(currentUserId);
                return Ok(balance);
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetTransactions()
        {
            int currentUserId = int.Parse(LoggedInUser);
            User payee, recipient;
            Account currentAccount = _accountService.GetAccountOfUser(currentUserId);
            if (currentAccount != null)
            {

                List<AccountTransaction> transactions = _accountTransactionService.GetTransactionsForAccount(currentAccount);

                if (transactions.Count > 0)
                {
                    List<AccountTransactionsForUser> ListOfTransactionModel = new List<AccountTransactionsForUser>();
                    foreach (AccountTransaction transaction in transactions)
                    {
                        
                        payee = _userService.GetUserById(_accountService.GetAccountById(transaction.PayeeId).UserId);
                        recipient = _userService.GetUserById(_accountService.GetAccountById(transaction.RecipientId).UserId);

                        ListOfTransactionModel.Add(_accountService.GetModelAccountTransactionsForUser(transaction, payee, recipient, currentUserId));
                    }

                    return Ok(ListOfTransactionModel);
                }
                else return NotFound();
            }
            else return NotFound();
        }

    }

}