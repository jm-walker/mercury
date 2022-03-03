using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.MessageBroker.Exceptions
{
    public class InvalidQueueException : Exception
    {
        public InvalidQueueException()
        {
        }

        public InvalidQueueException(string? message) : base(message)
        {
        }

        public InvalidQueueException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidQueueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
