using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.Exceptions
{
    public class InvalidQueryException : Exception
    {
        public InvalidQueryException() : base()
        {
        }

        public InvalidQueryException(string message) : base(message)
        {
        }

        public InvalidQueryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
