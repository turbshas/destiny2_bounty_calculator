using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.Exceptions
{
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException() : base()
        {
        }

        public InvalidResponseException(string message) : base(message)
        {
        }

        public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
