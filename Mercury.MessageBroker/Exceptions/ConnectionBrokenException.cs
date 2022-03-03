using System.Runtime.Serialization;

namespace Mercury.MessageBroker.Exceptions
{
    [Serializable]
    internal class ConnectionBrokenException : Exception
    {


        public ConnectionBrokenException()
        {
        }

        public ConnectionBrokenException(string? message) : base(message)
        {
        }

        public ConnectionBrokenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ConnectionBrokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}