using ITS_BE.Enumerations;
using ITS_BE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITS_BE.Data
{
    public class MyDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole,
        IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Product_Color> Product_Colors { get; set; }
        public DbSet<Product_Details> Product_Details { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetials { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogDetail> LogDetails { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        //public DbSet<User> Users {  get; set; }
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(e =>
            {
                e.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
            });
            builder.Entity<Role>(e =>
            {
                e.HasMany(x => x.UserRoles)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();
            });

            var rolesList = Enum.GetNames(typeof(RolesEnum)).Select(e => new Role
            {
                Name = e,
                NormalizedName = e.ToUpper()
            }).ToArray();
            builder.Entity<Role>().HasData(rolesList);

            var user = new User
            {
                FullName = "Nhựt Hòa",
                Email = "hoab2005755@student.ctu.edu.vn",
                NormalizedEmail = "hoab2005755@student.ctu.edu.vn",
                UserName = "hoab2005755@student.ctu.edu.vn",
                NormalizedUserName = "hoab2005755@student.ctu.edu.vn",
                PhoneNumber = "0944990152",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreateAt = DateTime.Now,
            };
            var employee = new User
            {
                FullName = "Chân Chân",
                Email = "lethinhachan18@gmail.com",
                NormalizedEmail = "lethinhachan18@gmail.com",
                UserName = "lethinhachan18@gmail.com",
                NormalizedUserName = "lethinhachan18@gmail.com",
                PhoneNumber = "0901089182",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreateAt = DateTime.Now,
            };

            builder.Entity<User>().HasData(user, employee);

            var userRole1 = new UserRole
            {
                RoleId = rolesList[0].Id,
                UserId = user.Id
            };
            var userRole2 = new UserRole
            {
                RoleId = rolesList[1].Id,
                UserId = employee.Id
            };
            builder.Entity<UserRole>().HasData(userRole1, userRole2);
        }
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IBaseEntity
                    && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((IBaseEntity)entry.Entity).CreateAt = DateTime.Now;
                }
                if (entry.State == EntityState.Modified)
                {
                    ((IBaseEntity)entry.Entity).UpdateAt = DateTime.Now;
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
