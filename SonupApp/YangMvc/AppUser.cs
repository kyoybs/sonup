using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace YangMvc
{
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Password { get; set; }

    }
}
