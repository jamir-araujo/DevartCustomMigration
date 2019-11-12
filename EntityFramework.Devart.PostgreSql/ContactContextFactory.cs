using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using Tnf.Drivers.DevartPostgreSQL;

namespace EntityFramework.Devart.PostgreSql
{
    public class ContactContextFactory : IDesignTimeDbContextFactory<ContactContext>
    {
        public ContactContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContactContext>();

            var connectionString = "Data Source=localhost;User Id=postgres;Password=admin;Database=Contacts;Protocol=ver20;Unicode=true";

            string schema = Guid.NewGuid().ToString();

            optionsBuilder.UsePostgreSql(
                connectionString,
                b => b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema));

            optionsBuilder.ReplaceService<IMigrationsAssembly, ForceSchemaMigrationsAssembly>();

            return new ContactContext(optionsBuilder.Options, schema);
        }
    }
}
