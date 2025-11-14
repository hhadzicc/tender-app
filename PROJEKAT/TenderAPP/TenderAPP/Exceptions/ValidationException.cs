using System;
using System.Diagnostics.CodeAnalysis;

namespace TenderiApp.Exceptions
{
    //ove klase su trivijalne. Coverage od 50% je dovoljan jer testovi već bacaju ove exception-e indirektno. ISKLJUCENO
    [ExcludeFromCodeCoverage]
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}