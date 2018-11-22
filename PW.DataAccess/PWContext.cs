using Microsoft.EntityFrameworkCore;
using PW.DataModel.Entities;


namespace PW.DataAccess
{
    public class PWContext : DbContext, IPWRepository
    {
        public PWContext(DbContextOptions<PWContext> options)
         : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }




    }

}