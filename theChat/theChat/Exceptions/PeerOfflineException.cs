using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theChat
{
    class PeerOfflineException : Exception 
    {



        public PeerOfflineException()
        {
        }

        public PeerOfflineException(string message)
            : base(message)
        {
        }

        public PeerOfflineException(string message, Exception inner)
            : base(message, inner)
        {
        }


    }
}
