using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Rinsen.DatabaseInstaller.Sql;
using System.Linq.Expressions;
using System.Reflection;
using Rinsen.DatabaseInstaller.Generic.Sql;

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

        public static Table<T> AddNewTable<T>(this List<IDbChange> dbChangeList) where T : class
        {   
            var name = typeof(T).Name + "s";

            var table = new Table<T>(name);

            dbChangeList.Add(table);

            return table;
        }

        public static string GetMemberName<T>(this Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression.Body is UnaryExpression)
            {
                return ((MemberExpression)((UnaryExpression)propertyExpression.Body).Operand).Member.Name;
            }
            else if (propertyExpression.Body is MemberExpression)
            {
                return ((MemberExpression)propertyExpression.Body).Member.Name;
            }
            else
            {
                throw new NotSupportedException("Unknown Expression type");
            }
        }

        public static Type GetMemberType<T>(this Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression.Body is UnaryExpression)
            {
                return ((UnaryExpression)propertyExpression.Body).Operand.Type;
            }
            else if (propertyExpression.Body is MemberExpression)
            {
                return ((PropertyInfo)((MemberExpression)propertyExpression.Body).Member).PropertyType;
            }
            else
            {
                throw new NotSupportedException("Unknown Expression type");
            }
        }
    }
}
