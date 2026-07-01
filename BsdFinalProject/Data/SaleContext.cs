using BsdFinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Data
{
    public class SaleContext : DbContext
    {
        public SaleContext(DbContextOptions<SaleContext> options):base(options) { }
        public DbSet<Basket> Basket => Set<Basket>();
        public DbSet<Card> Card => Set<Card>();
        public DbSet<Category> Category => Set<Category>();
        public DbSet<Donor> Donor => Set<Donor>();
        public DbSet<Gift> Gift => Set<Gift>();
        public DbSet<Manager> Manager => Set<Manager>();
        public DbSet<User> User => Set<User>();
        public DbSet<Winner> Winner => Set<Winner>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        // Basket configuration
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId)
                .IsRequired(); // UserId must be required

            entity.Property(e => e.GiftId)
                .IsRequired(); // GiftId must be required

            entity.HasOne(e => e.User)
                .WithMany() // Assuming a User can have many Baskets
                .HasForeignKey(e => e.UserId) // Defining the foreign key relationship
                .OnDelete(DeleteBehavior.Cascade); // Optional: cascade delete
            entity.HasOne(e => e.Gift)
                .WithMany()
                .HasForeignKey(e => e.GiftId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // Card configuration
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId)
                .IsRequired(); 

            entity.Property(e => e.GiftId)
                .IsRequired(); 
            entity.Property(e => e.BuingDate);
              
            entity.HasOne(e => e.Gift)
                .WithMany() 
                .HasForeignKey(e => e.GiftId) 
                .OnDelete(DeleteBehavior.Cascade); 

            entity.HasOne(e => e.User)
                .WithMany() 
                .HasForeignKey(e => e.UserId) 
                .OnDelete(DeleteBehavior.Cascade); 
        });
        
        // Category configuration
        modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasMany(e => e.GiftsList)
                      .WithOne(e => e.Category)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); 

            });
            // Donor configuration
            modelBuilder.Entity<Donor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.EMail).IsUnique();
                entity.HasIndex(e => e.EMail).IsUnique();
                entity.HasMany(e => e.GiftsList)
                      .WithOne(e => e.Donor)
                      .HasForeignKey(e => e.DonorId)
                      .OnDelete(DeleteBehavior.Cascade); 

            });
            // Gift configuration
            modelBuilder.Entity<Gift>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(300);
                entity.Property(e => e.Cost).IsRequired().HasDefaultValue(30);
                entity.HasCheckConstraint("CK_Gift_Cost", "Cost > 10 AND Cost < 100");
                entity.Property(e => e.Picture).HasMaxLength(300);
                entity.Property(e => e.WinnerName).HasMaxLength(100).HasDefaultValue("");
                entity.HasMany(e => e.CardsList)
                      .WithOne(e => e.Gift)
                      .HasForeignKey(e => e.GiftId)
                      .OnDelete(DeleteBehavior.Cascade);

            });
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.BasketList)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.CardsList)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); 
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.EMail).IsUnique();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.HasIndex(e => e.EMail).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue(Role.User);

            });

            // Manager configuration
            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(200);
            });


            // Winner configuration
            modelBuilder.Entity<Winner>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IdUser).IsRequired();
                entity.Property(e => e.IdGift).IsRequired();

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.IdUser)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Gift)
                      .WithMany()
                      .HasForeignKey(e => e.IdGift)
                      .OnDelete(DeleteBehavior.Restrict);
            });





        }
    }
}
