using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplainerProd.Test
{
    public class AuthenticationToken
    {
        public string Guid { get; set; }
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }

        [JsonProperty(".expires")]
        public DateTime Expires { get; set; }
    }
}
