using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Threading;

namespace Axon
{
    public class XmlProtocol : AProtocol
    {
        public static readonly string IDENTIFIER = "xml";
        public override string Identifier => IDENTIFIER;

        public XmlProtocol() : base()
        {
        }

        public override T Read<T>(Memory<byte> data, Func<IProtocolReader, T> handler)
        {
            using (var buffer = new MemoryStream(data.ToArray()))
            {
                var reader = new XmlProtocolReader(this, buffer);

                return handler(reader);
            }
        }
        public override Memory<byte> Write(Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                var writer = new XmlProtocolWriter(this, buffer);
                handler(writer);

                buffer.Position = 0;

                var data = buffer.ToArray();

                return data;
            }
        }

        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                var writer = new XmlProtocolWriter(this, buffer);
                handler(writer);

                buffer.Position = 0;

                var data = buffer.ToArray();
                await transport.Send(new TransportMessage(data, this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                var writer = new XmlProtocolWriter(this, buffer);
                handler(writer);

                buffer.Position = 0;

                var data = buffer.ToArray();
                await transport.Send(messageId, new TransportMessage(data, this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }

        public override async Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive();

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                var reader = new XmlProtocolReader(this, buffer);

                handler(reader, receivedData.Metadata);
            }
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive();

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                var reader = new XmlProtocolReader(this, buffer);

                return handler(reader, receivedData.Metadata);
            }
        }
        public override async Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(messageId);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                var reader = new XmlProtocolReader(this, buffer);

