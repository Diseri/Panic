using AASA.NetCore.Lib.Helper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AASA.NetCore.IDomainService.Panic.Models
{
    public class PanicActiveWorkOrders
    {
        public string WorkOrderNumber { get; set; }
        public string Status { get; set; }
        public string WorkOrderType { get; set; }
        public string DispatchedVendor { get; set; }
        public string TrackingURL { get; set; }
    }

    public class PanicActiveWorkOrderResult
    {
        public List<PanicActiveWorkOrders> WorkOrders { get; set; }
    }
    public class PanicActiveWorkOrderResponse : ResultBase
    {
        public PanicActiveWorkOrderResult Data { get; set; }
    }
}
