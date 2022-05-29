using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    internal class ServerMessage<T, D>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageKind Kind;

        [JsonConverter(typeof(StringEnumConverter))]
        public T Type { get; set; }

        public D Data { get; set; }

        public ServerMessage(MessageKind kind, T type, D data)
        {
            Kind = kind;
            Type = type;
            Data = data;
        }
    }

    class PlayerScreenshotResponse
    {
        public string ScreenEncoded { get; set; }
        public string PlayerId { get; set; }
    }
}
