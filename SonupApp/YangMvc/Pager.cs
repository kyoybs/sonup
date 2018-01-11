using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YangMvc
{
    public class Pager
    {
        public int PageIndex { get; set; }

        public int PageNo
        {
            get
            {
                return PageIndex + 1;
            }
            set
            {
                PageIndex = value - 1;
            }
        }

        public int PageSize { get; set; } = 50;

        public int RowCount { get; set; }

        public string OrderBy { get; set; }

        public int PageCount
        {
            get
            {
                if (PageSize <= 0)
                {
                    return 1;
                }
                return (RowCount - 1) / PageSize + 1;
            }
        }
    }
}
