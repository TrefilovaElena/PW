using Common.Data;
using System.ComponentModel.DataAnnotations;

namespace PW.DataModel.Entities
{
    public class AppUser : IPWRepository, IEntityBase
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

    }
}
