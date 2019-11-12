using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace EntityFramework.Npgsql.PostgreSql
{
    public class ContactContextFactory : IDesignTimeDbContextFactory<ContactContext>
    {
        public ContactContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContactContext>();

            var connectionString = "Host=localhost;User Id=postgres;Password=admin;Database=Contacts;";

            string schema = Guid.NewGuid().ToString();

            optionsBuilder.UseNpgsql(
                connectionString,
                b => b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema));

            optionsBuilder.ReplaceService<IMigrationsAssembly, ForceSchemaMigrationsAssembly>();

            return new ContactContext(optionsBuilder.Options, schema);
        }
    }
}
