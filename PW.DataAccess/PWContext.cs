using Common.Helpers;
using Microsoft.EntityFrameworkCore;
using PW.DataModel.Entities;
using PW.DataModel.Enums;
using System.Linq;
using System.Reflection.Emit;

namespace PW.DataAccess
{
    public class PWContext : DbContext, IPWRepository
    {
        public PWContext(DbContextOptions<PWContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            byte[] passwordHash, passwordSalt;
            PasswordHasher.CreatePasswordHash("1", out passwordHash, out passwordSalt);
            modelBuilder.Entity<User>().HasData(new User {
                Id = 1,
                UserName = "System",
                Email = "1@1.com",
                Type = UserType.System,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt}); 
            modelBuilder.Entity<Account>().HasData( new Account  {
                Id = 1,
                UserId = 1,
                Balance = 0
                });
         
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }

    }

    }

