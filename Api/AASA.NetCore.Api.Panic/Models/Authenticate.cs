using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AASA.NetCore.Api.Panic.Models
{
    public class Authenticate
    {
        public string username { get; set; }
        public string password { get; set; }
        public string client_secret { get; set; }
        public string client_id { get; set; }
    }


    public class AuthenticationResult
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
