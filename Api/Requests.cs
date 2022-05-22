using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{

    internal class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    internal class GetUserInfoRequest
    {
        public string? UserId { get; set; }
    }

    internal class BaseRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestType? Type { get; set; }
    }
}
