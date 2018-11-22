using Common.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace PW.DataModel.Entities
{
    public class AccountTransaction : IPWRepository, IEntityBase
    {
        [Required]
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public virtual Account Recipient { get; set; }
        [Required]
        public int PayeeId { get; set; }
        public virtual Account Payee { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime TimeOfTransaction { get; set; }
        [Required]
        public decimal PayeeBalance { get; set; }
        [Required]
        public decimal RecipientBalance { get; set; }
    }
}
