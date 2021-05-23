using System;
using System.Net;

namespace theChat
{
    class PeerNotListeningException : Exception 
    {
      
        public PeerNotListeningException()
        {
        }

        public PeerNotListeningException(string message)
            : base(message)
        {
        }

        public PeerNotListeningException(string message, Exception inner)
            : base(message, inner)
        {
        }


    }
}
