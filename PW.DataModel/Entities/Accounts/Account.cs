using Common.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PW.DataModel.Entities
{
    public class Account : IPWRepository, IEntityBase
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        public decimal Balance { get; set; }
    }
}
