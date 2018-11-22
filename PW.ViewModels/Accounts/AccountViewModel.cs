using PW.DataModel.Entities;
using System.ComponentModel.DataAnnotations;


namespace PW.ViewModels
{
    public class AccountViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }

        private User User { get; set; }

        public string UserName { get { return User.UserName; } set { User.UserName = value; } }
        public string Email { get { return User.Email; } set { User.Email = value; } }

        public decimal Balance { get; set; }

    }
}
