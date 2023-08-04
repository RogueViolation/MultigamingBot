using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DataAccess.DTO
{
    public class CodeLookupDTO
    {
        public string Code { get; set; }
        public string CodeContent1 { get; set; }
        public string CodeCOntent2 { get; set; }
        public string CodeCOntent3 { get; set; }
        public bool RedeemStatus { get; set; }
        public string DateRedeem { get; set; }
        public string CodeSource { get; set; }
    }
}