                handler(reader, receivedData.Metadata);
            }
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(messageId);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                var reader = new XmlProtocolReader(this, buffer);

                return handler(reader, receivedData.Metadata);
            }
        }

        public override async Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            if (receivedData.Message.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.Message.ProtocolIdentifier}]");

            using (var buffer = new MemoryStream(receivedData.Message.Payload))
            {
                var reader = new XmlProtocolReader(this, buffer);

                handler(reader, receivedData.Id, receivedData.Message.Metadata);
            }
        }
        public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            if (receivedData.Message.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.Message.ProtocolIdentifier}]");

            using (var buffer = new MemoryStream(receivedData.Message.Payload))
            {
                var reader = new XmlProtocolReader(this, buffer);

                return handler(reader, receivedData.Id, receivedData.Message.Metadata);
            }
        }

        public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                var writer = new XmlProtocolWriter(this, buffer);
                handler(writer);

                buffer.Position = 0;

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) =>
            {
                var receivedData = await receiveHandler();

                if (receivedData.ProtocolIdentifier != this.Identifier)
                    throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

                using (var buffer = new MemoryStream(receivedData.Payload))
                {
                    var reader = new XmlProtocolReader(this, buffer);

                    readHandler(reader, receivedData.Metadata);
                }
            });
        }
        public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                var writer = new XmlProtocolWriter(this, buffer);
                handler(writer);

                buffer.Position = 0;

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) =>
            {
                var receivedData = await receiveHandler();

                if (receivedData.ProtocolIdentifier != this.Identifier)
                    throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

                using (var buffer = new MemoryStream(receivedData.Payload))
                {
                    var reader = new XmlProtocolReader(this, buffer);

                    return readHandler(reader, receivedData.Metadata);
                }
            });
        }

        public override async Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
            //var receiveResults = await transport.ReceiveAndSend();

            //var receiveBuffer = new MemoryStream(receiveResults.ReceivedData.Data);
            //var reader = new EntanglementProtocolReader(transport, this, receiveBuffer);
            //handler(reader, receiveResults.ReceivedData.Metadata);

            //return new Func<Action<IProtocolWriter>, Task>(async (writeHandler) =>
            //{
            //    using (var buffer = new MemoryStream())
            //    {
            //        var writer = new EntanglementProtocolWriter(transport, this, buffer);
            //        writeHandler(writer);

            //        buffer.Position = 0;
            //        var data = buffer.ToArray();
            //        await receiveResults.SendHandler(data, receiveResults.ReceivedData.Metadata);
            //    }
            //});
        }

        public override Task WriteData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            throw new NotImplementedException();
        }

        public override Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            throw new NotImplementedException();
        }

        public override Task ReadData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
        }

        public override Task<TResult> ReadData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            throw new NotImplementedException();
        }

        public override Task ReadData(ITransport transport, string messageId, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
        }

        public override Task<TResult> ReadData<TResult>(ITransport transport, string messageId, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            throw new NotImplementedException();
        }

        public override Task ReadTaggedData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
        }

        public override Task<TResult> ReadTaggedData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            throw new NotImplementedException();
        }

        public override Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            throw new NotImplementedException();
        }

        public override Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            throw new NotImplementedException();
        }

        public override Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
        }
    }

    public class XmlProtocolWriter : AProtocolWriter
    {
        public XmlWriter EncoderStream { get; private set; }

        public XmlProtocolWriter(AProtocol protocol, Stream buffer)
            : base(protocol)
        {
            this.EncoderStream = XmlWriter.Create(buffer);
        }

        public override void WriteData(Span<byte> data)
        {
            throw new NotImplementedException();
        }

        public override void WriteStringValue(string value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteBooleanValue(bool value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteByteValue(byte value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteShortValue(short value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteIntegerValue(int value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteLongValue(long value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteFloatValue(float value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteDoubleValue(double value)
        {
            this.EncoderStream.WriteValue(value);
        }
        public override void WriteEnumValue<T>(T value)
        {
            this.EncoderStream.WriteValue(value.ToString());
        }
        public override void WriteIndeterminateValue(object value)
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }

        public override void WriteRequestStart(RequestHeader header)
        {
            this.EncoderStream.WriteStartElement("Request");
            this.EncoderStream.WriteAttributeString("ActionName", header.ActionName);
            this.EncoderStream.WriteAttributeString("ArgumentCount", header.ArgumentCount.ToString());
        }
        public override void WriteRequestEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteRequestArgumentStart(RequestArgumentHeader header)
        {
            this.EncoderStream.WriteStartElement("RequestArgument");
            this.EncoderStream.WriteAttributeString("ArgumentName", header.ArgumentName);
            this.EncoderStream.WriteAttributeString("Type", header.Type);
        }
        public override void WriteRequestArgumentEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteResponseStart(ResponseHeader header)
        {
            this.EncoderStream.WriteStartElement("Response");
            this.EncoderStream.WriteAttributeString("Success", header.Success.ToString());
            this.EncoderStream.WriteAttributeString("ArgumentCount", header.ArgumentCount.ToString());
            this.EncoderStream.WriteAttributeString("Type", header.Type);
        }
        public override void WriteResponseEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteResponseArgumentStart(ResponseArgumentHeader header)
        {
            this.EncoderStream.WriteStartElement("ResponseArgument");
            this.EncoderStream.WriteAttributeString("ArgumentName", header.ArgumentName);
            this.EncoderStream.WriteAttributeString("Type", header.Type);
        }
        public override void WriteResponseArgumentEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteModelStart(ModelHeader header)
        {
            this.EncoderStream.WriteStartElement("Model");
            this.EncoderStream.WriteAttributeString("ModelName", header.ModelName);
            this.EncoderStream.WriteAttributeString("PropertyCount", header.PropertyCount.ToString());
        }
        public override void WriteModelEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteModelPropertyStart(ModelPropertyHeader header)
        {
            this.EncoderStream.WriteStartElement("ModelProperty");
            this.EncoderStream.WriteAttributeString("PropertyName", header.PropertyName);
            this.EncoderStream.WriteAttributeString("Type", header.Type);
        }
        public override void WriteModelPropertyEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteArrayStart(ArrayHeader header)
        {
            this.EncoderStream.WriteStartElement("Array");
            this.EncoderStream.WriteAttributeString("ItemCount", header.ItemCount.ToString());
        }
        public override void WriteArrayEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteArrayItemStart(ArrayItemHeader header)
        {
            this.EncoderStream.WriteStartElement("ArrayItem");
            this.EncoderStream.WriteAttributeString("Type", header.Type);
        }
        public override void WriteArrayItemEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteDictionaryStart(DictionaryHeader header)
        {
            this.EncoderStream.WriteStartElement("Dictionary");
            this.EncoderStream.WriteAttributeString("RecordCount", header.RecordCount.ToString());
        }
        public override void WriteDictionaryEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteDictionaryItemStart(DictionaryItemHeader header)
        {
            this.EncoderStream.WriteStartElement("DictionaryItem");
            this.EncoderStream.WriteAttributeString("KeyType", header.KeyType);
            this.EncoderStream.WriteAttributeString("ValueType", header.ValueType);
        }
        public override void WriteDictionaryItemEnd()
        {
            this.EncoderStream.WriteEndElement();
        }

        public override void WriteIndefiniteValueStart(IndefiniteValueHeader header)
        {
            this.EncoderStream.WriteStartElement("IndefiniteValue");
            this.EncoderStream.WriteAttributeString("ValueType", header.ValueType);
        }
        public override void WriteIndefiniteValueEnd()
        {
            this.EncoderStream.WriteEndElement();
        }
    }

    public class XmlProtocolReader : AProtocolReader
    {
        public XmlReader DecoderStream { get; private set; }

        public XmlProtocolReader(AProtocol protocol, Stream buffer)
            : base(protocol)
        {
            this.DecoderStream = XmlReader.Create(buffer);
        }

        public override Span<byte> ReadData()
        {
            throw new NotImplementedException();
        }

        public override string ReadStringValue()
        {
            return this.DecoderStream.ReadContentAsString();
        }
        public override bool ReadBooleanValue()
        {
            return this.DecoderStream.ReadContentAsBoolean();
        }
        public override byte ReadByteValue()
        {
            return (byte)this.DecoderStream.ReadContentAsInt();
        }
        public override short ReadShortValue()
        {
            return (short)this.DecoderStream.ReadContentAsInt();
        }
        public override int ReadIntegerValue()
        {
            return this.DecoderStream.ReadContentAsInt();
        }
        public override long ReadLongValue()
        {
            return this.DecoderStream.ReadContentAsLong();
        }
        public override float ReadFloatValue()
        {
            return this.DecoderStream.ReadContentAsFloat();
        }
        public override double ReadDoubleValue()
        {
            return this.DecoderStream.ReadContentAsDouble();
        }
        public override T ReadEnumValue<T>()
        {
            var enumValue = this.DecoderStream.ReadContentAsString();

            return (T)Enum.Parse(typeof(T), enumValue);
        }
        public override object ReadIndeterminateValue()
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }

        public override RequestHeader ReadRequestStart()
        {
            this.DecoderStream.ReadStartElement("Request");

            this.DecoderStream.ReadAttributeValue();
            var actionName = this.ReadStringValue();

            this.DecoderStream.ReadAttributeValue();
            var argumentCount = this.ReadIntegerValue();

            return new RequestHeader(actionName, argumentCount);
        }
        public override void ReadRequestEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override RequestArgumentHeader ReadRequestArgumentStart()
        {
            this.DecoderStream.ReadStartElement("RequestArgument");

            this.DecoderStream.ReadAttributeValue();
            var argumentName = this.ReadStringValue();

            this.DecoderStream.ReadAttributeValue();
            var type = this.ReadStringValue();

            return new RequestArgumentHeader(argumentName, type);
        }
        public override void ReadRequestArgumentEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override ResponseHeader ReadResponseStart()
        {
            this.DecoderStream.ReadStartElement("Response");

            this.DecoderStream.ReadAttributeValue();
            var success = this.ReadBooleanValue();

            this.DecoderStream.ReadAttributeValue();
            var type = this.ReadStringValue();

            this.DecoderStream.ReadAttributeValue();
            var argumentCount = this.ReadIntegerValue();

            return new ResponseHeader(success, type, argumentCount);
        }
        public override void ReadResponseEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override ResponseArgumentHeader ReadResponseArgumentStart()
        {
            this.DecoderStream.ReadStartElement("ResponseArgument");

            this.DecoderStream.ReadAttributeValue();
            var argumentName = this.ReadStringValue();

            this.DecoderStream.ReadAttributeValue();
            var type = this.ReadStringValue();

            return new ResponseArgumentHeader(argumentName, type);
        }
        public override void ReadResponseArgumentEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override ModelHeader ReadModelStart()
        {
            this.DecoderStream.ReadStartElement("Model");

            this.DecoderStream.ReadAttributeValue();
            var modelName = this.ReadStringValue();

            this.DecoderStream.ReadAttributeValue();
            var propertyCount = this.ReadIntegerValue();

            return new ModelHeader(modelName, propertyCount);
        }
        public override void ReadModelEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override ModelPropertyHeader ReadModelPropertyStart()
        {
            this.DecoderStream.ReadStartElement("ModelProperty");

            this.DecoderStream.ReadAttributeValue();
            var propertyName = this.ReadStringValue();

            this.DecoderStream.ReadAttributeValue();
            var type = this.ReadStringValue();

            return new ModelPropertyHeader(propertyName, type);
        }
        public override void ReadModelPropertyEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override ArrayHeader ReadArrayStart()
        {
            this.DecoderStream.ReadStartElement("Array");

            this.DecoderStream.ReadAttributeValue();
            var itemCount = this.ReadIntegerValue();

            return new ArrayHeader(itemCount);
        }
        public override void ReadArrayEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override ArrayItemHeader ReadArrayItemStart()
        {
            this.DecoderStream.ReadStartElement("ArrayItem");

            this.DecoderStream.ReadAttributeValue();
            var type = this.ReadStringValue();

            return new ArrayItemHeader(type);
        }
        public override void ReadArrayItemEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override DictionaryHeader ReadDictionaryStart()
        {
            this.DecoderStream.ReadStartElement("Dictionary");

            this.DecoderStream.ReadAttributeValue();
            var recordCount = this.ReadIntegerValue();

            return new DictionaryHeader(recordCount);
        }
        public override void ReadDictionaryEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override DictionaryItemHeader ReadDictionaryItemStart()
        {
            this.DecoderStream.ReadStartElement("DictionaryItem");

            this.DecoderStream.ReadAttributeValue();
            var keyType = this.ReadStringValue();

            this.DecoderStream.ReadAttributeValue();
            var valueType = this.ReadStringValue();

            return new DictionaryItemHeader(keyType, valueType);
        }
        public override void ReadDictionaryItemEnd()
        {
            this.DecoderStream.ReadEndElement();
        }

        public override IndefiniteValueHeader ReadIndefiniteValueStart()
        {
            this.DecoderStream.ReadStartElement("IndefiniteValue");

            this.DecoderStream.ReadAttributeValue();
            var valueType = this.ReadStringValue();

            return new IndefiniteValueHeader(valueType);
        }
        public override void ReadIndefiniteValueEnd()
        {
            this.DecoderStream.ReadEndElement();
        }
    }
}
