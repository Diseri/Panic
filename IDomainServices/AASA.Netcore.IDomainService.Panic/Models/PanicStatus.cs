using AASA.NetCore.Lib.Helper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AASA.NetCore.IDomainService.Panic.Models
{
    public class PanicStatusResult
    {
        public string Status { get; set; }
    }
    public class ResultPanicStatusResponse : ResultBase
    {
        public PanicStatusResult Data { get; set; }
    }
}
