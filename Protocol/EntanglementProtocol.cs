using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using System.Buffers;

namespace Axon
{
    public class EntanglementProtocol : AProtocol
    {
        public readonly bool Compress;

        private Microsoft.IO.RecyclableMemoryStreamManager MemoryStreamManager { get; }

        public EntanglementProtocol(Microsoft.IO.RecyclableMemoryStreamManager memoryStreamManager) : base()
        {
            this.MemoryStreamManager = memoryStreamManager;
            this.Compress = false;
        }
        public EntanglementProtocol(Microsoft.IO.RecyclableMemoryStreamManager memoryStreamManager, bool compress) : base()
        {
            this.MemoryStreamManager = memoryStreamManager;
            this.Compress = compress;
        }

        public T Read<T>(byte[] data, Func<IProtocolReader, T> handler)
        {
            var buffer = data.AsMemory();
            var reader = new EntanglementProtocolBufferReader(buffer);

            return handler(reader);

            //using (var buffer = this.MemoryStreamManager.GetStream(data))
            //{
            //    if (this.Compress)
            //    {
            //        using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
            //        {
            //            var reader = new EntanglementProtocolReader(gzip);

            //            return handler(reader);
            //        }
            //    }
            //    else
            //    {
            //        var reader = new EntanglementProtocolReader(buffer);

            //        return handler(reader);
            //    }
            //}
        }
        public byte[] Write(Action<IProtocolWriter> handler)
        {
            //using (var stream = this.MemoryStreamManager.GetStream())
            //{
            //    var writer = new EntanglementProtocolBufferWriter(stream);
            //    handler(writer);

            //    return writer.ToArray();
            //}

            using (var buffer = this.MemoryStreamManager.GetStream())
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
                    writer.Stats.WriteToConsole();

                    buffer.Position = 0;
                }

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                return data.ToArray();
            }
        }

        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                await transport.Send(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }
        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                await transport.Send(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                await transport.Send(messageId, new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
            }
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                await transport.Send(messageId, new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }
        }

        public override async Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive();

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
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

            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
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
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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
            using (var buffer = this.MemoryStreamManager.GetStream())
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

                buffer.Flush();
                if (!buffer.TryGetBuffer(out var data))
                    throw new Exception("Could not get data buffer");

                receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
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

            //var receiveBuffer = this.MemoryStreamManager.GetStream(receiveResults.ReceivedData.Data);
            //var reader = new EntanglementProtocolReader(transport, this, receiveBuffer);
            //handler(reader, receiveResults.ReceivedData.Metadata);

            //return new Func<Action<IProtocolWriter>, Task>(async (writeHandler) =>
            //{
            //    using (var buffer = this.MemoryStreamManager.GetStream())
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

    public class WriterStats
    {
        public int StringWrites { get; set; }
        public int BooleanWrites { get; set; }
        public int ByteWrites { get; set; }
        public int ShortWrites { get; set; }
        public int IntegerWrites { get; set; }
        public int LongWrites { get; set; }
        public int FloatWrites { get; set; }
        public int DoubleWrites { get; set; }
        public int EnumWrites { get; set; }

        public void WriteToConsole()
        {
            Console.WriteLine("-- Writer Stats --");
            Console.WriteLine($"String: {this.StringWrites}");
            Console.WriteLine($"Boolean: {this.BooleanWrites}");
            Console.WriteLine($"Byte: {this.ByteWrites}");
            Console.WriteLine($"Short: {this.ShortWrites}");
            Console.WriteLine($"Integer: {this.IntegerWrites}");
            Console.WriteLine($"Long: {this.LongWrites}");
            Console.WriteLine($"Float: {this.FloatWrites}");
            Console.WriteLine($"Double: {this.DoubleWrites}");
            Console.WriteLine($"Enum: {this.EnumWrites}");
        }
    }

    public class EntanglementProtocolWriter : AProtocolWriter
    {
        public Stream EncoderStream { get; private set; }

        private byte[] PrimitivesBuffer;

        public WriterStats Stats { get; } = new WriterStats();

        public EntanglementProtocolWriter(Stream encoderStream)
        {
            this.EncoderStream = encoderStream;

#if NETSTANDARD 
            this.PrimitivesBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(2048);
#else
            this.PrimitivesBuffer = new byte[2048];
#endif
        }

        public override void WriteStringValue(string value)
        {
            this.Stats.StringWrites++;

            var length = Encoding.UTF8.GetBytes(value, 0, value.Length, this.PrimitivesBuffer, 4);
            //for (var i = 0; i < value.Length; i++)
            //    this.PrimitivesBuffer[i] = (byte)(value[i] & 0x7f);
            //var buffer = System.Text.Encoding.UTF8.GetBytes(value);

            this.PrimitivesBuffer[0] = (byte)length;
            this.PrimitivesBuffer[1] = (byte)(length >> 8);
            this.PrimitivesBuffer[2] = (byte)(length >> 16);
            this.PrimitivesBuffer[3] = (byte)(length >> 24);

            //this.EncoderStream.Write(BitConverter.GetBytes(length), 0, 4);
            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
            this.EncoderStream.Write(this.PrimitivesBuffer, 4, length);
        }
        public override void WriteBooleanValue(bool value)
        {
            this.Stats.BooleanWrites++;

            this.PrimitivesBuffer[0] = value ? (byte)1 : (byte)0;

            this.EncoderStream.Write(BitConverter.GetBytes(value), 0, 1);
        }
        public override void WriteByteValue(byte value)
        {
            this.Stats.ByteWrites++;

            this.PrimitivesBuffer[0] = value;

            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 1);
        }
        public override void WriteShortValue(short value)
        {
            this.Stats.ShortWrites++;

            this.PrimitivesBuffer[0] = (byte)value;
            this.PrimitivesBuffer[1] = (byte)(value >> 8);

            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 2);
        }
        public override void WriteIntegerValue(int value)
        {
            this.Stats.IntegerWrites++;

            this.PrimitivesBuffer[0] = (byte)value;
            this.PrimitivesBuffer[1] = (byte)(value >> 8);
            this.PrimitivesBuffer[2] = (byte)(value >> 16);
            this.PrimitivesBuffer[3] = (byte)(value >> 24);

            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
        }
        public override void WriteLongValue(long value)
        {
            this.Stats.LongWrites++;

            this.PrimitivesBuffer[0] = (byte)value;
            this.PrimitivesBuffer[1] = (byte)(value >> 8);
            this.PrimitivesBuffer[2] = (byte)(value >> 16);
            this.PrimitivesBuffer[3] = (byte)(value >> 24);
            this.PrimitivesBuffer[4] = (byte)(value >> 32);
            this.PrimitivesBuffer[5] = (byte)(value >> 40);
            this.PrimitivesBuffer[6] = (byte)(value >> 48);
            this.PrimitivesBuffer[7] = (byte)(value >> 56);

            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 8);
        }
        public override unsafe void WriteFloatValue(float value)
        {
            this.Stats.FloatWrites++;

            uint tmpValue = *(uint*)&value;
            this.PrimitivesBuffer[0] = (byte)tmpValue;
            this.PrimitivesBuffer[1] = (byte)(tmpValue >> 8);
            this.PrimitivesBuffer[2] = (byte)(tmpValue >> 16);
            this.PrimitivesBuffer[3] = (byte)(tmpValue >> 24);

            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
        }
        public override unsafe void WriteDoubleValue(double value)
        {
            this.Stats.DoubleWrites++;

            ulong tmpValue = *(ulong*)&value;
            this.PrimitivesBuffer[0] = (byte)tmpValue;
            this.PrimitivesBuffer[1] = (byte)(tmpValue >> 8);
            this.PrimitivesBuffer[2] = (byte)(tmpValue >> 16);
            this.PrimitivesBuffer[3] = (byte)(tmpValue >> 24);
            this.PrimitivesBuffer[4] = (byte)(tmpValue >> 32);
            this.PrimitivesBuffer[5] = (byte)(tmpValue >> 40);
            this.PrimitivesBuffer[6] = (byte)(tmpValue >> 48);
            this.PrimitivesBuffer[7] = (byte)(tmpValue >> 56);

            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 8);
        }
        public override void WriteEnumValue<T>(T value)
        {
            this.Stats.EnumWrites++;

            var enumValue = value.ToInt32(System.Globalization.CultureInfo.InvariantCulture);

            this.PrimitivesBuffer[0] = (byte)enumValue;
            this.PrimitivesBuffer[1] = (byte)(enumValue >> 8);
            this.PrimitivesBuffer[2] = (byte)(enumValue >> 16);
            this.PrimitivesBuffer[3] = (byte)(enumValue >> 24);

            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
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
        public Stream DecoderStream { get; private set; }

        private byte[] PrimitivesBuffer;

        public EntanglementProtocolReader(Stream decoderStream)
        {
            this.DecoderStream = decoderStream;

#if NETSTANDARD
            this.PrimitivesBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(2048);
#else
            this.PrimitivesBuffer = new byte[2048];
#endif
        }

        public override string ReadStringValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);
            var length = BitConverter.ToInt32(this.PrimitivesBuffer, 0);

            if (length > 0)
            {
                //var contentBuffer = new byte[length];
                //Console.WriteLine(length);
                this.DecoderStream.Read(this.PrimitivesBuffer, 0, length);

                return System.Text.Encoding.UTF8.GetString(this.PrimitivesBuffer, 0, length);
            }
            else
            {
                return string.Empty;
            }
        }
        public override bool ReadBooleanValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 1);

            return BitConverter.ToBoolean(this.PrimitivesBuffer, 0);
        }
        public override byte ReadByteValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 1);

            return this.PrimitivesBuffer[0];
        }
        public override short ReadShortValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 2);

            return BitConverter.ToInt16(this.PrimitivesBuffer, 0);
        }
        public override int ReadIntegerValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);

            return BitConverter.ToInt32(this.PrimitivesBuffer, 0);
        }
        public override long ReadLongValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 8);

            return BitConverter.ToInt64(this.PrimitivesBuffer, 0);
        }
        public override float ReadFloatValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);

            return BitConverter.ToSingle(this.PrimitivesBuffer, 0);
        }
        public override double ReadDoubleValue()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 8);

            return BitConverter.ToDouble(this.PrimitivesBuffer, 0);
        }
        public override T ReadEnumValue<T>()
        {
            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);

            var enumValue = BitConverter.ToInt32(this.PrimitivesBuffer, 0);

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

