using Monocle.Api;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Config
{
    public class MonocleConfiguration : IRocketPluginConfiguration
    {
        public string BindAddress { get; set; }
        public string CertificatePath { get; set; }
        public int ListenPort { get; set; }
        public bool UseSSL { get; set; }
        public List<AuthorizedUser> AuthorizedUsers { get; set; }
        public int MaxFailedLoginAttempts { get; set; }

        public void LoadDefaults()
        {
            var defaultAdmin = new AuthorizedUser()
            {
                Username = "admin",
                Password = "potato",
                Type = AuthorizedUserType.Administrator
            };

            ListenPort = 55554;
            BindAddress = "127.0.0.1";
            AuthorizedUsers = new List<AuthorizedUser>() { defaultAdmin };
            MaxFailedLoginAttempts = 5;
            UseSSL = true;
            CertificatePath = "";
        }
    }

    public class AuthorizedUser
    {
        public string? Username { get; set; }
        public string? Password { get; set; } // TODO: Maybe use password hash
        public AuthorizedUserType? Type { get; set; }
    }
}
