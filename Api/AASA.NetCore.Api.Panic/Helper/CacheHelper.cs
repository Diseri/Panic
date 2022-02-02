using AASA.NetCore.DomainService.Panic.Services;
using AASA.NetCore.Lib.CacheManagerLib;
using AASA.NetCore.Lib.Helper;
using AASA.NetCore.Lib.Helper.Extensions;
using AASA.NetCore.Lib.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AASA.NetCore.Api.Panic.Helper
{
    public class CacheHelper
    {
        public CacheHelper(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        private readonly ICacheManager _cacheManager = null;

        public async Task<Guid?> GetImpersonationUserId(string token)
        {
            Guid? retval = null;
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                var oathurl = "";

                if (string.IsNullOrEmpty(oathurl))
                {
                    string memKey = $"oauthurl";
                    if (!_cacheManager.TryGetKey<string>(memKey, out oathurl) || !CachingSession.IsCached(memKey))
                    {
                        oathurl = GeneralHelpers.GetServiceUrls(ApplicationSettings.DiscoveryServiceURL, "aasa.api.oauth2crm").GetURL();
                        _cacheManager.SetKey(memKey, oathurl); CachingSession.IsCachedEntries.Add(memKey);
                    }
                }

                var lib = new AASA.NetCore.Lib.Helper.ImpersonationUserId();
                var user = lib.GetUserIdFromToken(token);
                if (!string.IsNullOrEmpty(user))
                {
                    string memKey = $"impUsr_{user.ToUpper()}";
                    if (!_cacheManager.TryGetKey<Guid?>(memKey, out retval) || !CachingSession.IsCached(memKey))
                    {
                        retval = await lib.GetCRMUserMappingByUsername(user, oathurl);
                        _cacheManager.SetKey(memKey, retval); CachingSession.IsCachedEntries.Add(memKey);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return retval;
        }
    }
}
