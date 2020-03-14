using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

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

        public static async Task RunDatabaseInstaller(this IApplicationBuilder app, List<DatabaseVersion> databaseVersions)
        {
            await app.ApplicationServices.GetRequiredService<Installer>().RunAsync(databaseVersions);
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

        public static Index AddNewClusteredIndex(this List<IDbChange> dbChangeList, string name, string tableName)
        {
            var index = new Index(name, tableName).Clustered();
            dbChangeList.Add(index);
            return index;
        }

        public static Index<T> AddNewClusteredIndex<T>(this List<IDbChange> dbChangeList, string name) where T : class
        {
            var tableName = typeof(T).Name + "s";
            return dbChangeList.AddNewClusteredIndex<T>(name, tableName);
        }

        public static Index<T> AddNewClusteredIndex<T>(this List<IDbChange> dbChangeList, string name, string tableName) where T : class
        {
            var index = new Index<T>(name, tableName).Clustered();
            dbChangeList.Add(index);
            return index;
        }

        public static Index AddNewUniqueIndex(this List<IDbChange> dbChangeList, string name, string tableName)
        {
            var index = new Index(name, tableName).Unique();
            dbChangeList.Add(index);
            return index;
        }

        public static Index<T> AddNewUniqueIndex<T>(this List<IDbChange> dbChangeList, string name) where T : class
        {
            var tableName = typeof(T).Name + "s";

            return dbChangeList.AddNewUniqueIndex<T>(name, tableName);
        }

        public static Index<T> AddNewUniqueIndex<T>(this List<IDbChange> dbChangeList, string name, string tableName) where T : class
        {
            var Index = new Index<T>(name, tableName).Unique();
            dbChangeList.Add(Index);
            return Index;
        }

        public static Index AddNewUniqueClusteredIndex(this List<IDbChange> dbChangeList, string name, string tableName)
        {
            var index = new Index(name, tableName).Unique().Clustered();
            dbChangeList.Add(index);
            return index;
        }

        public static Index<T> AddNewUniqueClusteredIndex<T>(this List<IDbChange> dbChangeList, string name) where T : class
        {
            var tableName = typeof(T).Name + "s";

            return dbChangeList.AddNewUniqueClusteredIndex<T>(name, tableName);
        }

        public static Index<T> AddNewUniqueClusteredIndex<T>(this List<IDbChange> dbChangeList, string name, string tableName) where T : class
        {
            var Index = new Index<T>(name, tableName).Unique().Clustered();
            dbChangeList.Add(Index);
            return Index;
        }

        public static AddValue AddColumnValue(this List<IDbChange> dbChangeList, string tableName)
        {
            var addValue = new AddValue(tableName);
            dbChangeList.Add(addValue);

            return addValue;
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