#if NETSTANDARD
    public class EntanglementProtocolBufferWriter : AProtocolWriter
    {
        public MemoryStream MemoryStream { get; }
        private int Position { get; set; }

        public EntanglementProtocolBufferWriter(MemoryStream memoryStream)
        {
            this.MemoryStream = memoryStream;
            this.Position = 0;
        }

        public byte[] ToArray()
        {
            if (!this.MemoryStream.TryGetBuffer(out var bufferSegment))
                throw new Exception("Could not resolve buffer");

            return bufferSegment.Array;
        }

        public override void WriteStringValue(string value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            var length = Encoding.UTF8.GetBytes(value, buffer[(this.Position + 4)..]);
            if (!BitConverter.TryWriteBytes(buffer[this.Position..], length))
                throw new Exception("Invalid buffer write");
            this.Position += length + 4;
        }
        public override void WriteBooleanValue(bool value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            if (!BitConverter.TryWriteBytes(buffer[this.Position..], value))
                throw new Exception("Invalid buffer write");
            this.Position += 1;
        }
        public override void WriteByteValue(byte value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            buffer[this.Position] = value;
            this.Position += 1;
        }
        public override void WriteShortValue(short value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            if (!BitConverter.TryWriteBytes(buffer[this.Position..], value))
                throw new Exception("Invalid buffer write");
            this.Position += 2;
        }
        public override void WriteIntegerValue(int value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            if (!BitConverter.TryWriteBytes(buffer[this.Position..], value))
                throw new Exception("Invalid buffer write");
            this.Position += 4;
        }
        public override void WriteLongValue(long value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            if (!BitConverter.TryWriteBytes(buffer[this.Position..], value))
                throw new Exception("Invalid buffer write");
            this.Position += 8;
        }
        public override unsafe void WriteFloatValue(float value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            if (!BitConverter.TryWriteBytes(buffer[this.Position..], value))
                throw new Exception("Invalid buffer write");
            this.Position += 4;
        }
        public override unsafe void WriteDoubleValue(double value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            if (!BitConverter.TryWriteBytes(buffer[this.Position..], value))
                throw new Exception("Invalid buffer write");
            this.Position += 8;
        }
        public override void WriteEnumValue<T>(T value)
        {
            var buffer = this.MemoryStream.GetBuffer().AsSpan();

            var enumValue = value.ToInt32(System.Globalization.CultureInfo.InvariantCulture);

            if (!BitConverter.TryWriteBytes(buffer[this.Position..], enumValue))
                throw new Exception("Invalid buffer write");
            this.Position += 4;
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

    public class EntanglementProtocolBufferReader : AProtocolReader
    {
        public Memory<byte> Buffer { get; }
        private int Position { get; set; }

        public EntanglementProtocolBufferReader(Memory<byte> buffer)
        {
            this.Buffer = buffer;
            this.Position = 0;
        }

        public override string ReadStringValue()
        {
            var length = BitConverter.ToInt32(this.Buffer.Span[this.Position..]);
            this.Position += 4;

            string content;
            if (length > 0)
                content = Encoding.UTF8.GetString(this.Buffer.Span.Slice(this.Position, length));
            else
                content = string.Empty;
            this.Position += length;

            return content;
        }
        public override bool ReadBooleanValue()
        {
            var value = BitConverter.ToBoolean(this.Buffer.Span[this.Position..]);
            this.Position += 1;

            return value;
        }
        public override byte ReadByteValue()
        {
            var value = this.Buffer.Span[this.Position];
            this.Position += 1;

            return value;
        }
        public override short ReadShortValue()
        {
            var value = BitConverter.ToInt16(this.Buffer.Span[this.Position..]);
            this.Position += 2;

            return value;
        }
        public override int ReadIntegerValue()
        {
            var value = BitConverter.ToInt32(this.Buffer.Span[this.Position..]);
            this.Position += 4;

            return value;
        }
        public override long ReadLongValue()
        {
            var value = BitConverter.ToInt64(this.Buffer.Span[this.Position..]);
            this.Position += 8;

            return value;
        }
        public override float ReadFloatValue()
        {
            var value = BitConverter.ToSingle(this.Buffer.Span[this.Position..]);
            this.Position += 4;

            return value;
        }
        public override double ReadDoubleValue()
        {
            var value = BitConverter.ToDouble(this.Buffer.Span[this.Position..]);
            this.Position += 8;

            return value;
        }
        public override T ReadEnumValue<T>()
        {
            var enumValue = BitConverter.ToInt32(this.Buffer.Span[this.Position..]);
            this.Position += 4;

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
#endif
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