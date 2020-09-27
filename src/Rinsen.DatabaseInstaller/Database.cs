using System;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public class Database : IDbChange
    {
        private readonly List<LoginAndUserBuilder> _loginBuilders = new List<LoginAndUserBuilder>();
        public string DatabaseName { get; }

        public Database(string name)
        {
            DatabaseName = name;
        }

        public IReadOnlyList<string> GetDownScript(InstallerOptions installerOptions)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<string> GetUpScript(InstallerOptions installerOptions)
        {
            var result = new List<string>
            {
                $"IF '{DatabaseName}' NOT IN (SELECT [name] FROM [master].[sys].[databases] WHERE [name] NOT IN ('master', 'tempdb', 'model', 'msdb'))\r\nCREATE DATABASE {DatabaseName}",
                $"USE {DatabaseName}"
            };

            foreach (var loginBuilder in _loginBuilders)
            {
                if (loginBuilder.CreateNewLogin)
                {
                    result.Add($"IF '{loginBuilder.LoginName}' NOT IN (SELECT [name] FROM [master].[sys].[sql_logins])\r\nCREATE LOGIN Kalle WITH PASSWORD = '{loginBuilder.Password}'");
                }

                if (loginBuilder.CreateNewUser)
                {
                    result.Add($"IF '{loginBuilder.UserName}' NOT IN (SELECT [name] FROM [{DatabaseName}].[sys].[sysusers])\r\nCRATE USER {loginBuilder.UserName} FOR LOGIN {loginBuilder.LoginName}");
                }

                foreach (var roleMembership in loginBuilder.RoleMemberships)
                {
                    result.Add($"ALTER ROLE {roleMembership.Role} ADD MEMBER {roleMembership.UserName}");
                }
            }

            return result;
        }

        public LoginAndUserBuilder CreateUser(string userName)
        {
            var loginBuilder = new LoginAndUserBuilder();

            _loginBuilders.Add(loginBuilder);

            return loginBuilder.WithUser(userName);
        }

        public LoginAndUserBuilder CreateLogin(string loginName)
        {
            var loginBuilder = new LoginAndUserBuilder(loginName);
            _loginBuilders.Add(loginBuilder);

            return loginBuilder;
        }
    }
}
