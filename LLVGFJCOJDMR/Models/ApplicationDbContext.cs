using Microsoft.EntityFrameworkCore;

namespace LLVGFJCOJDMR.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasMany(p => p.PhoneNumbers)
                .WithOne(d => d.Customer)
                .OnDelete(DeleteBehavior.Cascade);

            //insertar datos inicales a la tabla rols
            modelBuilder.Entity<Rol>().HasData(new Rol { Id = 1, Name = "Administrador", Description = "soy admin" });
            modelBuilder.Entity<Rol>().HasData(new Rol { Id = 2, Name = "Empleado", Description = "soy empleado" });
            modelBuilder.Entity<Rol>().HasData(new Rol { Id = 3, Name = "Gerente", Description = "soy gerente" });

            modelBuilder.Entity<User>().HasData(new User { Id = 1, UserName = "Root", RolId = 1, Password = "be4c75f3853a9f55a0b27fbaeb1a0dde", Email = "root@gmail.com", Status = 1, Image = null });
        }

    }
}
