using Microsoft.EntityFrameworkCore;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(a => a.Email).IsUnique();

            entity.Property(a => a.Balance)
                  .HasColumnType("decimal(18,2)");

            entity.Property(a => a.AccountHolderName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(a => a.Email).IsRequired();
            entity.Property(a => a.PasswordHash).IsRequired();
            entity.Property(a => a.PinHash).IsRequired();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.Amount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(t => t.Type)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(t => t.Description)
                  .HasMaxLength(250);

            entity.Property(t => t.Date)
                  .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<User>()
            .HasMany(u => u.Accounts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Account>()
            .HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}