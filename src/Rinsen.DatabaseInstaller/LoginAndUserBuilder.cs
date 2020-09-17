using Microsoft.AspNetCore.WebUtilities;
using Rinsen.DatabaseInstaller.Internal;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Rinsen.DatabaseInstaller
{
    public class LoginAndUserBuilder
    {
        private readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();


        public LoginAndUserBuilder()
        {
            CreateNewLogin = false;
        }

        public LoginAndUserBuilder(string loginName)
        {
            Password = GetRandomString(40);
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
        public string Password { get; }
        public bool CreateNewUser { get; private set; } = false;
        
        private string _loginName;

        public IReadOnlyList<RoleMembership> RoleMemberships { get { return _roleMembershipsToAdd; } }

        private readonly List<RoleMembership> _roleMembershipsToAdd = new List<RoleMembership>();

        private string GetRandomString(int length)
        {
            var bytes = new byte[length];

            CryptoRandom.GetBytes(bytes);

            return WebEncoders.Base64UrlEncode(bytes);
        }

        public LoginAndUserBuilder AddRoleMembershipDataWriter()
        {
            _roleMembershipsToAdd.Add(new RoleMembership("db_datawriter", UserName));

            return this;
        }

        public LoginAndUserBuilder AddRoleMembershipDataReader()
        {
            _roleMembershipsToAdd.Add(new RoleMembership("db_datareader", UserName));

            return this;
        }

        public LoginAndUserBuilder WithUser(string userName)
        {
            CreateNewUser = true;
            UserName = userName;

            return this;
        }

        public LoginAndUserBuilder WithUser()
        {
            WithUser(LoginName);

            return this;
        }

        public void ForLogin(string loginName)
        {
            _loginName = loginName;
        }
    }
}
