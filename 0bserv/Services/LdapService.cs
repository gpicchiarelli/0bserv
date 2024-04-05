using System.DirectoryServices.Protocols;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System.DirectoryServices;
using System.Web.Providers.Entities;

namespace _0bserv.Services
{
    public class LdapConfig
    {
        public string Path { get; set; }
        public string UserDomainName { get; set; }
    }
    public interface IAuthenticationService
    {
        User Login(string userName, string password);
    }
    public class User
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
    }

    public class LdapAuthenticationService : IAuthenticationService
    {
        private const string DisplayNameAttribute = "DisplayName";
        private const string SAMAccountNameAttribute = "SAMAccountName";

        private readonly LdapConfig config;

        public LdapAuthenticationService(IOptions<LdapConfig> config)
        {
            this.config = config.Value;
        }
        public User Login(string userName, string password)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry(config.Path, config.UserDomainName + "\\" + userName, password))
                {
                    using (DirectorySearcher se = new DirectorySearcher(entry))
                    {
                        se.Filter = String.Format("({0}={1})", SAMAccountNameAttribute, userName);
                        se.PropertiesToLoad.Add(DisplayNameAttribute);
                        se.PropertiesToLoad.Add(SAMAccountNameAttribute);
                        var result = se.FindOne();
                        if (result != null)
                        {
                            var displayName = result.Properties[DisplayNameAttribute];
                            var samAccountName = result.Properties[SAMAccountNameAttribute];

                            return new User
                            {
                                DisplayName = displayName == null || displayName.Count <= 0 ? null : displayName[0].ToString(),
                                UserName = samAccountName == null || samAccountName.Count <= 0 ? null : samAccountName[0].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if we get an error, it means we have a login failure.  
                // Log specific exception  
            }
            return null;
        }
    }

}
