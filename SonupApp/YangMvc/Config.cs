using Microsoft.Extensions.Configuration;
using ShareCarApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YangMvc
{
    public static class Config
    {
        public static IConfiguration Configuration { get; set; }

        public static string Get(string key, string section= "AppSettings")
        {
            return Configuration[$"{section}:{key}"];
        }
    }
}
