using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Rinsen.DatabaseInstaller
{
    public class Database : IDbChange
    {
        private string _databaseName;
        private bool _createNewDatabase = false;
        private bool _createNewUser = false;
        private string _userName;
        private string _forLoginName;
        private bool _createNewLogin = false;
        private string _loginName;
        private string _password;
        private readonly List<RoleMembership> _roleMembershipsToAdd = new List<RoleMembership>();
        private readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        public Database(string name)
        {
            _databaseName = name;
        }

        public void CreateDatabase()
        {
            _createNewDatabase = true;
        }

        public List<string> GetDownScript()
        {
            throw new NotImplementedException();
        }

        public List<string> GetUpScript()
        {
            var result = new List<string>();

            if (_createNewDatabase)
            {
                result.Add($"IF '{_databaseName}' NOT IN (SELECT [name] FROM [master].[sys].[databases] WHERE [name] NOT IN ('master', 'tempdb', 'model', 'msdb'))\r\nCREATE DATABASE {_databaseName}");
            }

            result.Add($"USE {_databaseName}");

            if (_createNewLogin)
            {
                _password = GetRandomString(40);
                result.Add($"CREATE LOGIN {_loginName} WITH PASSWORD = '{_password}'");
            }

            if (_createNewUser)
            {
                if (string.IsNullOrEmpty(_forLoginName))
                {
                    result.Add($"CRATE USER {_userName} FOR LOGIN {_loginName}");
                }
                else
                {
                    result.Add($"CRATE USER {_userName} FOR LOGIN {_forLoginName}");
                }
            }

            foreach (var roleMembership in _roleMembershipsToAdd)
            {
                result.Add($"ALTER ROLE {roleMembership.Role} ADD MEMBER {roleMembership.UserName}");
            }

            return result;
        }

        public void AddRoleMembershipDataWriter(string userName)
        {
            _roleMembershipsToAdd.Add(new RoleMembership("db_datawriter", userName));
        }

        public void AddRoleMembershipDataReader(string userName)
        {
            _roleMembershipsToAdd.Add(new RoleMembership("db_datareader", userName));
        }

        public void CreateLogin(string loginName)
        {
            _createNewLogin = true;
            _loginName = loginName;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="forLoginName">LoginName from CreateLogin will be used if null</param>
        public void CreateUser(string userName, string forLoginName = null)
        {
            _createNewUser = true;
            _userName = userName;

            if (string.IsNullOrEmpty(_loginName))
            {
                throw new ArgumentException($"{nameof(forLoginName)} cannot be null when LoginName is null or empty");
            }

            _forLoginName = forLoginName;
        }

        internal string GetGeneratedPassword()
        {
            return _password;
        }

        private string GetRandomString(int length)
        {
            var bytes = new byte[length];

            CryptoRandom.GetBytes(bytes);

            return WebEncoders.Base64UrlEncode(bytes);
        }

        private class RoleMembership
        {
            public RoleMembership(string role, string userName)
            {
                Role = role;
                UserName = userName;
            }

            public string Role { get; }

            public string UserName { get; }
        }
    }
}
