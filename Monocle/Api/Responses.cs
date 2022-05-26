using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    abstract class Response { }

    internal class BaseResponse<T> : Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageKind Kind = MessageKind.Response;

        public T Data { get; set; }

        public BaseResponse(ResponseType type, T data)
        {
            Type = type;
            Data = data;
        }
    }

    internal class WorldSizeResponse : Response
    {
        // Apparently all of unturned maps are square
        public int Size { get; set; }
    }
}
