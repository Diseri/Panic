using AASA.NetCore.Lib.Helper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AASA.NetCore.IDomainService.Panic.Models
{
    public class DispatchHistory
    {
        public string WorkOrderNumber { get; set; }
        public string Status { get; set; }
        public string StatusTime { get; set; }
    }

    public class DispatchHistoryResponse : ResultBase
    {
        public List<DispatchHistory> Data { get; set; }
    }
}
