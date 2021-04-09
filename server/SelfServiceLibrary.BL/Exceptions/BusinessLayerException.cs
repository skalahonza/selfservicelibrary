using System;
using System.Runtime.Serialization;

namespace SelfServiceLibrary.BL.Exceptions
{
    public class BusinessLayerException : Exception
    {
        public BusinessLayerException()
        {
        }

        public BusinessLayerException(string? message) : base(message)
        {
        }

        public BusinessLayerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BusinessLayerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
