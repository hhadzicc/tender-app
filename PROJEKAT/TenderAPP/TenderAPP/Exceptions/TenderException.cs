using System;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Exceptions
{
    //ove klase su trivijalne. Coverage od 50% je dovoljan jer testovi već bacaju ove exception-e indirektno. ISKLJUCENO
    [ExcludeFromCodeCoverage]
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