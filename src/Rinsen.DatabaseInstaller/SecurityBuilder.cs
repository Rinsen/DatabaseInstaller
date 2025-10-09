using Microsoft.AspNetCore.WebUtilities;
using Rinsen.DatabaseInstaller.Internal;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Rinsen.DatabaseInstaller
{
    public class SecurityBuilder
    {
        private readonly RandomNumberGenerator _cryptoRandom = RandomNumberGenerator.Create();

        /// <summary>
        /// Initializes a new instance of the SecurityBuilder class.
        /// </summary>
        public SecurityBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SecurityBuilder class using the specified login name and password.
        /// </summary>
        /// <param name="loginName">The login name to associate with the new security context. Cannot be null or empty.</param>
        /// <param name="password">The password to use for the security context. If null or empty, a random password is generated
        /// automatically.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="loginName"/> is null or empty.</exception>
        internal SecurityBuilder(string loginName, string password = "")
        {
            if (string.IsNullOrEmpty(loginName))
            {
                throw new ArgumentException("Login name cannot be null or empty.", nameof(loginName));
            }

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

        public bool CreateNewLogin { get; } = false;
        public string UserName { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public bool CreateNewUser { get; private set; } = false;
        
        private string _loginName = string.Empty;

        public IReadOnlyList<RoleMembership> RoleMemberships { get { return _roleMembershipsToAdd; } }

        private readonly List<RoleMembership> _roleMembershipsToAdd = [];

        private string GetRandomString(int length)
        {
            var bytes = new byte[length];

            _cryptoRandom.GetBytes(bytes);

            return WebEncoders.Base64UrlEncode(bytes);
        }

        public SecurityBuilder AddRoleMembershipDataWriter()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                throw new InvalidOperationException("UserName cannot be null or empty when adding role membership.");
            }

            _roleMembershipsToAdd.Add(new RoleMembership("db_datawriter", UserName));

            return this;
        }

        public SecurityBuilder AddRoleMembershipDataReader()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                throw new InvalidOperationException("UserName cannot be null or empty when adding role membership.");
            }

            _roleMembershipsToAdd.Add(new RoleMembership("db_datareader", UserName));

            return this;
        }

        public SecurityBuilder WithUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
            }

            CreateNewUser = true;
            UserName = userName;

            return this;
        }

        public SecurityBuilder WithUser()
        {
            if (string.IsNullOrWhiteSpace(LoginName))
            {
                throw new InvalidOperationException("LoginName cannot be null or empty when creating user with login name.");
            }

            WithUser(LoginName);

            return this;
        }

        public void ForLogin(string loginName)
        {
            if (string.IsNullOrWhiteSpace(loginName))
            {
                throw new ArgumentException("Login name cannot be null or empty.", nameof(loginName));
            }

            _loginName = loginName;
        }

        public void ForLogin(string loginName, string password)
        {
            if (string.IsNullOrWhiteSpace(loginName))
            {
                throw new ArgumentException("Login name cannot be null or empty.", nameof(loginName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            _loginName = loginName;
            Password = password;
        }
    }
}
