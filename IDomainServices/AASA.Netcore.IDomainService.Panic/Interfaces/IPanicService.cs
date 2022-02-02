using AASA.NetCore.IDomainService.Panic.Models;
using AASA.NetCore.IDomainService.Panic.RabbitMQ;
using AASA.NetCore.Lib.Helper.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AASA.NetCore.IDomainService.Panic.Interfaces
{
    public interface IPanicService
    {
        public Guid? ImpersonateUserId { get; set; }
        public Task<ResultPanicResponse> Create(PanicRequestModel panic, IRabbitManager manager);
        public Task<ResultPanicResponse> Cancel(PanicCancellation value, IRabbitManager manager);
        public Task<ResultPanicStatusResponse> GetStatus(PanicPassed value);
        public Task<PanicActiveWorkOrderResponse> GetClientActiveWorkOrders(ActiveClientPanicModel value);
        public Task<DispatchHistoryResponse> GetAllStatusUpdates(string caseNumber);
        public Task<SystemNotesResponse> GetSystemNotesAsync(string caseNumber);

        public Task<Result> CreateWorkOrderNote(NotePackage package);
        public Task<Result> CreateCaseNote(NotePackage package);
        public Task<ResultPanicResponse> CallMeBack(PanicTelematics value);

    }
}
