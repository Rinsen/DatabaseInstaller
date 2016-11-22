using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Rinsen.DatabaseInstaller.Sql;
using System.Linq.Expressions;
using System.Reflection;
using Rinsen.DatabaseInstaller.Sql.Generic;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddSingleton(installerOptions);

            services.AddTransient<Installer, Installer>();
            services.AddTransient<DatabaseVersionInstaller, DatabaseVersionInstaller>();
            services.AddTransient<DatabaseScriptRunner, DatabaseScriptRunner>();
            services.AddTransient<VersionHandler, VersionHandler>();
            services.AddTransient<IVersionStorage, AdoNetVersionStorage>();
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

        public static Table<T> AddNewTable<T>(this List<IDbChange> dbChangeList) where T : class
        {
            var name = typeof(T).Name + "s";

            return dbChangeList.AddNewTable<T>(name);
        }

        public static Table<T> AddNewTable<T>(this List<IDbChange> dbChangeList, string tableName) where T : class
        {
            var table = new Table<T>(tableName);
            dbChangeList.Add(table);
            return table;
        }

        public static TableAlteration AddNewTableAlteration(this List<IDbChange> dbChangeList, string tableAlterationName)
        {
            var table = new TableAlteration(tableAlterationName);
            dbChangeList.Add(table);
            return table;
        }

        public static TableAlteration<T> AddNewTableAlteration<T>(this List<IDbChange> dbChangeList) where T : class
        {
            var name = typeof(T).Name + "s";

            return dbChangeList.AddNewTableAlteration<T>(name);
        }

        public static TableAlteration<T> AddNewTableAlteration<T>(this List<IDbChange> dbChangeList, string tableName) where T : class
        {
            var table = new TableAlteration<T>(tableName);
            dbChangeList.Add(table);
            return table;
        }

        public static Index AddNewIndex(this List<IDbChange> dbChangeList, string name, string tableName)
        {
            var index = new Index(name, tableName);
            dbChangeList.Add(index);
            return index;
        }

        public static Index<T> AddNewIndex<T>(this List<IDbChange> dbChangeList, string name) where T : class
        {
            var tableName = typeof(T).Name + "s";

            return dbChangeList.AddNewIndex<T>(name, tableName);
        }

        public static Index<T> AddNewIndex<T>(this List<IDbChange> dbChangeList, string name, string tableName) where T : class
        {
            var Index = new Index<T>(name, tableName);
            dbChangeList.Add(Index);
            return Index;
        }

        public static ClusteredIndex AddNewClusteredIndex(this List<IDbChange> dbChangeList, string name, string tableName)
        {
            var index = new ClusteredIndex(name, tableName);
            dbChangeList.Add(index);
            return index;
        }

        public static ClusteredIndex<T> AddNewClusteredIndex<T>(this List<IDbChange> dbChangeList, string name) where T : class
        {
            var tableName = typeof(T).Name + "s";

            return dbChangeList.AddNewClusteredIndex<T>(name, tableName);
        }

        public static ClusteredIndex<T> AddNewClusteredIndex<T>(this List<IDbChange> dbChangeList, string name, string tableName) where T : class
        {
            var Index = new ClusteredIndex<T>(name, tableName);
            dbChangeList.Add(Index);
            return Index;
        }

        public static UniqueIndex AddNewUniqueIndex(this List<IDbChange> dbChangeList, string name, string tableName)
        {
            var index = new UniqueIndex(name, tableName);
            dbChangeList.Add(index);
            return index;
        }

        public static UniqueIndex<T> AddNewUniqueIndex<T>(this List<IDbChange> dbChangeList, string name) where T : class
        {
            var tableName = typeof(T).Name + "s";

            return dbChangeList.AddNewUniqueIndex<T>(name, tableName);
        }

        public static UniqueIndex<T> AddNewUniqueIndex<T>(this List<IDbChange> dbChangeList, string name, string tableName) where T : class
        {
            var Index = new UniqueIndex<T>(name, tableName);
            dbChangeList.Add(Index);
            return Index;
        }

        public static UniqueClusteredIndex AddNewUniqueClusteredIndex(this List<IDbChange> dbChangeList, string name, string tableName)
        {
            var index = new UniqueClusteredIndex(name, tableName);
            dbChangeList.Add(index);
            return index;
        }

        public static UniqueClusteredIndex<T> AddNewUniqueClusteredIndex<T>(this List<IDbChange> dbChangeList, string name) where T : class
        {
            var tableName = typeof(T).Name + "s";

            return dbChangeList.AddNewUniqueClusteredIndex<T>(name, tableName);
        }

        public static UniqueClusteredIndex<T> AddNewUniqueClusteredIndex<T>(this List<IDbChange> dbChangeList, string name, string tableName) where T : class
        {
            var Index = new UniqueClusteredIndex<T>(name, tableName);
            dbChangeList.Add(Index);
            return Index;
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
