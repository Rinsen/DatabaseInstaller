using System;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseSettings : IDbChange
    {
        private readonly List<SecurityBuilder> _securityBuilders = new();

        public IReadOnlyList<string> GetDownScript(InstallerOptions installerOptions)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<string> GetUpScript(InstallerOptions installerOptions)
        {
            var result = new List<string>
            {
                $"USE [{installerOptions.DatabaseName}]"
            };

            foreach (var securityBuilder in _securityBuilders)
            {
                if (securityBuilder.CreateNewLogin)
                {
                    result.Add($"IF '{securityBuilder.LoginName}' NOT IN (SELECT [name] FROM [master].[sys].[sql_logins]){Environment.NewLine}CREATE LOGIN {securityBuilder.LoginName} WITH PASSWORD = '{securityBuilder.Password}'");
                }

                if (securityBuilder.CreateNewUser)
                {
                    result.Add($"IF '{securityBuilder.UserName}' NOT IN (SELECT [name] FROM [{installerOptions.DatabaseName}].[sys].[sysusers]){Environment.NewLine}CREATE USER {securityBuilder.UserName} FOR LOGIN {securityBuilder.LoginName}");
                }

                foreach (var roleMembership in securityBuilder.RoleMemberships)
                {
                    result.Add($"ALTER ROLE {roleMembership.Role} ADD MEMBER {roleMembership.UserName}");
                }
            }

            return result;
        }

        /// <summary>
        /// Create a new database user
        /// </summary>
        /// <param name="userName">UserName for created database user.</param>
        /// <returns>New SecurityBuilder.</returns>
        public SecurityBuilder CreateUser(string userName)
        {
            var securityBuilder = new SecurityBuilder();

            _securityBuilders.Add(securityBuilder);

            return securityBuilder.WithUser(userName);
        }

        /// <summary>
        /// Create login with a random password generated at install time.
        /// </summary>
        /// <param name="loginName">Login name.</param>
        /// <returns>New instance of SecurityBuilder.</returns>
        public SecurityBuilder CreateLogin(string loginName)
        {
            var securityBuilder = new SecurityBuilder(loginName);
            _securityBuilders.Add(securityBuilder);

            return securityBuilder;
        }

        /// <summary>
        /// Create login with a specified password.
        /// </summary>
        /// <param name="loginName">Login name.</param>
        /// <param name="password">Login password.</param>
        /// <returns>New instance of SecurityBuilder.</returns>
        public SecurityBuilder CreateLogin(string loginName, string password)
        {
            var securityBuilder = new SecurityBuilder(loginName, password);
            _securityBuilders.Add(securityBuilder);

            return securityBuilder;
        }
    }
}
