using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    internal enum RequestType
    {
        Unknown,
        Authenticate,
        Players,
        Structures,
        Barricades,
        Vehicles,
        ServerInfo,
        PlayerScreenshot,
        GameMap,
    }

    internal class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    internal class PlayerScreenshotRequest
    {
        public string? UserId { get; set; }
    }

    internal class BaseRequest<T>
    {
        public RequestType? Type { get; set; }

        // We can't know the type data that an user sends until we cast it
        public T? Data { get; set; }
    }
}
