using System;
using System.Runtime.InteropServices;

namespace Axon
{
    [ComVisible(false)]
    public interface IRequestError
    {
        string Message { get; }

        Exception ToException();
    }

    [ComVisible(false)]
    public class RequestError : IRequestError
    {
        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                this.message = value;
            }
        }

        public RequestError()
        {
        }
        public RequestError(string message)
        {
            this.message = message;
        }

        public void Write(Axon.IProtocolWriter protocol)
        {
            protocol.WriteModelPropertyHeader(new Axon.ModelPropertyHeader("message", "string"));
            protocol.WriteStringValue(this.Message);
        }
        public void Read(Axon.IProtocolReader protocol, Axon.ModelHeader modelHeader)
        {
            for (var a = 0; a < modelHeader.PropertyCount; a++)
            {
                var propertyHeader = protocol.ReadModelPropertyHeader();

                switch (propertyHeader.PropertyName)
                {
                    case "message":
                        if (propertyHeader.Type != "string")
                            throw new Exception("Unexpected data type for property " + propertyHeader.PropertyName + " (" + propertyHeader.Type + ")");

                        this.Message = protocol.ReadStringValue();
                        break;
                    default:
                        throw new Exception("Property " + propertyHeader.PropertyName + " not recognized");
                }
            }
        }

        public Exception ToException()
        {
            return new Exception(this.Message);
        }
    }
}