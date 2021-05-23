using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theChat
{
    class UserOfflineException : Exception 
    {

        public UserOfflineException()
        {
        }

        public UserOfflineException(string message)
            : base(message)
        {
        }

        public UserOfflineException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
