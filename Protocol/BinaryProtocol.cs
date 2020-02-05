using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Axon
{
    public interface IBinaryProtocol : IProtocol
    {
        bool CompressionEnabled { get; }
    }

    public class BinaryProtocol : AProtocol, IBinaryProtocol
    {
        private readonly bool compressionEnabled;
        public bool CompressionEnabled
        {
            get
            {
                return this.compressionEnabled;
            }
        }

        public BinaryProtocol(bool compressionEnabled) : base()
        {
            this.compressionEnabled = compressionEnabled;
        }

        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = new MemoryStream())
            {
                if (this.CompressionEnabled)
                {
                    using (var compressionStream = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new BinaryProtocolWriter(compressionStream);

                        handler(writer);
                    }
                }
                else
                {
                    var writer = new BinaryProtocolWriter(buffer);

                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                await transport.Send(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }

        public override async Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive();

            var buffer = new MemoryStream(receivedData.Payload);
            BinaryProtocolReader reader;
            if (this.CompressionEnabled)
            {
                var compressionStream = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress);
                reader = new BinaryProtocolReader(compressionStream);
            }
            else
            {
                reader = new BinaryProtocolReader(buffer);
            }

            handler(reader, receivedData.Metadata);
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive();

            var buffer = new MemoryStream(receivedData.Payload);
            BinaryProtocolReader reader;
            if (this.CompressionEnabled)
            {
                var compressionStream = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress);
                reader = new BinaryProtocolReader(compressionStream);
            }
            else
            {
                reader = new BinaryProtocolReader(buffer);
            }

            var result = handler(reader, receivedData.Metadata);

            return result;
        }

        public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                if (this.CompressionEnabled)
                {
                    using (var compressionStream = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new BinaryProtocolWriter(compressionStream);

                        handler(writer);
                    }
                }
                else
                {
                    var writer = new BinaryProtocolWriter(buffer);

                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                var buffer = new MemoryStream(receivedData.Payload);
                BinaryProtocolReader reader;
                if (this.CompressionEnabled)
                {
                    var compressionStream = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress);
                    reader = new BinaryProtocolReader(compressionStream);
                }
                else
                {
                    reader = new BinaryProtocolReader(buffer);
                }

                readHandler(reader, receivedData.Metadata);
            });
        }
        public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var buffer = new MemoryStream())
            {
                if (this.CompressionEnabled)
                {
                    using (var compressionStream = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
                    {
                        var writer = new BinaryProtocolWriter(compressionStream);

                        handler(writer);
                    }
                }
                else
                {
                    var writer = new BinaryProtocolWriter(buffer);

                    handler(writer);

                    buffer.Position = 0;
                }

                var data = buffer.ToArray();
                receiveHandler = await transport.SendAndReceive(new TransportMessage(data, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                var buffer = new MemoryStream(receivedData.Payload);
                BinaryProtocolReader reader;
                if (this.CompressionEnabled)
                {
                    var compressionStream = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress);
                    reader = new BinaryProtocolReader(compressionStream);
                }
                else
                {
                    reader = new BinaryProtocolReader(buffer);
                }

                return readHandler(reader, receivedData.Metadata);
            });
        }
    }

    public interface IBinaryProtocolReader : IProtocolReader
    {
        BinaryReader DecoderStream { get; }
    }

    public class BinaryProtocolReader : AProtocolReader, IBinaryProtocolReader
    {
        public BinaryReader DecoderStream { get; private set; }

        public BinaryProtocolReader(Stream buffer)
        {
            this.DecoderStream = new BinaryReader(buffer);
        }

        public override string ReadStringValue()
        {
            return this.DecoderStream.ReadString();
        }
        public override bool ReadBooleanValue()
        {
            return this.DecoderStream.ReadBoolean();
        }
        public override byte ReadByteValue()
        {
            return this.DecoderStream.ReadByte();
        }
        public override short ReadShortValue()
        {
            return this.DecoderStream.ReadInt16();
        }
        public override int ReadIntegerValue()
        {
            return this.DecoderStream.ReadInt32();
        }
        public override long ReadLongValue()
        {
            return this.DecoderStream.ReadInt64();
        }
        public override float ReadFloatValue()
        {
            return this.DecoderStream.ReadSingle();
        }
        public override double ReadDoubleValue()
        {
            return this.DecoderStream.ReadDouble();
        }
        public override T ReadEnumValue<T>()
        {
            var encodedValue = this.DecoderStream.ReadInt32();

            return (T)Enum.ToObject(typeof(T), encodedValue);
        }
        public override object ReadIndeterminateValue()
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }
    }

    public interface IBinaryProtocolWriter : IProtocolWriter
    {
        BinaryWriter EncoderStream { get; }
    }

    public class BinaryProtocolWriter : AProtocolWriter, IBinaryProtocolWriter
    {
        public BinaryWriter EncoderStream { get; private set; }

        public BinaryProtocolWriter(Stream buffer)
        {
            this.EncoderStream = new BinaryWriter(buffer);
        }

        public override void WriteStringValue(string value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteBooleanValue(bool value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteByteValue(byte value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteShortValue(short value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteIntegerValue(int value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteLongValue(long value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteFloatValue(float value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteDoubleValue(double value)
        {
            this.EncoderStream.Write(value);
        }
        public override void WriteEnumValue<T>(T value)
        {
            var decodeValue = value.ToInt32(System.Globalization.CultureInfo.InvariantCulture);

            this.EncoderStream.Write(decodeValue);
        }
        public override void WriteIndeterminateValue(object value)
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }
    }
}