using Microsoft.AspNetCore.WebUtilities;
using Rinsen.DatabaseInstaller.Internal;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Rinsen.DatabaseInstaller
{
    public class SecurityBuilder
    {
        private readonly RandomNumberGenerator _cryptoRandom = RandomNumberGenerator.Create();


        internal SecurityBuilder()
        {
            CreateNewLogin = false;
        }

        internal SecurityBuilder(string loginName, string password = "")
        {
            if (string.IsNullOrEmpty(password))
            {
                Password = GetRandomString(40);
            }
            else
            {
                Password = password;
            }
            
            _loginName = loginName;
            CreateNewLogin = true;
        }

        public string LoginName 
        { 
            get 
            {
                if (string.IsNullOrEmpty(_loginName))
                {
                    return UserName;
                }
                return _loginName;
            }
        }

        public bool CreateNewLogin { get; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public bool CreateNewUser { get; private set; } = false;
        
        private string _loginName;

        public IReadOnlyList<RoleMembership> RoleMemberships { get { return _roleMembershipsToAdd; } }

        private readonly List<RoleMembership> _roleMembershipsToAdd = new();

        private string GetRandomString(int length)
        {
            var bytes = new byte[length];

            _cryptoRandom.GetBytes(bytes);

            return WebEncoders.Base64UrlEncode(bytes);
        }

        public SecurityBuilder AddRoleMembershipDataWriter()
        {
            _roleMembershipsToAdd.Add(new RoleMembership("db_datawriter", UserName));

            return this;
        }

        public SecurityBuilder AddRoleMembershipDataReader()
        {
            _roleMembershipsToAdd.Add(new RoleMembership("db_datareader", UserName));

            return this;
        }

        public SecurityBuilder WithUser(string userName)
        {
            CreateNewUser = true;
            UserName = userName;

            return this;
        }

        public SecurityBuilder WithUser()
        {
            WithUser(LoginName);

            return this;
        }

        public void ForLogin(string loginName)
        {
            _loginName = loginName;
        }

        public void ForLogin(string loginName, string password)
        {
            _loginName = loginName;
            Password = password;
        }
    }
}
