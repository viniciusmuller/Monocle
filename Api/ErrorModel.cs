using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    internal class ErrorModel : Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType Type { get; set; }
        public string Message { get; set; }

        public ErrorModel(ErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
