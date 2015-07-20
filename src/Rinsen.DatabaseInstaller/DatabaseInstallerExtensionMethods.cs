using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Rinsen.DatabaseInstaller.Sql;

namespace Rinsen.DatabaseInstaller
{
    public static class DatabaseInstallerExtensionMethods
    {
        public static void AddDatabaseInstaller(this IServiceCollection services, string connectionString)
        {
            var identityOptions = new InstallerOptions { ConnectionString = connectionString };
            services.AddDatabaseInstaller(identityOptions);

        }

        public static void AddDatabaseInstaller(this IServiceCollection services, InstallerOptions installerOptions)
        {
            if (string.IsNullOrEmpty(installerOptions.ConnectionString))
            {
                throw new ArgumentException("No connection string is provided");
            }

            services.AddInstance(installerOptions);

            services.AddTransient<Installer, Installer>();
            services.AddTransient<DatabaseVersionInstaller, DatabaseVersionInstaller>();
            services.AddTransient<DatabaseScriptRunner, DatabaseScriptRunner>();
            services.AddTransient<VersionHandler, VersionHandler>();
        }

        public static void RunDatabaseInstaller(this IApplicationBuilder app, IEnumerable<DatabaseVersion> databaseVersions)
        {
            app.ApplicationServices.GetRequiredService<Installer>().Run(databaseVersions);
        }

        public static Table AddNewTable(this List<IDbChange> dbChangeList, string tableName)
        {
            var table = new Table(tableName);
            dbChangeList.Add(table);
            return table;
        }

        public static TableAlteration AddNewTableAlteration(this List<IDbChange> dbChangeList, string tableAlterationName)
        {
            var table = new TableAlteration(tableAlterationName);
            dbChangeList.Add(table);
            return table;
        }
    }
}
