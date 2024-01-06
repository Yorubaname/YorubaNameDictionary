using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class RepositoryAccessException : Exception
    {
        public RepositoryAccessException()
        {
        }

        public RepositoryAccessException(string message)
            : base(message)
        {
        }

        public RepositoryAccessException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
