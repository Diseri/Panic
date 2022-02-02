using System;
using System.Threading.Tasks;
using AASA.NetCore.IDomainService.Panic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication;
using AASA.NetCore.Lib.Helper.Models;
using AASA.NetCore.Api.Panic.Helper;
using AASA.NetCore.Lib.CacheManagerLib;
using AASA.NetCore.DomainService.Panic.RabbitMQ;
using AASA.NetCore.IDomainService.Panic.RabbitMQ;
using AASA.NetCore.IDomainService.Panic.Interfaces;

namespace AASA.NetCore.v1.Panic.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]")]
    public class PanicController : ControllerBase
    {
        private string ErrorLog = string.Empty;
        IPanicService context = null;
        ILogger<PanicController> _logger;
        private readonly ICacheManager _cacheManager = null;
        private IRabbitManager _manager;

        public PanicController(ILogger<PanicController> logger, IMemoryCache memoryCache, IRabbitManager manager, IPanicService panicService)
        {
            _logger = logger;
            context = panicService;

            if (_cacheManager == null)
            {
                _cacheManager = new CacheManager(new CacheManagerConfig() { MongoDatabaseName = "cache_manager", MongoDBConnectionString = MongoDBSettings.MongoDBConnectionString }, memoryCache);
            }
            _manager = manager;
        }

        /// <summary>
        /// Log a case with the AA
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpPost]
        public async Task<ActionResult<ResultPanicResponse>> Create([FromBody] PanicRequestModel value)
        {
            context.ImpersonateUserId = await new CacheHelper(_cacheManager).GetImpersonationUserId(await HttpContext.GetTokenAsync("access_token"));

            ActionResult action = Ok();
            ResultPanicResponse retval = null;

            try
            {
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error connecting to Redis. Retrying in 5 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.Create(value, _manager);
                        });
            }
            catch (Exception ex)
            {
                retval = new ResultPanicResponse() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Panic Create");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }

        /// <summary>
        /// Cancel a case with the AA
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpDelete]
        public async Task<ActionResult<ResultPanicResponse>> Delete([FromBody] PanicCancellation value)
        {
            _logger.LogInformation("Panic Cancel was executed at {date}", DateTime.UtcNow);

            ActionResult action = Ok();
            ResultPanicResponse retval = null;

            try
            {
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error connecting to Redis. Retrying in 5 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.Cancel(value, _manager);
                        });
            }
            catch (Exception ex)
            {
                retval = new ResultPanicResponse() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Panic Cancel");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }

        /// <summary>
        /// Get the Status of the Case
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="CaseNumber"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpGet]
        [Route("GetStatus/{CaseNumber}")]
        public async Task<ActionResult<ResultPanicStatusResponse>> GetStatus(string CaseNumber)
        {
            ActionResult action = Ok();
            ResultPanicStatusResponse retval = null;

            try
            {
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error. Retrying in 3 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.GetStatus(new PanicPassed() { CaseNumber = CaseNumber });
                        });
            }
            catch (Exception ex)
            {
                retval = new ResultPanicStatusResponse() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Panic GetStatus");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }

        /// <summary>
        /// Get Active Work Orders for a client
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="MobileNo"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpGet]
        [Route("GetClientActiveWorkOrders/{MobileNo}")]
        public async Task<ActionResult<PanicActiveWorkOrderResponse>> GetClientActiveWorkOrders(string MobileNo)
        {
            ActionResult action = Ok();
            PanicActiveWorkOrderResponse retval = null;

            try
            {
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error. Retrying in 3 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.GetClientActiveWorkOrders(new ActiveClientPanicModel() { MobileNo = MobileNo });
                        });
            }
            catch (Exception ex)
            {
                retval = new PanicActiveWorkOrderResponse() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Panic GetStatus");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }



        /// <summary>
        /// Get All Status Updates for a particular case.
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="CaseNumber"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpGet]
        [Route("[action]/{CaseNumber}")]
        public async Task<ActionResult<SystemNotesResponse>> GetAllStatusUpdates(string CaseNumber)
        {
            ActionResult action = Ok();
            SystemNotesResponse retval = null;

            try
            {
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error. Retrying in 3 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.GetSystemNotesAsync(CaseNumber);
                        });
            }
            catch (Exception ex)
            {
                retval = new SystemNotesResponse() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Panic GetAllStatusUpdates");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }

        /// <summary>
        /// Create a note against your work order with the AA
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="package"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Result>> CreateWorkOrderNote([FromBody] NotePackage package)
        {
            _logger.LogInformation("Create Work Order Note was executed at {date}", DateTime.UtcNow);

            ActionResult action = Ok();
            Result retval = null;

            try
            {
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error. Retrying in 5 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.CreateWorkOrderNote(package);
                        });
            }
            catch (Exception ex)
            {
                retval = new Result() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Create WorkOrder Note");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }

        /// <summary>
        /// Create a note against your case with the AA
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="package"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Result>> CreateCaseNote([FromBody] NotePackage package)
        {
            _logger.LogInformation("Create Case Note was executed at {date}", DateTime.UtcNow);

            ActionResult action = Ok();
            Result retval = null;

            try
            {
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error. Retrying in 5 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.CreateCaseNote(package);
                        });
            }
            catch (Exception ex)
            {
                retval = new Result() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Create Case Note");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }

        /// <summary>
        /// Log a Call Me Back with the AA
        /// </summary>
        /// <remarks>
        /// Authentication Type : Bearer (Access Token)
        /// <br /> 
        /// You are required to call the Authentication method and pass a Bearer Token received in the Header.
        /// <br /> 
        /// The token provided lasts for a certain time period before it expires.
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Successfull response object</returns>
        /// <response code="200">
        /// </response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ResultPanicResponse>> CallMeBack([FromBody] PanicTelematics value)
        {
            context.ImpersonateUserId = await new CacheHelper(_cacheManager).GetImpersonationUserId(await HttpContext.GetTokenAsync("access_token"));

            ActionResult action = Ok();
            ResultPanicResponse retval = null;

            try
            {
                // throw new Exception("test exception");
                await Policy
                        .Handle<Exception>()
                        .WaitAndRetry(3, r => TimeSpan.FromSeconds(3), (ex, ts) => { ErrorLog = ("Error. Retrying in 5 sec."); })
                        .Execute(async () =>
                        {
                            retval = await context.CallMeBack(value);
                        });
            }
            catch (Exception ex)
            {
                retval = new ResultPanicResponse() { Success = false, Error = ex.Message };
                _logger.LogError(ex, "Panic Call Me Back");
                return BadRequest(retval);
            }

            action = Ok(retval);

            return action;
        }
    }
}