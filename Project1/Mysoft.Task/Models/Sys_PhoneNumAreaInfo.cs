using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    class Sys_PhoneNumAreaInfo
    {
        public int F_Id { get; set; }
        public string F_NumSegment { get; set; }
        public string F_Province { get; set; }

        public string F_City { get; set; }
        public string F_Operator { get; set; }
        public string F_AreaCode { get; set; }
        public string F_PostCode { get; set; }
    }
}
