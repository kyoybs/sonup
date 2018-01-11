using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YangMvc
{ 
    public class SysErrorLog
    {
        [Key]
        public int ErrorLogId { get; set; }

        public bool IsDebug { get; set; }

        public bool IsSql { get; set; }

        public string UserMsg { get; set; }

        public string LogTitle { get; set; }

        public string LogContent { get; set; }

        public string RequestUrl { get; set; }

        public string FormData { get; set; }

        public DateTime LogTime { get; set; }

        public int? UserId { get; set; }

        public string ResultType { get; set; }
    }
}
