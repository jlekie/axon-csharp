using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Axon
{
    public class EntanglementProtocol : AProtocol
    {
        public EntanglementProtocol() : base()
        {
        }

        public T Read<T>(byte[] data, Func<IProtocolReader, T> handler)
        {
            using (var buffer = new MemoryStream(data))
            {
                var reader = new EntanglementProtocolReader(buffer);

                return handler(reader);
            }
        }
        public byte[] Write(Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                var writer = new EntanglementProtocolWriter(buffer);
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
                var writer = new EntanglementProtocolWriter(buffer);
                handler(writer);

                buffer.Position = 0;
                var data = buffer.ToArray();
                await transport.Send(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                var writer = new EntanglementProtocolWriter(buffer);
                handler(writer);

                buffer.Position = 0;
                var data = buffer.ToArray();
                await transport.Send(messageId, new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }

        public override async Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive();

            var buffer = new MemoryStream(receivedData.Payload);
            var reader = new EntanglementProtocolReader(buffer);

            handler(reader, receivedData.Metadata);
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive();

            var buffer = new MemoryStream(receivedData.Payload);
            var reader = new EntanglementProtocolReader(buffer);

            var result = handler(reader, receivedData.Metadata);

            return result;
        }
        public override async Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(messageId);

            var buffer = new MemoryStream(receivedData.Payload);
            var reader = new EntanglementProtocolReader(buffer);

            handler(reader, receivedData.Metadata);
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(messageId);

            var buffer = new MemoryStream(receivedData.Payload);
            var reader = new EntanglementProtocolReader(buffer);

            var result = handler(reader, receivedData.Metadata);

            return result;
        }

        public override async Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            var buffer = new MemoryStream(receivedData.Message.Payload);
            var reader = new EntanglementProtocolReader(buffer);

            handler(reader, receivedData.Id, receivedData.Message.Metadata);
        }
        public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            var buffer = new MemoryStream(receivedData.Message.Payload);
            var reader = new EntanglementProtocolReader(buffer);

            var result = handler(reader, receivedData.Id, receivedData.Message.Metadata);

            return result;
        }

        public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                var writer = new EntanglementProtocolWriter(buffer);
                handler(writer);

                buffer.Position = 0;
                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                var buffer = new MemoryStream(receivedData.Payload);
                var reader = new EntanglementProtocolReader(buffer);

                readHandler(reader, receivedData.Metadata);
            });
        }
        public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                var writer = new EntanglementProtocolWriter(buffer);
                handler(writer);

                buffer.Position = 0;
                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                var buffer = new MemoryStream(receivedData.Payload);
                var reader = new EntanglementProtocolReader(buffer);

                return readHandler(reader, receivedData.Metadata);
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
    }

    public class EntanglementProtocolWriter : AProtocolWriter
    {
        public BinaryWriter EncoderStream { get; private set; }

        public EntanglementProtocolWriter(Stream buffer)
        {
            this.EncoderStream = new BinaryWriter(buffer);
        }

        public override void WriteStringValue(string value)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(value);

            this.EncoderStream.Write(BitConverter.GetBytes(buffer.Length));
            this.EncoderStream.Write(buffer);
        }
        public override void WriteBooleanValue(bool value)
        {
            this.EncoderStream.Write(BitConverter.GetBytes(value));
        }
        public override void WriteByteValue(byte value)
        {
            this.EncoderStream.Write(new byte[] { value });
        }
        public override void WriteShortValue(short value)
        {
            this.EncoderStream.Write(BitConverter.GetBytes(value));
        }
        public override void WriteIntegerValue(int value)
        {
            this.EncoderStream.Write(BitConverter.GetBytes(value));
        }
        public override void WriteLongValue(long value)
        {
            this.EncoderStream.Write(BitConverter.GetBytes(value));
        }
        public override void WriteFloatValue(float value)
        {
            this.EncoderStream.Write(BitConverter.GetBytes(value));
        }
        public override void WriteDoubleValue(double value)
        {
            this.EncoderStream.Write(BitConverter.GetBytes(value));
        }
        public override void WriteEnumValue<T>(T value)
        {
            var enumValue = value.ToInt32(System.Globalization.CultureInfo.InvariantCulture);

            this.EncoderStream.Write(BitConverter.GetBytes(enumValue));
        }
        public override void WriteIndeterminateValue(object value)
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }
    }

    public class EntanglementProtocolReader : AProtocolReader
    {
        public BinaryReader DecoderStream { get; private set; }

        public EntanglementProtocolReader(Stream buffer)
        {
            this.DecoderStream = new BinaryReader(buffer);
        }

        public override string ReadStringValue()
        {
            var length = BitConverter.ToInt32(this.DecoderStream.ReadBytes(4), 0);

            if (length > 0)
            {
                var contentBuffer = this.DecoderStream.ReadBytes(length);

                return System.Text.Encoding.UTF8.GetString(contentBuffer);
            }
            else
            {
                return string.Empty;
            }
        }
        public override bool ReadBooleanValue()
        {
            return BitConverter.ToBoolean(this.DecoderStream.ReadBytes(1), 0);
        }
        public override byte ReadByteValue()
        {
            return this.DecoderStream.ReadByte();
        }
        public override short ReadShortValue()
        {
            return BitConverter.ToInt16(this.DecoderStream.ReadBytes(2), 0);
        }
        public override int ReadIntegerValue()
        {
            return BitConverter.ToInt32(this.DecoderStream.ReadBytes(4), 0);
        }
        public override long ReadLongValue()
        {
            return BitConverter.ToInt64(this.DecoderStream.ReadBytes(8), 0);
        }
        public override float ReadFloatValue()
        {
            return BitConverter.ToSingle(this.DecoderStream.ReadBytes(4), 0);
        }
        public override double ReadDoubleValue()
        {
            return BitConverter.ToDouble(this.DecoderStream.ReadBytes(8), 0);
        }
        public override T ReadEnumValue<T>()
        {
            var enumValue = BitConverter.ToInt32(this.DecoderStream.ReadBytes(4), 0);

            return (T)Enum.ToObject(typeof(T), enumValue);
        }
        public override object ReadIndeterminateValue()
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }
    }
}

internal static class ByteHelpers
{
    public static string FormatBytes(this byte[] bytes)
    {
        string value = "";
        foreach (var byt in bytes)
            value += String.Format("{0:X2} ", byt);

        return value;
    }
}