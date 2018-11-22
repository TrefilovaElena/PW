using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PW.ViewModels
{
   public class AccountTransactionsForUser
    {

        [Required]
        public int PayeeId { get; set; }
        [Required]
        public string PayeeName { get; set; }
        [Required]
        public int RecipientId { get; set; }
        [Required]
        public string RecipientName { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string TimeOfTransaction { get; set; }
        [Required]
        public decimal Balance { get; set; }
        [Required]
        public bool Repeat { get; set; }


    }
}
