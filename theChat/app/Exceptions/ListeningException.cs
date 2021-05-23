using System;
using System.Net;

namespace theChat
{
    class ListeningException : Exception
    {

        public ListeningException()
        {
        }

        public ListeningException(string message)
            : base(message)
        {
        }

        public ListeningException(string message, Exception inner)
            : base(message, inner)
        {
        }


    }
}