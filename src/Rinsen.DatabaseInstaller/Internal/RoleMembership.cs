using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.DatabaseInstaller.Internal
{
    public class RoleMembership
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
