using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YangMvc;

namespace SonupApp.Models.Sys
{
    public class UserDb : DbBase
    {
        public UserDb()
        {
            this.Settings.Server = DbServer.MainDb;
        }

        public AppUser GetUser(string userName)
        {
            string sql = "SELECT * FROM AppUser WHERE UserName=@UserName";
            return FirstOrDefault<AppUser>(sql, new { UserName = userName });
        }
    }
}
