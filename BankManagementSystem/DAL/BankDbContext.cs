using BankManagementSystem.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.DAL
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
        }

       
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Accounts)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SenderAccount) // Transaction has one Sender Account
                .WithMany() // Account may have many Transactions (optional: specify a navigation property if needed)
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ReceiverAccount) // Transaction has one Receiver Account
                .WithMany() // Receiver Account may have many Transactions (optional: specify a navigation property if needed)
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(a => a.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder
            .Entity<Account>()
            .Property(c => c.AccountType)
            .HasConversion(new EnumToStringConverter<AccountType>());

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionType)
                .HasConversion(new EnumToStringConverter<TransactionType>());

            modelBuilder.Entity<Customer>()
            .Property(c => c.Surname)
            .HasColumnOrder(3);

            modelBuilder.Entity<Customer>()
            .HasIndex(c => new { c.PanNo, c.AadharNo })
            .IsUnique();


        }
    }
}
