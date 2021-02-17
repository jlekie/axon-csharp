using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Axon
{
    public class EntanglementProtocol : AProtocol
    {
        public readonly bool Compress;

        public EntanglementProtocol() : base()
        {
            this.Compress = false;
        }
        public EntanglementProtocol(bool compress) : base()
        {
            this.Compress = compress;
        }

        public T Read<T>(byte[] data, Func<IProtocolReader, T> handler)
        {
            using (var buffer = new MemoryStream(data))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        return handler(reader);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    return handler(reader);
                }
            }
        }
        public byte[] Write(Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();

                return data;
            }
        }

        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                await transport.Send(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }
        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                await transport.Send(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                await transport.Send(messageId, new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                await transport.Send(messageId, new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }
        }

        public override async Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive();

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    handler(reader, receivedData.Metadata);
                }
            }
        }
        public override async Task ReadData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(cancellationToken);

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    handler(reader, receivedData.Metadata);
                }
            }
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive();

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        return handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    return handler(reader, receivedData.Metadata);
                }
            }
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(cancellationToken);

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        return handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    return handler(reader, receivedData.Metadata);
                }
            }
        }
        public override async Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(messageId);

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    handler(reader, receivedData.Metadata);
                }
            }
        }
        public override async Task ReadData(ITransport transport, string messageId, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(messageId, cancellationToken);

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    handler(reader, receivedData.Metadata);
                }
            }
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(messageId);

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        return handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    return handler(reader, receivedData.Metadata);
                }
            }
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(messageId, cancellationToken);

            using (var buffer = new MemoryStream(receivedData.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        return handler(reader, receivedData.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    return handler(reader, receivedData.Metadata);
                }
            }
        }

        public override async Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            using (var buffer = new MemoryStream(receivedData.Message.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        handler(reader, receivedData.Id, receivedData.Message.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    handler(reader, receivedData.Id, receivedData.Message.Metadata);
                }
            }
        }
        public override async Task ReadTaggedData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            var receivedData = await transport.ReceiveTagged(cancellationToken);

            using (var buffer = new MemoryStream(receivedData.Message.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        handler(reader, receivedData.Id, receivedData.Message.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    handler(reader, receivedData.Id, receivedData.Message.Metadata);
                }
            }
        }
        public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            using (var buffer = new MemoryStream(receivedData.Message.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        return handler(reader, receivedData.Id, receivedData.Message.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    return handler(reader, receivedData.Id, receivedData.Message.Metadata);
                }
            }
        }
        public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.ReceiveTagged(cancellationToken);

            using (var buffer = new MemoryStream(receivedData.Message.Payload))
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                    {
                        var reader = new EntanglementProtocolReader(gzip);

                        return handler(reader, receivedData.Id, receivedData.Message.Metadata);
                    }
                }
                else
                {
                    var reader = new EntanglementProtocolReader(buffer);

                    return handler(reader, receivedData.Id, receivedData.Message.Metadata);
                }
            }
        }

        public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = new MemoryStream(receivedData.Payload))
                {
                    if (this.Compress)
                    {
                        using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                        {
                            var reader = new EntanglementProtocolReader(gzip);

                            readHandler(reader, receivedData.Metadata);
                        }
                    }
                    else
                    {
                        var reader = new EntanglementProtocolReader(buffer);

                        readHandler(reader, receivedData.Metadata);
                    }
                }
            });
        }
        public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = new MemoryStream(receivedData.Payload))
                {
                    if (this.Compress)
                    {
                        using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                        {
                            var reader = new EntanglementProtocolReader(gzip);

                            readHandler(reader, receivedData.Metadata);
                        }
                    }
                    else
                    {
                        var reader = new EntanglementProtocolReader(buffer);

                        readHandler(reader, receivedData.Metadata);
                    }
                }
            });
        }
        public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = new MemoryStream(receivedData.Payload))
                {
                    if (this.Compress)
                    {
                        using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                        {
                            var reader = new EntanglementProtocolReader(gzip);

                            return readHandler(reader, receivedData.Metadata);
                        }
                    }
                    else
                    {
                        var reader = new EntanglementProtocolReader(buffer);

                        return readHandler(reader, receivedData.Metadata);
                    }
                }
            });
        }
        public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                if (this.Compress)
                {
                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new EntanglementProtocolWriter(gzip);
                        handler(writer);
                    }
                }
                else
                {
                    var writer = new EntanglementProtocolWriter(buffer);
                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = new MemoryStream(receivedData.Payload))
                {
                    if (this.Compress)
                    {
                        using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
                        {
                            var reader = new EntanglementProtocolReader(gzip);

                            return readHandler(reader, receivedData.Metadata);
                        }
                    }
                    else
                    {
                        var reader = new EntanglementProtocolReader(buffer);

                        return readHandler(reader, receivedData.Metadata);
                    }
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
        public override async Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
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

        public override void WriteRequestStart(RequestHeader header)
        {
            this.WriteStringValue(header.ActionName);
            this.WriteIntegerValue(header.ArgumentCount);
        }
        public override void WriteRequestEnd()
        {
        }

        public override void WriteRequestArgumentStart(RequestArgumentHeader header)
        {
            this.WriteStringValue(header.ArgumentName);
            this.WriteStringValue(header.Type);
        }
        public override void WriteRequestArgumentEnd()
        {
        }

        public override void WriteResponseStart(ResponseHeader header)
        {
            this.WriteBooleanValue(header.Success);
            this.WriteStringValue(header.Type);
            this.WriteIntegerValue(header.ArgumentCount);
        }
        public override void WriteResponseEnd()
        {
        }

        public override void WriteResponseArgumentStart(ResponseArgumentHeader header)
        {
            this.WriteStringValue(header.ArgumentName);
            this.WriteStringValue(header.Type);
        }
        public override void WriteResponseArgumentEnd()
        {
        }

        public override void WriteModelStart(ModelHeader header)
        {
            this.WriteStringValue(header.ModelName);
            this.WriteIntegerValue(header.PropertyCount);
        }
        public override void WriteModelEnd()
        {
        }

        public override void WriteModelPropertyStart(ModelPropertyHeader header)
        {
            this.WriteStringValue(header.PropertyName);
            this.WriteStringValue(header.Type);
        }
        public override void WriteModelPropertyEnd()
        {
        }

        public override void WriteArrayStart(ArrayHeader header)
        {
            this.WriteIntegerValue(header.ItemCount);
        }
        public override void WriteArrayEnd()
        {
        }

        public override void WriteArrayItemStart(ArrayItemHeader header)
        {
            this.WriteStringValue(header.Type);
        }
        public override void WriteArrayItemEnd()
        {
        }

        public override void WriteDictionaryStart(DictionaryHeader header)
        {
            this.WriteIntegerValue(header.RecordCount);
        }
        public override void WriteDictionaryEnd()
        {
        }

        public override void WriteDictionaryItemStart(DictionaryItemHeader header)
        {
            this.WriteStringValue(header.KeyType);
            this.WriteStringValue(header.ValueType);
        }
        public override void WriteDictionaryItemEnd()
        {
        }

        public override void WriteIndefiniteValueStart(IndefiniteValueHeader header)
        {
            this.WriteStringValue(header.ValueType);
        }
        public override void WriteIndefiniteValueEnd()
        {
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

        public override RequestHeader ReadRequestStart()
        {
            var actionName = this.ReadStringValue();
            var argumentCount = this.ReadIntegerValue();

            return new RequestHeader(actionName, argumentCount);
        }
        public override void ReadRequestEnd()
        {
        }

        public override RequestArgumentHeader ReadRequestArgumentStart()
        {
            var argumentName = this.ReadStringValue();
            var type = this.ReadStringValue();

            return new RequestArgumentHeader(argumentName, type);
        }
        public override void ReadRequestArgumentEnd()
        {
        }

        public override ResponseHeader ReadResponseStart()
        {
            var success = this.ReadBooleanValue();
            var type = this.ReadStringValue();
            var argumentCount = this.ReadIntegerValue();

            return new ResponseHeader(success, type, argumentCount);
        }
        public override void ReadResponseEnd()
        {
        }

        public override ResponseArgumentHeader ReadResponseArgumentStart()
        {
            var argumentName = this.ReadStringValue();
            var type = this.ReadStringValue();

            return new ResponseArgumentHeader(argumentName, type);
        }
        public override void ReadResponseArgumentEnd()
        {
        }

        public override ModelHeader ReadModelStart()
        {
            var modelName = this.ReadStringValue();
            var propertyCount = this.ReadIntegerValue();

            return new ModelHeader(modelName, propertyCount);
        }
        public override void ReadModelEnd()
        {
        }

        public override ModelPropertyHeader ReadModelPropertyStart()
        {
            var propertyName = this.ReadStringValue();
            var type = this.ReadStringValue();

            return new ModelPropertyHeader(propertyName, type);
        }
        public override void ReadModelPropertyEnd()
        {
        }

        public override ArrayHeader ReadArrayStart()
        {
            var itemCount = this.ReadIntegerValue();

            return new ArrayHeader(itemCount);
        }
        public override void ReadArrayEnd()
        {
        }

        public override ArrayItemHeader ReadArrayItemStart()
        {
            var type = this.ReadStringValue();

            return new ArrayItemHeader(type);
        }
        public override void ReadArrayItemEnd()
        {
        }

        public override DictionaryHeader ReadDictionaryStart()
        {
            var recordCount = this.ReadIntegerValue();

            return new DictionaryHeader(recordCount);
        }
        public override void ReadDictionaryEnd()
        {
        }

        public override DictionaryItemHeader ReadDictionaryItemStart()
        {
            var keyType = this.ReadStringValue();
            var valueType = this.ReadStringValue();

            return new DictionaryItemHeader(keyType, valueType);
        }
        public override void ReadDictionaryItemEnd()
        {
        }

        public override IndefiniteValueHeader ReadIndefiniteValueStart()
        {
            var valueType = this.ReadStringValue();

            return new IndefiniteValueHeader(valueType);
        }
        public override void ReadIndefiniteValueEnd()
        {
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