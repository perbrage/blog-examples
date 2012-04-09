using System;

namespace Brage.Infrastructure.Broker
{
    public class ConnectionStatusEventArgs : EventArgs
    {
        public ConnectionStatusEventArgs()
        {
            Success = true;
        }

        public ConnectionStatusEventArgs(String errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        public Boolean Success { get; private set; }
        public String ErrorMessage { get; private set; }
    }
}
