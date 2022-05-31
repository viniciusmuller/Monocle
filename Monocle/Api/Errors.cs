using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    class InternalError
    {
        public string Message;
        public string Source;
        public string StackTrace;

        public InternalError(Exception ex)
        {
            Message = ex.Message;
            Source = ex.Source;
            StackTrace = ex.StackTrace;
        }
    }

    class UserNotFoundError
    {
        public string UserId;

        public UserNotFoundError(string userId)
        {
            UserId = userId;
        }
    }
}
