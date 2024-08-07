using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Middlewares.Middlewares
{
    //Class to create custom exceptions
    public class CustomExceptions
    {
        public class UnauthorizedAccessEx : Exception
        {
            public UnauthorizedAccessEx(string message) : base(message) { }
        }

        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }

        public class ValidationException : Exception
        {
            public ValidationException(string message) : base(message) { }
        }
        public class CosmoException : Exception
        {
            public CosmoException(string message) : base(message)
            {
                
            }
        }
    }
}
