using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    internal class SuccesfulAuthResponse
    {
        public AuthorizedUserType UserType;

        public SuccesfulAuthResponse(AuthorizedUserType type)
        {
            UserType = type;
        }
    }

    class PlayerScreenshotResponse
    {
        public string? ScreenEncoded { get; set; }
        public string? PlayerId { get; set; }
    }
}
