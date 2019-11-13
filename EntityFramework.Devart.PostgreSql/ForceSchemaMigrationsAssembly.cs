using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Collections.Generic;
using System.Reflection;

namespace EntityFramework.Devart.PostgreSql
{
    public class ForceSchemaMigrationsAssembly : MigrationsAssembly
    {
        private readonly ICurrentDbContext _currentContext;

        public ForceSchemaMigrationsAssembly(
            ICurrentDbContext currentContext,
            IDbContextOptions options,
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
            : base(currentContext, options, idGenerator, logger)
        {
            _currentContext = currentContext;
        }

        public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
        {
            var migration = base.CreateMigration(migrationClass, activeProvider);
            var schema = _currentContext.Context.Model.Relational().DefaultSchema;

            if (!string.IsNullOrWhiteSpace(schema))
            {
                foreach (var upOperation in migration.UpOperations)
                {
                    SetSchema(upOperation, schema);
                }

                foreach (var downOperation in migration.DownOperations)
                {
                    SetSchema(downOperation, schema);
                }
            }

            return migration;
        }

        private void SetSchema(MigrationOperation operation, string schema)
        {
            var opertationType = operation.GetType();
            var schemaProperty = opertationType.GetProperty("Schema");

            if (schemaProperty != null && schemaProperty.CanWrite)
            {
                schemaProperty.SetValue(operation, schema);
            }

            schemaProperty = opertationType.GetProperty("PrincipalSchema");

            if (schemaProperty != null && schemaProperty.CanWrite)
            {
                schemaProperty.SetValue(operation, schema);
            }

            var properties = opertationType.GetProperties();

            foreach (var property in properties)
            {
                if (property.CanRead && property.GetIndexParameters().Length == 0)
                {
                    var propertyValue = property.GetValue(operation);

                    if (propertyValue is IEnumerable<MigrationOperation> items)
                    {
                        foreach (var item in items)
                        {
                            if (item is AddColumnOperation addColumnOperation)
                            {
                                //if (addColumnOperation.Name != "Id")
                                {
                                    SetSchema(item, schema);
                                }
                            }
                            else
                            {
                                SetSchema(item, schema);
                            }
                        }
                    }
                    else if (propertyValue is MigrationOperation migrationOperation)
                    {
                        SetSchema(migrationOperation, schema);
                    }
                }
            }
        }
    }
}
