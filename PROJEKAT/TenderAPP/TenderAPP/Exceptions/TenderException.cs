using System;

namespace TenderiApp.Exceptions
{
    public class TenderException : Exception
    {
        public TenderException(string message) : base(message)
        {
        }

        public TenderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}