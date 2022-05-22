using Monocle.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Exceptions
{
    internal class ApiException : Exception
    {
        public ErrorModel ErrorModel;

        public ApiException(ErrorType type, string message)
        {
            ErrorModel = new ErrorModel(type, message);
        }
    }
}
