using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Npgsql.PostgreSql
{
    public class ContactContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public string Schema { get; set; }

        public ContactContext(DbContextOptions options, string schema)
            : base(options)
        {
            Schema = schema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!string.IsNullOrWhiteSpace(Schema))
            {
                modelBuilder.HasDefaultSchema(Schema);
            }

            modelBuilder.Entity<Contact>(b =>
            {
                b.HasKey(c => c.Id);

                b.Property(c => c.Name)
                    .IsRequired();

                b.Property(c => c.Phone)
                    .IsRequired();
            });
        }
    }

    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
