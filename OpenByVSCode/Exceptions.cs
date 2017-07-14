using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenByVSCode
{
    /// <summary>
    /// An exception that should be thrown when the error condition is caused.
    /// </summary>
    class AppException : Exception
    {
        public AppException(string message) : base(message) { }
        public AppException(string message, Exception inner) : base(message, inner) { }
    }
}
