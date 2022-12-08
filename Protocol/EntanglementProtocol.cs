using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using System.Buffers;
using System.Runtime.InteropServices;

using K4os.Compression.LZ4;
using System.Security.Cryptography;
using Blake3;
using K4os.Compression.LZ4.Engine;

namespace Axon
{
    public struct EntanglementProtocolOptions
    {
        public bool Compress;

        public EntanglementProtocolOptions(bool compress)
        {
            this.Compress = compress;
        }
    }
    //public class EntanglementProtocol : AProtocol
    //{
    //    public readonly bool Compress;

    //    private Microsoft.IO.RecyclableMemoryStreamManager MemoryStreamManager { get; }

    //    public EntanglementProtocol(Microsoft.IO.RecyclableMemoryStreamManager memoryStreamManager) : base()
    //    {
    //        this.MemoryStreamManager = memoryStreamManager;
    //        this.Compress = false;
    //    }
    //    public EntanglementProtocol(Microsoft.IO.RecyclableMemoryStreamManager memoryStreamManager, bool compress) : base()
    //    {
    //        this.MemoryStreamManager = memoryStreamManager;
    //        this.Compress = compress;
    //    }

    //    public T Read<T>(byte[] data, Func<IProtocolReader, T> handler)
    //    {
    //        var buffer = data.AsMemory();
    //        var reader = new EntanglementProtocolBufferReader(buffer);

    //        return handler(reader);

    //        //using (var buffer = this.MemoryStreamManager.GetStream(data))
    //        //{
    //        //    if (this.Compress)
    //        //    {
    //        //        using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //        //        {
    //        //            var reader = new EntanglementProtocolReader(gzip);

    //        //            return handler(reader);
    //        //        }
    //        //    }
    //        //    else
    //        //    {
    //        //        var reader = new EntanglementProtocolReader(buffer);

    //        //        return handler(reader);
    //        //    }
    //        //}
    //    }
    //    public byte[] Write(Action<IProtocolWriter> handler)
    //    {
    //        //using (var stream = this.MemoryStreamManager.GetStream())
    //        //{
    //        //    var writer = new EntanglementProtocolBufferWriter(stream);
    //        //    handler(writer);

    //        //    return writer.ToArray();
    //        //}

    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);

    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);
    //                writer.Stats.WriteToConsole();

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            return data.ToArray();
    //        }
    //    }

    //    public T ReadSpan<T>(Memory<byte> data, Func<IProtocolReader, T> handler)
    //    {
    //        var reader = new EntanglementProtocolBufferReader(data);

    //        return handler(reader);
    //    }
    //    public Memory<byte> WriteSpan(Action<IProtocolWriter> handler)
    //    {
    //        var writer = new EntanglementProtocolBufferWriter();
    //        handler(writer);

    //        writer.Stats.WriteToConsole();

    //        return writer.Span.ToArray();
    //    }

    //    public override async Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
    //    {
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            await transport.Send(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
    //        }
    //    }
    //    public override async Task WriteData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
    //    {
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            await transport.Send(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
    //        }
    //    }
    //    public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler)
    //    {
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            await transport.Send(messageId, new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
    //        }
    //    }
    //    public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
    //    {
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            await transport.Send(messageId, new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
    //        }
    //    }

    //    public override async Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
    //    {
    //        var receivedData = await transport.Receive();

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task ReadData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
    //    {
    //        var receivedData = await transport.Receive(cancellationToken);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler)
    //    {
    //        var receivedData = await transport.Receive();

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    return handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                return handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task<TResult> ReadData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
    //    {
    //        var receivedData = await transport.Receive(cancellationToken);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    return handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                return handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler)
    //    {
    //        var receivedData = await transport.Receive(messageId);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task ReadData(ITransport transport, string messageId, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
    //    {
    //        var receivedData = await transport.Receive(messageId, cancellationToken);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler)
    //    {
    //        var receivedData = await transport.Receive(messageId);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    return handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                return handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
    //    {
    //        var receivedData = await transport.Receive(messageId, cancellationToken);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    return handler(reader, receivedData.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                return handler(reader, receivedData.Metadata);
    //            }
    //        }
    //    }

    //    public override async Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler)
    //    {
    //        var receivedData = await transport.ReceiveTagged();

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task ReadTaggedData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, string, ITransportMetadata> handler)
    //    {
    //        var receivedData = await transport.ReceiveTagged(cancellationToken);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
    //    {
    //        var receivedData = await transport.ReceiveTagged();

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    return handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                return handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //            }
    //        }
    //    }
    //    public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
    //    {
    //        var receivedData = await transport.ReceiveTagged(cancellationToken);

    //        using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Message.Payload))
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                {
    //                    var reader = new EntanglementProtocolReader(gzip);

    //                    return handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //                }
    //            }
    //            else
    //            {
    //                var reader = new EntanglementProtocolReader(buffer);

    //                return handler(reader, receivedData.Id, receivedData.Message.Metadata);
    //            }
    //        }
    //    }

    //    public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
    //    {
    //        Func<Task<TransportMessage>> receiveHandler;
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
    //        }

    //        return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
    //            var receivedData = await receiveHandler();

    //            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //            {
    //                if (this.Compress)
    //                {
    //                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                    {
    //                        var reader = new EntanglementProtocolReader(gzip);

    //                        readHandler(reader, receivedData.Metadata);
    //                    }
    //                }
    //                else
    //                {
    //                    var reader = new EntanglementProtocolReader(buffer);

    //                    readHandler(reader, receivedData.Metadata);
    //                }
    //            }
    //        });
    //    }
    //    public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
    //    {
    //        Func<Task<TransportMessage>> receiveHandler;
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
    //        }

    //        return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
    //            var receivedData = await receiveHandler();

    //            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //            {
    //                if (this.Compress)
    //                {
    //                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                    {
    //                        var reader = new EntanglementProtocolReader(gzip);

    //                        readHandler(reader, receivedData.Metadata);
    //                    }
    //                }
    //                else
    //                {
    //                    var reader = new EntanglementProtocolReader(buffer);

    //                    readHandler(reader, receivedData.Metadata);
    //                }
    //            }
    //        });
    //    }
    //    public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
    //    {
    //        Func<Task<TransportMessage>> receiveHandler;
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)));
    //        }

    //        return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
    //            var receivedData = await receiveHandler();

    //            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //            {
    //                if (this.Compress)
    //                {
    //                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                    {
    //                        var reader = new EntanglementProtocolReader(gzip);

    //                        return readHandler(reader, receivedData.Metadata);
    //                    }
    //                }
    //                else
    //                {
    //                    var reader = new EntanglementProtocolReader(buffer);

    //                    return readHandler(reader, receivedData.Metadata);
    //                }
    //            }
    //        });
    //    }
    //    public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
    //    {
    //        Func<Task<TransportMessage>> receiveHandler;
    //        using (var buffer = this.MemoryStreamManager.GetStream())
    //        {
    //            if (this.Compress)
    //            {
    //                using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Compress))
    //                {
    //                    var writer = new EntanglementProtocolWriter(gzip);
    //                    handler(writer);
    //                }
    //            }
    //            else
    //            {
    //                var writer = new EntanglementProtocolWriter(buffer);
    //                handler(writer);

    //                buffer.Position = 0;
    //            }

    //            buffer.Flush();
    //            if (!buffer.TryGetBuffer(out var data))
    //                throw new Exception("Could not get data buffer");

    //            receiveHandler = await transport.SendAndReceive(new TransportMessage(data.ToArray(), VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
    //        }

    //        return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
    //            var receivedData = await receiveHandler();

    //            using (var buffer = this.MemoryStreamManager.GetStream(receivedData.Payload))
    //            {
    //                if (this.Compress)
    //                {
    //                    using (var gzip = new System.IO.Compression.DeflateStream(buffer, System.IO.Compression.CompressionMode.Decompress))
    //                    {
    //                        var reader = new EntanglementProtocolReader(gzip);

    //                        return readHandler(reader, receivedData.Metadata);
    //                    }
    //                }
    //                else
    //                {
    //                    var reader = new EntanglementProtocolReader(buffer);

    //                    return readHandler(reader, receivedData.Metadata);
    //                }
    //            }
    //        });
    //    }

    //    public override async Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
    //    {
    //        throw new NotImplementedException();
    //        //var receiveResults = await transport.ReceiveAndSend();

    //        //var receiveBuffer = this.MemoryStreamManager.GetStream(receiveResults.ReceivedData.Data);
    //        //var reader = new EntanglementProtocolReader(transport, this, receiveBuffer);
    //        //handler(reader, receiveResults.ReceivedData.Metadata);

    //        //return new Func<Action<IProtocolWriter>, Task>(async (writeHandler) =>
    //        //{
    //        //    using (var buffer = this.MemoryStreamManager.GetStream())
    //        //    {
    //        //        var writer = new EntanglementProtocolWriter(transport, this, buffer);
    //        //        writeHandler(writer);

    //        //        buffer.Position = 0;
    //        //        var data = buffer.ToArray();
    //        //        await receiveResults.SendHandler(data, receiveResults.ReceivedData.Metadata);
    //        //    }
    //        //});
    //    }
    //    public override async Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public class EntanglementProtocol : AProtocol
    {
        public static readonly string IDENTIFIER = "entanglement";
        public override string Identifier => IDENTIFIER;

        public bool Compress { get; }

        public EntanglementProtocol(EntanglementProtocolOptions options = default(EntanglementProtocolOptions))
            : base()
        {
            this.Compress = options.Compress;
        }

        public override void Read(Memory<byte> data, Action<IProtocolReader> handler)
        {
            using var reader = new EntanglementProtocolBufferReader(this, data);

            handler(reader);
        }
        public override T Read<T>(Memory<byte> data, Func<IProtocolReader, T> handler)
        {
            using var reader = new EntanglementProtocolBufferReader(this, data);

            return handler(reader);
        }
        public override Memory<byte> Write(Action<IProtocolWriter> handler)
        {
            using var writer = new EntanglementProtocolBufferWriter(this);
            handler(writer);

            //writer.Stats.WriteToConsole();

            return writer.Span.ToArray();
        }

        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using var writer = new EntanglementProtocolBufferWriter(this);
            handler(writer);

            await transport.Send(new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
        }
        public override async Task WriteData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            using var writer = new EntanglementProtocolBufferWriter(this);
            handler(writer);

            await transport.Send(new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            using var writer = new EntanglementProtocolBufferWriter(this);
            handler(writer);

            await transport.Send(messageId, new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
        }
        public override async Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            using var writer = new EntanglementProtocolBufferWriter(this);
            handler(writer);

            await transport.Send(messageId, new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
        }

        public override async Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive();

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            handler(reader, receivedData.Metadata);
        }
        public override async Task ReadData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(cancellationToken);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            handler(reader, receivedData.Metadata);
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive();

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            return handler(reader, receivedData.Metadata);
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(cancellationToken);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            return handler(reader, receivedData.Metadata);
        }
        public override async Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(messageId);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            handler(reader, receivedData.Metadata);
        }
        public override async Task ReadData(ITransport transport, string messageId, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler)
        {
            var receivedData = await transport.Receive(messageId, cancellationToken);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            handler(reader, receivedData.Metadata);
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(messageId);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            return handler(reader, receivedData.Metadata);
        }
        public override async Task<TResult> ReadData<TResult>(ITransport transport, string messageId, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.Receive(messageId, cancellationToken);

            if (receivedData.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
            return handler(reader, receivedData.Metadata);
        }

        public override async Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            if (receivedData.Message.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.Message.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Message.Payload));
            handler(reader, receivedData.Id, receivedData.Message.Metadata);
        }
        public override async Task ReadTaggedData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            var receivedData = await transport.ReceiveTagged(cancellationToken);

            if (receivedData.Message.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.Message.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Message.Payload));
            handler(reader, receivedData.Id, receivedData.Message.Metadata);
        }
        public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.ReceiveTagged();

            if (receivedData.Message.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.Message.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Message.Payload));
            return handler(reader, receivedData.Id, receivedData.Message.Metadata);
        }
        public override async Task<TResult> ReadTaggedData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            var receivedData = await transport.ReceiveTagged(cancellationToken);

            if (receivedData.Message.ProtocolIdentifier != this.Identifier)
                throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.Message.ProtocolIdentifier}]");

            using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Message.Payload));
            return handler(reader, receivedData.Id, receivedData.Message.Metadata);
        }

        public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var writer = new EntanglementProtocolBufferWriter(this))
            {
                handler(writer);

                receiveHandler = await transport.SendAndReceive(new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                if (receivedData.ProtocolIdentifier != this.Identifier)
                    throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

                using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
                readHandler(reader, receivedData.Metadata);
            });
        }
        public override async Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var writer = new EntanglementProtocolBufferWriter(this))
            {
                handler(writer);

                receiveHandler = await transport.SendAndReceive(new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }

            return new Func<Action<IProtocolReader, ITransportMetadata>, Task>(async (readHandler) => {
                var receivedData = await receiveHandler();

                if (receivedData.ProtocolIdentifier != this.Identifier)
                    throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

                using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
                readHandler(reader, receivedData.Metadata);
            });
        }
        public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var writer = new EntanglementProtocolBufferWriter(this))
            {
                handler(writer);

                receiveHandler = await transport.SendAndReceive(new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)));
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                if (receivedData.ProtocolIdentifier != this.Identifier)
                    throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

                using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
                return readHandler(reader, receivedData.Metadata);
            });
        }
        public override async Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler)
        {
            Func<Task<TransportMessage>> receiveHandler;
            using (var writer = new EntanglementProtocolBufferWriter(this))
            {
                handler(writer);

                receiveHandler = await transport.SendAndReceive(new TransportMessage(this.CompressData(writer.Span), this.Identifier, VolatileTransportMetadata.FromMetadata(metadata)), cancellationToken);
            }

            return new Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>(async (readHandler) => {
                var receivedData = await receiveHandler();

                if (receivedData.ProtocolIdentifier != this.Identifier)
                    throw new Exception($"Protocol mismatch [{this.Identifier} / {receivedData.ProtocolIdentifier}]");

                using var reader = new EntanglementProtocolBufferReader(this, this.DecompressData(receivedData.Payload));
                return readHandler(reader, receivedData.Metadata);
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

        private byte[] CompressData(ReadOnlySpan<byte> data)
        {
            if (!this.Compress)
                return data.ToArray();

            //var output = new byte[LZ4Codec.MaximumOutputSize(data.Length)];
            //var encodedLength = LZ4Codec.Encode(data, output);

            //return output[0..encodedLength];



            //Console.WriteLine($"Raw Data Length: {data.Length}");
            using (var ms = new MemoryStream())
            {
                using (var gzs = new System.IO.Compression.ZLibStream(ms, System.IO.Compression.CompressionLevel.Fastest))
                {
                    gzs.Write(data);
                }

                return ms.ToArray();
            }





            //var output = new byte[data.Length];
            //if (!System.IO.Compression.BrotliEncoder.TryCompress(data, output, out var encodedLength))
            //    throw new Exception("Could not compress data");

            //return output[0..encodedLength];
        }
        private byte[] DecompressData(ReadOnlySpan<byte> data)
        {
            if (!this.Compress)
                return data.ToArray();

            //var output = new byte[data.Length * 255];
            //var encodedLength = LZ4Codec.Decode(data, output);

            //return output[0..encodedLength];




            using (var ms = new MemoryStream(data.ToArray()))
            using (var oms = new MemoryStream())
            {
                using (var gzs = new System.IO.Compression.ZLibStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    gzs.CopyTo(oms);
                }

                return oms.ToArray();
            }





            //var output = new byte[data.Length];
            //if (!System.IO.Compression.BrotliDecoder.TryDecompress(data, output, out var encodedLength))
            //    throw new Exception("Could not decompress data");

            //return output[0..encodedLength];
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

    //    public class EntanglementProtocolWriter : AProtocolWriter
    //    {
    //        public Stream EncoderStream { get; private set; }

    //        private byte[] PrimitivesBuffer;

    //        public WriterStats Stats { get; } = new WriterStats();

    //        public EntanglementProtocolWriter(AProtocol protocol, Stream encoderStream)
    //            : base(protocol)
    //        {
    //            this.EncoderStream = encoderStream;

    //#if NETSTANDARD
    //            this.PrimitivesBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(2048);
    //#else
    //            this.PrimitivesBuffer = new byte[2048];
    //#endif
    //        }

    //        public override void WriteData(Span<byte> data)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public override void WriteStringValue(string value)
    //        {
    //            this.Stats.StringWrites++;

    //            var length = Encoding.UTF8.GetBytes(value, 0, value.Length, this.PrimitivesBuffer, 4);
    //            //for (var i = 0; i < value.Length; i++)
    //            //    this.PrimitivesBuffer[i] = (byte)(value[i] & 0x7f);
    //            //var buffer = System.Text.Encoding.UTF8.GetBytes(value);

    //            this.PrimitivesBuffer[0] = (byte)length;
    //            this.PrimitivesBuffer[1] = (byte)(length >> 8);
    //            this.PrimitivesBuffer[2] = (byte)(length >> 16);
    //            this.PrimitivesBuffer[3] = (byte)(length >> 24);

    //            //this.EncoderStream.Write(BitConverter.GetBytes(length), 0, 4);
    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
    //            this.EncoderStream.Write(this.PrimitivesBuffer, 4, length);
    //        }
    //        public override void WriteBooleanValue(bool value)
    //        {
    //            this.Stats.BooleanWrites++;

    //            this.PrimitivesBuffer[0] = value ? (byte)1 : (byte)0;

    //            this.EncoderStream.Write(BitConverter.GetBytes(value), 0, 1);
    //        }
    //        public override void WriteByteValue(byte value)
    //        {
    //            this.Stats.ByteWrites++;

    //            this.PrimitivesBuffer[0] = value;

    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 1);
    //        }
    //        public override void WriteShortValue(short value)
    //        {
    //            this.Stats.ShortWrites++;

    //            this.PrimitivesBuffer[0] = (byte)value;
    //            this.PrimitivesBuffer[1] = (byte)(value >> 8);

    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 2);
    //        }
    //        public override void WriteIntegerValue(int value)
    //        {
    //            this.Stats.IntegerWrites++;

    //            this.PrimitivesBuffer[0] = (byte)value;
    //            this.PrimitivesBuffer[1] = (byte)(value >> 8);
    //            this.PrimitivesBuffer[2] = (byte)(value >> 16);
    //            this.PrimitivesBuffer[3] = (byte)(value >> 24);

    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
    //        }
    //        public override void WriteLongValue(long value)
    //        {
    //            this.Stats.LongWrites++;

    //            this.PrimitivesBuffer[0] = (byte)value;
    //            this.PrimitivesBuffer[1] = (byte)(value >> 8);
    //            this.PrimitivesBuffer[2] = (byte)(value >> 16);
    //            this.PrimitivesBuffer[3] = (byte)(value >> 24);
    //            this.PrimitivesBuffer[4] = (byte)(value >> 32);
    //            this.PrimitivesBuffer[5] = (byte)(value >> 40);
    //            this.PrimitivesBuffer[6] = (byte)(value >> 48);
    //            this.PrimitivesBuffer[7] = (byte)(value >> 56);

    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 8);
    //        }
    //        public override unsafe void WriteFloatValue(float value)
    //        {
    //            this.Stats.FloatWrites++;

    //            uint tmpValue = *(uint*)&value;
    //            this.PrimitivesBuffer[0] = (byte)tmpValue;
    //            this.PrimitivesBuffer[1] = (byte)(tmpValue >> 8);
    //            this.PrimitivesBuffer[2] = (byte)(tmpValue >> 16);
    //            this.PrimitivesBuffer[3] = (byte)(tmpValue >> 24);

    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
    //        }
    //        public override unsafe void WriteDoubleValue(double value)
    //        {
    //            this.Stats.DoubleWrites++;

    //            ulong tmpValue = *(ulong*)&value;
    //            this.PrimitivesBuffer[0] = (byte)tmpValue;
    //            this.PrimitivesBuffer[1] = (byte)(tmpValue >> 8);
    //            this.PrimitivesBuffer[2] = (byte)(tmpValue >> 16);
    //            this.PrimitivesBuffer[3] = (byte)(tmpValue >> 24);
    //            this.PrimitivesBuffer[4] = (byte)(tmpValue >> 32);
    //            this.PrimitivesBuffer[5] = (byte)(tmpValue >> 40);
    //            this.PrimitivesBuffer[6] = (byte)(tmpValue >> 48);
    //            this.PrimitivesBuffer[7] = (byte)(tmpValue >> 56);

    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 8);
    //        }
    //        public override void WriteEnumValue<T>(T value)
    //        {
    //            this.Stats.EnumWrites++;

    //            var enumValue = value.ToInt32(System.Globalization.CultureInfo.InvariantCulture);

    //            this.PrimitivesBuffer[0] = (byte)enumValue;
    //            this.PrimitivesBuffer[1] = (byte)(enumValue >> 8);
    //            this.PrimitivesBuffer[2] = (byte)(enumValue >> 16);
    //            this.PrimitivesBuffer[3] = (byte)(enumValue >> 24);

    //            this.EncoderStream.Write(this.PrimitivesBuffer, 0, 4);
    //        }
    //        public override void WriteIndeterminateValue(object value)
    //        {
    //            throw new NotImplementedException("Indeterminate values not supported at this time");
    //        }

    //        public override void WriteRequestStart(RequestHeader header)
    //        {
    //            this.WriteStringValue(header.ActionName);
    //            this.WriteIntegerValue(header.ArgumentCount);
    //        }
    //        public override void WriteRequestEnd()
    //        {
    //        }

    //        public override void WriteRequestArgumentStart(RequestArgumentHeader header)
    //        {
    //            this.WriteStringValue(header.ArgumentName);
    //            this.WriteStringValue(header.Type);
    //        }
    //        public override void WriteRequestArgumentEnd()
    //        {
    //        }

    //        public override void WriteResponseStart(ResponseHeader header)
    //        {
    //            this.WriteBooleanValue(header.Success);
    //            this.WriteStringValue(header.Type);
    //            this.WriteIntegerValue(header.ArgumentCount);
    //        }
    //        public override void WriteResponseEnd()
    //        {
    //        }

    //        public override void WriteResponseArgumentStart(ResponseArgumentHeader header)
    //        {
    //            this.WriteStringValue(header.ArgumentName);
    //            this.WriteStringValue(header.Type);
    //        }
    //        public override void WriteResponseArgumentEnd()
    //        {
    //        }

    //        public override void WriteModelStart(ModelHeader header)
    //        {
    //            this.WriteStringValue(header.ModelName);
    //            this.WriteIntegerValue(header.PropertyCount);
    //        }
    //        public override void WriteModelEnd()
    //        {
    //        }

    //        public override void WriteModelPropertyStart(ModelPropertyHeader header)
    //        {
    //            this.WriteStringValue(header.PropertyName);
    //            this.WriteStringValue(header.Type);
    //        }
    //        public override void WriteModelPropertyEnd()
    //        {
    //        }

    //        public override void WriteArrayStart(ArrayHeader header)
    //        {
    //            this.WriteIntegerValue(header.ItemCount);
    //        }
    //        public override void WriteArrayEnd()
    //        {
    //        }

    //        public override void WriteArrayItemStart(ArrayItemHeader header)
    //        {
    //            this.WriteStringValue(header.Type);
    //        }
    //        public override void WriteArrayItemEnd()
    //        {
    //        }

    //        public override void WriteDictionaryStart(DictionaryHeader header)
    //        {
    //            this.WriteIntegerValue(header.RecordCount);
    //        }
    //        public override void WriteDictionaryEnd()
    //        {
    //        }

    //        public override void WriteDictionaryItemStart(DictionaryItemHeader header)
    //        {
    //            this.WriteStringValue(header.KeyType);
    //            this.WriteStringValue(header.ValueType);
    //        }
    //        public override void WriteDictionaryItemEnd()
    //        {
    //        }

    //        public override void WriteIndefiniteValueStart(IndefiniteValueHeader header)
    //        {
    //            this.WriteStringValue(header.ValueType);
    //        }
    //        public override void WriteIndefiniteValueEnd()
    //        {
    //        }
    //    }

    //    public class EntanglementProtocolReader : AProtocolReader
    //    {
    //        public Stream DecoderStream { get; private set; }

    //        private byte[] PrimitivesBuffer;

    //        public EntanglementProtocolReader(AProtocol protocol, Stream decoderStream)
    //            : base(protocol)
    //        {
    //            this.DecoderStream = decoderStream;

    //#if NETSTANDARD
    //            this.PrimitivesBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(2048);
    //#else
    //            this.PrimitivesBuffer = new byte[2048];
    //#endif
    //        }

    //        public override Span<byte> ReadData()
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public override string ReadStringValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);
    //            var length = BitConverter.ToInt32(this.PrimitivesBuffer, 0);

    //            if (length > 0)
    //            {
    //                //var contentBuffer = new byte[length];
    //                //Console.WriteLine(length);
    //                this.DecoderStream.Read(this.PrimitivesBuffer, 0, length);

    //                return System.Text.Encoding.UTF8.GetString(this.PrimitivesBuffer, 0, length);
    //            }
    //            else
    //            {
    //                return string.Empty;
    //            }
    //        }
    //        public override bool ReadBooleanValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 1);

    //            return BitConverter.ToBoolean(this.PrimitivesBuffer, 0);
    //        }
    //        public override byte ReadByteValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 1);

    //            return this.PrimitivesBuffer[0];
    //        }
    //        public override short ReadShortValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 2);

    //            return BitConverter.ToInt16(this.PrimitivesBuffer, 0);
    //        }
    //        public override int ReadIntegerValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);

    //            return BitConverter.ToInt32(this.PrimitivesBuffer, 0);
    //        }
    //        public override long ReadLongValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 8);

    //            return BitConverter.ToInt64(this.PrimitivesBuffer, 0);
    //        }
    //        public override float ReadFloatValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);

    //            return BitConverter.ToSingle(this.PrimitivesBuffer, 0);
    //        }
    //        public override double ReadDoubleValue()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 8);

    //            return BitConverter.ToDouble(this.PrimitivesBuffer, 0);
    //        }
    //        public override T ReadEnumValue<T>()
    //        {
    //            this.DecoderStream.Read(this.PrimitivesBuffer, 0, 4);

    //            var enumValue = BitConverter.ToInt32(this.PrimitivesBuffer, 0);

    //            return (T)Enum.ToObject(typeof(T), enumValue);
    //        }
    //        public override object ReadIndeterminateValue()
    //        {
    //            throw new NotImplementedException("Indeterminate values not supported at this time");
    //        }

    //        public override RequestHeader ReadRequestStart()
    //        {
    //            var actionName = this.ReadStringValue();
    //            var argumentCount = this.ReadIntegerValue();

    //            return new RequestHeader(actionName, argumentCount);
    //        }
    //        public override void ReadRequestEnd()
    //        {
    //        }

    //        public override RequestArgumentHeader ReadRequestArgumentStart()
    //        {
    //            var argumentName = this.ReadStringValue();
    //            var type = this.ReadStringValue();

    //            return new RequestArgumentHeader(argumentName, type);
    //        }
    //        public override void ReadRequestArgumentEnd()
    //        {
    //        }

    //        public override ResponseHeader ReadResponseStart()
    //        {
    //            var success = this.ReadBooleanValue();
    //            var type = this.ReadStringValue();
    //            var argumentCount = this.ReadIntegerValue();

    //            return new ResponseHeader(success, type, argumentCount);
    //        }
    //        public override void ReadResponseEnd()
    //        {
    //        }

    //        public override ResponseArgumentHeader ReadResponseArgumentStart()
    //        {
    //            var argumentName = this.ReadStringValue();
    //            var type = this.ReadStringValue();

    //            return new ResponseArgumentHeader(argumentName, type);
    //        }
    //        public override void ReadResponseArgumentEnd()
    //        {
    //        }

    //        public override ModelHeader ReadModelStart()
    //        {
    //            var modelName = this.ReadStringValue();
    //            var propertyCount = this.ReadIntegerValue();

    //            return new ModelHeader(modelName, propertyCount);
    //        }
    //        public override void ReadModelEnd()
    //        {
    //        }

    //        public override ModelPropertyHeader ReadModelPropertyStart()
    //        {
    //            var propertyName = this.ReadStringValue();
    //            var type = this.ReadStringValue();

    //            return new ModelPropertyHeader(propertyName, type);
    //        }
    //        public override void ReadModelPropertyEnd()
    //        {
    //        }

    //        public override ArrayHeader ReadArrayStart()
    //        {
    //            var itemCount = this.ReadIntegerValue();

    //            return new ArrayHeader(itemCount);
    //        }
    //        public override void ReadArrayEnd()
    //        {
    //        }

    //        public override ArrayItemHeader ReadArrayItemStart()
    //        {
    //            var type = this.ReadStringValue();

    //            return new ArrayItemHeader(type);
    //        }
    //        public override void ReadArrayItemEnd()
    //        {
    //        }

    //        public override DictionaryHeader ReadDictionaryStart()
    //        {
    //            var recordCount = this.ReadIntegerValue();

    //            return new DictionaryHeader(recordCount);
    //        }
    //        public override void ReadDictionaryEnd()
    //        {
    //        }

    //        public override DictionaryItemHeader ReadDictionaryItemStart()
    //        {
    //            var keyType = this.ReadStringValue();
    //            var valueType = this.ReadStringValue();

    //            return new DictionaryItemHeader(keyType, valueType);
    //        }
    //        public override void ReadDictionaryItemEnd()
    //        {
    //        }

    //        public override IndefiniteValueHeader ReadIndefiniteValueStart()
    //        {
    //            var valueType = this.ReadStringValue();

    //            return new IndefiniteValueHeader(valueType);
    //        }
    //        public override void ReadIndefiniteValueEnd()
    //        {
    //        }
    //    }

    public class IncrementalHashWriter : AProtocolWriter
    {
        public IncrementalHash Hasher { get; }

        public IncrementalHashWriter(AProtocol protocol)
            : base(protocol)
        {
            this.Hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        }
        
        public override void WriteData(Span<byte> data)
        {
            this.Hasher.AppendData(data);
        }

        public override void WriteStringValue(string value)
        {
            var length = Encoding.UTF8.GetByteCount(value);
            var stringBuffer = ArrayPool<byte>.Shared.Rent(length);
            Encoding.UTF8.GetBytes(value, 0, value.Length, stringBuffer, 0);
            this.Hasher.AppendData(stringBuffer, 0, length);
            ArrayPool<byte>.Shared.Return(stringBuffer);
        }
        public override void WriteBooleanValue(bool value)
        {
            this.Hasher.AppendData(BitConverter.GetBytes(value));
        }
        public override void WriteByteValue(byte value)
        {
            this.Hasher.AppendData(new byte[] { value });
        }
        public override void WriteShortValue(short value)
        {
            this.Hasher.AppendData(BitConverter.GetBytes(value));
        }
        public override void WriteIntegerValue(int value)
        {
            this.Hasher.AppendData(BitConverter.GetBytes(value));
        }
        public override void WriteLongValue(long value)
        {
            this.Hasher.AppendData(BitConverter.GetBytes(value));
        }
        public override void WriteFloatValue(float value)
        {
            this.Hasher.AppendData(BitConverter.GetBytes(value));
        }
        public override void WriteDoubleValue(double value)
        {
            this.Hasher.AppendData(BitConverter.GetBytes(value));
        }
        public override void WriteEnumValue<T>(T value)
        {
            this.Hasher.AppendData(BitConverter.GetBytes(value.ToInt32(System.Globalization.CultureInfo.InvariantCulture)));
        }
        public override void WriteIndeterminateValue(object value)
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }

        public override void WriteHashedBlock(Action<IProtocolWriter> writerHandler)
        {
            var data = this.Protocol.Write(writer => writerHandler(writer));
            this.Hasher.AppendData(data.Span);
        }
        public override void WriteHashedBlock(string hash, Action<IProtocolWriter> writerHandler)
        {
            var data = this.Protocol.Write(writer => writerHandler(writer));
            this.Hasher.AppendData(data.Span);
        }
        public override void WriteHashedBlock(Func<IncrementalHash, byte[], string> hashHandler, Action<IProtocolWriter> writerHandler)
        {
            var data = this.Protocol.Write(writer => writerHandler(writer));
            this.Hasher.AppendData(data.Span);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Hasher.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    internal class EntanglementProtocolBufferWriterIndex : IDisposable
    {
        private bool disposedValue;

        public readonly Dictionary<string, int> Dictionary = new Dictionary<string, int>();
        public readonly Entanglement.BinaryWriter Writer = new Entanglement.BinaryWriter(Entanglement.BinaryWriter.DefaultSize);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Writer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EntanglementProtocolBufferWriterIndex()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class EntanglementProtocolBufferWriter : AProtocolWriter
    {
        //public WriterStats Stats { get; } = new WriterStats();

        private Dictionary<string, int> IndexDictionary;
        private Entanglement.BinaryWriter IndexWriter;
        private IncrementalHash Hasher;
        private byte[] HasherBuffer;
        private bool Forked;

        private Entanglement.BinaryWriter Writer = new Entanglement.BinaryWriter(Entanglement.BinaryWriter.DefaultSize);

        public ReadOnlySpan<byte> Span
        {
            get => this.ResolveData();
        }

        public EntanglementProtocolBufferWriter(AProtocol protocol)
            : base(protocol)
        {
            this.IndexDictionary = new Dictionary<string, int>();
            this.IndexWriter = new Entanglement.BinaryWriter(Entanglement.BinaryWriter.DefaultSize);
            this.Hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            this.HasherBuffer = ArrayPool<byte>.Shared.Rent(32);
        }
        internal EntanglementProtocolBufferWriter(AProtocol protocol, Dictionary<string, int> indexDictionary, Entanglement.BinaryWriter indexWriter, IncrementalHash hasher, byte[] hasherBuffer)
            : base(protocol)
        {
            this.IndexDictionary = indexDictionary;
            this.IndexWriter = indexWriter;
            this.Hasher = hasher;
            this.HasherBuffer = hasherBuffer;
            this.Forked = true;
        }

        public EntanglementProtocolBufferWriter Fork()
        {
            return new EntanglementProtocolBufferWriter(this.Protocol, this.IndexDictionary, this.IndexWriter, this.Hasher, this.HasherBuffer);
        }

        public Span<byte> ResolveData()
        {
            var writer = new Entanglement.BinaryWriter(this.IndexWriter.Span.Length + this.Writer.Span.Length);
            writer.Write(this.IndexWriter.Span.Length);
            writer.Write(this.IndexWriter.Span);

            //writer.Write(this.Writer.Span.Length);
            writer.Write(this.Writer.Span);

            //Console.WriteLine("Index Count: " + this.Index.Count);
            //Console.WriteLine("Index Length: " + this.IndexWriter.Span.Length);
            //Console.WriteLine("Payload Length: " + this.Writer.Span.Length);
            //this.Stats.WriteToConsole();

            return writer.Span;
        }

        public override void WriteData(Span<byte> data)
        {
            this.Writer.Write(data.Length);
            this.Writer.Write(data);
        }

        public override void WriteStringValue(string value)
        {
            //this.Stats.StringWrites++;

            if (this.IndexDictionary.TryGetValue(value, out var pos))
            {
                this.Writer.Write(pos);
            }
            else
            {
                this.IndexDictionary.Add(value, this.IndexWriter.Position);
                this.Writer.Write(this.IndexWriter.Position);

                var length = Encoding.UTF8.GetByteCount(value);
                this.IndexWriter.Write(length);

                this.IndexWriter.EnsureCapacity(length);
                if (!MemoryMarshal.TryGetArray(this.IndexWriter.Memory.Slice(this.IndexWriter.Position, length), out ArraySegment<byte> segment))
                    throw new Exception("Could not aquire memory segment for string encoding.");
                Encoding.UTF8.GetBytes(value, 0, value.Length, segment.Array, segment.Offset);
                this.IndexWriter.Advance(length);
            }




            //int length = Encoding.UTF8.GetByteCount(value);

            //this.Writer.Write(length);

            //this.Writer.EnsureCapacity(length);
            //if (!MemoryMarshal.TryGetArray(this.Writer.Memory.Slice(this.Writer.Position, length), out ArraySegment<byte> segment))
            //    throw new Exception("Could not aquire memory segment for string encoding.");
            //Encoding.UTF8.GetBytes(value, 0, value.Length, segment.Array, segment.Offset);
            //this.Writer.Advance(length);




            //var encoded = Encoding.UTF8.GetBytes(value);
            //this.Writer.Write(encoded.Length);
            //this.Writer.Write(encoded.AsSpan());
        }
        public override void WriteBooleanValue(bool value)
        {
            //this.Stats.BooleanWrites++;

            this.Writer.Write(value);
        }
        public override void WriteByteValue(byte value)
        {
            //this.Stats.ByteWrites++;

            this.Writer.Write(value);
        }
        public override void WriteShortValue(short value)
        {
            //this.Stats.ShortWrites++;

            this.Writer.Write(value);
        }
        public override void WriteIntegerValue(int value)
        {
            //this.Stats.IntegerWrites++;

            this.Writer.Write(value);
        }
        public override void WriteLongValue(long value)
        {
            //this.Stats.LongWrites++;

            this.Writer.Write(value);
        }
        public override void WriteFloatValue(float value)
        {
            //this.Stats.FloatWrites++;

            this.Writer.Write(value);
        }
        public override void WriteDoubleValue(double value)
        {
            //this.Stats.DoubleWrites++;

            this.Writer.Write(value);
        }
        public override void WriteEnumValue<T>(T value)
        {
            //this.Stats.EnumWrites++;

            this.Writer.Write(value.ToInt32(System.Globalization.CultureInfo.InvariantCulture));
        }
        public override void WriteIndeterminateValue(object value)
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }

        public override void WriteHashedBlock(Action<IProtocolWriter> writerHandler)
        {
            ////string hash;
            ////using (var hashWriter = new IncrementalHashWriter(this.Protocol))
            ////{
            ////    hashHandler(hashWriter);

            ////    var encodedHash = hashWriter.Hasher.GetHashAndReset();
            ////    hash = Convert.ToHexString(encodedHash);
            ////}

            //Span<byte> data;
            //using (var forkedWriter = this.Fork())
            //{
            //    writerHandler(forkedWriter);
            //    //data = forkedWriter.Writer.Span;
            //    data = forkedWriter.Writer.Span.ToArray();

            //    //data = ArrayPool<byte>.Shared.Rent(forkedWriter.Writer.Span.Length);
            //    ////data = new byte[forkedWriter.Writer.Span.Length];
            //    //forkedWriter.Writer.Span.CopyTo(data);
            //}

            ////var data = this.Protocol.Write(writer => writerHandler(writer));

            //string hash;
            //using (var sha256 = SHA256.Create())
            //{
            //    var hashBytes = ArrayPool<byte>.Shared.Rent(32);
            //    try
            //    {
            //        if (!sha256.TryComputeHash(data, hashBytes, out var length))
            //            throw new Exception("Could not compute hash of block");

            //        hash = Convert.ToHexString(hashBytes);
            //    }
            //    finally
            //    {
            //        ArrayPool<byte>.Shared.Return(hashBytes);
            //    }
            //}
            ////var hash = Blake3.Hasher.Hash(data).ToString();
            ////var hash = Guid.NewGuid().ToString();

            //if (this.IndexDictionary.TryGetValue(hash, out var pos))
            //{
            //    this.Writer.Write(pos);
            //}
            //else
            //{
            //    this.IndexDictionary.Add(hash, this.IndexWriter.Position);
            //    this.Writer.Write(this.IndexWriter.Position);

            //    //this.IndexWriter.Write(data.Length);
            //    this.IndexWriter.Write(data);
            //}

            ////ArrayPool<byte>.Shared.Return(data);

            throw new NotImplementedException();
        }
        public override void WriteHashedBlock(string hash, Action<IProtocolWriter> writerHandler)
        {
            //string hash;
            //using (var hashWriter = new IncrementalHashWriter(this.Protocol))
            //{
            //    hashHandler(hashWriter);

            //    var encodedHash = hashWriter.Hasher.GetHashAndReset();
            //    hash = Convert.ToHexString(encodedHash);
            //}

            //Span<byte> data;
            //using (var forkedWriter = this.Fork())
            //{
            //    writerHandler(forkedWriter);
            //    //data = forkedWriter.Writer.Span;
            //    data = forkedWriter.Writer.Span.ToArray();

            //    //data = ArrayPool<byte>.Shared.Rent(forkedWriter.Writer.Span.Length);
            //    ////data = new byte[forkedWriter.Writer.Span.Length];
            //    //forkedWriter.Writer.Span.CopyTo(data);
            //}

            //var data = this.Protocol.Write(writer => writerHandler(writer));

            //string hash;
            //using (var sha256 = SHA256.Create())
            //{
            //    var hashBytes = ArrayPool<byte>.Shared.Rent(32);
            //    try
            //    {
            //        if (!sha256.TryComputeHash(data, hashBytes, out var length))
            //            throw new Exception("Could not compute hash of block");

            //        hash = Convert.ToHexString(hashBytes);
            //    }
            //    finally
            //    {
            //        ArrayPool<byte>.Shared.Return(hashBytes);
            //    }
            //}
            //var hash = Blake3.Hasher.Hash(data).ToString();
            //var hash = Guid.NewGuid().ToString();

            if (this.IndexDictionary.TryGetValue(hash, out var pos))
            {
                this.Writer.Write(pos);
            }
            else
            {
                Span<byte> data;
                using (var forkedWriter = this.Fork())
                {
                    writerHandler(forkedWriter);

                    this.IndexDictionary.Add(hash, this.IndexWriter.Position);
                    this.Writer.Write(this.IndexWriter.Position);

                    this.IndexWriter.Write(forkedWriter.Writer.Span);
                }
            }

            //ArrayPool<byte>.Shared.Return(data);
        }
        public override void WriteHashedBlock(Func<IncrementalHash, byte[], string> hashHandler, Action<IProtocolWriter> writerHandler)
        {
            var hash = hashHandler(this.Hasher, this.HasherBuffer);
            this.WriteHashedBlock(hash, writerHandler);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.Forked)
                {
                    this.IndexWriter.Dispose();
                    this.Hasher.Dispose();
                    ArrayPool<byte>.Shared.Return(this.HasherBuffer);
                }

                this.Writer.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    public class EntanglementProtocolBufferReader : AProtocolReader
    {
        public Memory<byte> Buffer { get; }
        private int Position { get; set; }

        private Dictionary<int, string> Index = new Dictionary<int, string>();
        //private (int, int) IndexBounds { get; }
        //private Entanglement.BinaryReader IndexReader
        //{
        //    get
        //    {
        //        return new Entanglement.BinaryReader(this.Buffer.Span[4..]);
        //    }
        //}

        private Entanglement.BinaryReader Reader
        {
            get
            {
                return new Entanglement.BinaryReader(this.Buffer.Span[4..]);
            }
        }

        public EntanglementProtocolBufferReader(AProtocol protocol, Memory<byte> buffer)
            : base(protocol)
        {
            this.Buffer = buffer;

            var reader = new Entanglement.BinaryReader(this.Buffer.Span);
            this.Position = reader.Read<int>();
        }
        private EntanglementProtocolBufferReader(AProtocol protocol, Memory<byte> buffer, int position)
            : base(protocol)
        {
            this.Buffer = buffer;
            this.Position = position;
        }

        public override Span<byte> ReadData()
        {
            var length = this.Reader.Read<int>(this.Position);
            this.Position += 4;

            var data = this.Buffer.Span.Slice(4 + this.Position, length);
            this.Position += length;

            return data;
        }

        public override string ReadStringValue()
        {
            var indexPos = this.Reader.Read<int>(this.Position);
            this.Position += 4;

            if (this.Index.TryGetValue(indexPos, out var value))
            {
                return value;
            }
            else
            {
                var length = this.Reader.Read<int>(indexPos);

                string content;
                if (length > 0)
                    content = String.Create(length, new { Buffer = this.Buffer, Position = 4 + indexPos + 4 }, (chars, state) => Encoding.UTF8.GetString(state.Buffer.Span.Slice(state.Position, length)).AsSpan().CopyTo(chars));
                else
                    content = string.Empty;

                this.Index.Add(indexPos, content);

                return content;
            }





            //var length = this.Reader.Read<int>();
            //this.Position += 4;

            //string content;
            //if (length > 0)
            //    //content = Encoding.UTF8.GetString(this.Buffer.Span.Slice(this.Position, length));
            //    content = String.Create(length, new { Buffer = this.Buffer, Position = this.Position }, (chars, state) => Encoding.UTF8.GetString(state.Buffer.Span.Slice(state.Position, length)).AsSpan().CopyTo(chars));
            //else
            //    content = string.Empty;
            //this.Position += length;

            //return content;






            //var length = BitConverter.ToInt32(this.Buffer.Span[this.Position..]);
            //this.Position += 4;

            //string content;
            //if (length > 0)
            //    content = Encoding.UTF8.GetString(this.Buffer.Span.Slice(this.Position, length));
            //else
            //    content = string.Empty;
            //this.Position += length;

            //return content;
        }
        public override bool ReadBooleanValue()
        {
            var value = this.Reader.Read<bool>(this.Position);
            this.Position += 1;
            return value;

            //var value = BitConverter.ToBoolean(this.Buffer.Span[this.Position..]);
            //this.Position += 1;

            //return value;
        }
        public override byte ReadByteValue()
        {
            var value = this.Reader.Read<byte>(this.Position);
            this.Position += 1;
            return value;

            //var value = this.Buffer.Span[this.Position];
            //this.Position += 1;

            //return value;
        }
        public override short ReadShortValue()
        {
            var value = this.Reader.Read<short>(this.Position);
            this.Position += 2;
            return value;

            //var value = BitConverter.ToInt16(this.Buffer.Span[this.Position..]);
            //this.Position += 2;

            //return value;
        }
        public override int ReadIntegerValue()
        {
            var value = this.Reader.Read<int>(this.Position);
            this.Position += 4;
            return value;

            //var value = BitConverter.ToInt32(this.Buffer.Span[this.Position..]);
            //this.Position += 4;

            //return value;
        }
        public override long ReadLongValue()
        {
            var value = this.Reader.Read<long>(this.Position);
            this.Position += 8;
            return value;

            //var value = BitConverter.ToInt64(this.Buffer.Span[this.Position..]);
            //this.Position += 8;

            //return value;
        }
        public override float ReadFloatValue()
        {
            var value = this.Reader.Read<float>(this.Position);
            this.Position += 4;
            return value;

            //var value = BitConverter.ToSingle(this.Buffer.Span[this.Position..]);
            //this.Position += 4;

            //return value;
        }
        public override double ReadDoubleValue()
        {
            var value = this.Reader.Read<double>(this.Position);
            this.Position += 8;
            return value;

            //var value = BitConverter.ToDouble(this.Buffer.Span[this.Position..]);
            //this.Position += 8;

            //return value;
        }
        public override T ReadEnumValue<T>()
        {
            var value = (T)Enum.ToObject(typeof(T), this.Reader.Read<int>(this.Position));
            this.Position += 4;
            return value;

            //var enumValue = BitConverter.ToInt32(this.Buffer.Span[this.Position..]);
            //this.Position += 4;

            //return (T)Enum.ToObject(typeof(T), enumValue);
        }
        public override object ReadIndeterminateValue()
        {
            throw new NotImplementedException("Indeterminate values not supported at this time");
        }

        public override void ReadHashedBlock(Action<IProtocolReader> readHandler)
        {
            var indexPos = this.Reader.Read<int>(this.Position);
            this.Position += 4;

            //var length = this.IndexReader.Read<int>(indexPos);
            //var data = this.Buffer.Slice(4 + indexPos + 4, length);

            var forkedReader = new EntanglementProtocolBufferReader(this.Protocol, this.Buffer, indexPos);
            readHandler(forkedReader);
        }
        public override T ReadHashedBlock<T>(Func<IProtocolReader, T> readHandler)
        {
            var indexPos = this.Reader.Read<int>(this.Position);
            this.Position += 4;

            //var length = this.IndexReader.Read<int>(indexPos);
            //var data = this.Buffer.Slice(4 + indexPos + 4, length);

            var forkedReader = new EntanglementProtocolBufferReader(this.Protocol, this.Buffer, indexPos);
            return readHandler(forkedReader);
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

namespace Axon.Entanglement
{
    // Sourced from https://github.com/Sergio0694/BinaryPack/tree/master/src/BinaryPack/Serialization/Buffers

    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Diagnostics.Contracts;

    internal ref struct BinaryReader
    {
        /// <summary>
        /// The <see cref="Span{T}"/> current in use
        /// </summary>
        private readonly Span<byte> Buffer;

        /// <summary>
        /// The current position into <see cref="Buffer"/>
        /// </summary>
        private int _Position;

        /// <summary>
        /// Creates a new <see cref="BinaryReader"/> instance with the given parameters
        /// </summary>
        /// <param name="buffer">The source<see cref="Span{T}"/> to read data from</param>
        public BinaryReader(Span<byte> buffer)
        {
            Buffer = buffer;
            _Position = 0;
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> from the underlying buffer
        /// </summary>
        /// <typeparam name="T">The type of value to read from the buffer</typeparam>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() where T : unmanaged
        {
            /* The reasons for declaring this struct as a ref struct, and for
             * carrying a Span<byte> instead of a byte[] array are that this way
             * the reader can also read data from memory areas that are now owned
             * by the caller, or data that is just a slice on another array.
             * These variable declarations are just for clarity, they are
             * all optimized away bit the JIT compiler anyway. */
            ref byte r0 = ref Buffer[_Position];
            T value = Unsafe.As<byte, T>(ref r0);
            _Position += Unsafe.SizeOf<T>();

            return value;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>(int position) where T : unmanaged
        {
            /* The reasons for declaring this struct as a ref struct, and for
             * carrying a Span<byte> instead of a byte[] array are that this way
             * the reader can also read data from memory areas that are now owned
             * by the caller, or data that is just a slice on another array.
             * These variable declarations are just for clarity, they are
             * all optimized away bit the JIT compiler anyway. */
            ref byte r0 = ref Buffer[position];
            T value = Unsafe.As<byte, T>(ref r0);

            return value;
        }

        /// <summary>
        /// Reads a sequence of elements of type <typeparamref name="T"/> from the underlying buffer
        /// </summary>
        /// <typeparam name="T">The type of elements to read from the buffer</typeparam>
        /// <param name="span">The target <see cref="Span{T}"/> to write the read elements to</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read<T>(Span<T> span) where T : unmanaged
        {
            /* This method is only invoked by the internal deserializers,
             * which already perform bounds check before creating the target Span<T>.
             * Since the input Span<T> is guaranteed to never be empty,
             * we can use GetPinnableReference() instead of the this[int]
             * indexer and skip one extra conditional jump in the JITted code. */
            int size = Unsafe.SizeOf<T>() * span.Length;
            ref T r0 = ref span.GetPinnableReference();
            ref byte r1 = ref Unsafe.As<T, byte>(ref r0);
            Span<byte> destination = MemoryMarshal.CreateSpan(ref r1, size);
            Span<byte> source = Buffer.Slice(_Position, size);

            source.CopyTo(destination);
            _Position += size;
        }
    }

    /// <summary>
    /// A <see langword="struct"/> that provides a fast implementation of a binary writer, leveraging <see cref="ArrayPool{T}"/> for memory pooling
    /// </summary>
    internal class BinaryWriter
    {
        /// <summary>
        /// The default size to use to create new <see cref="BinaryWriter"/> instances
        /// </summary>
        public const int DefaultSize = 1024;

        /// <summary>
        /// The <see cref="byte"/> array current in use
        /// </summary>
        private byte[] _Buffer;

        /// <summary>
        /// The current position into <see cref="_Buffer"/>
        /// </summary>
        private int _Position;

        /// <summary>
        /// Creates a new <see cref="BinaryWriter"/> instance with the given parameters
        /// </summary>
        /// <param name="initialSize">The initial size of the internal buffer</param>
        public BinaryWriter(int initialSize)
        {
            _Buffer = ArrayPool<byte>.Shared.Rent(initialSize);
            _Position = 0;
        }

        public int Position
        {
            get => this._Position;
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{T}"/> instance mapping the used content of the underlying buffer
        /// </summary>
        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<byte>(_Buffer, 0, _Position);
        }

        public Memory<byte> Memory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Memory<byte>(_Buffer, 0, _Buffer.Length);
        }

        /// <summary>
        /// Writes a value of type <typeparamref name="T"/> to the underlying buffer
        /// </summary>
        /// <typeparam name="T">The type of value to write to the buffer</typeparam>
        /// <param name="value">The <typeparamref name="T"/> value to write to the buffer</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value) where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();

            EnsureCapacity(size);

            Unsafe.As<byte, T>(ref _Buffer[_Position]) = value;
            _Position += size;
        }

        /// <summary>
        /// Writes the contents of the input <see cref="Span{T}"/> instance to the underlying buffer
        /// </summary>
        /// <typeparam name="T">The type of values to write to the buffer</typeparam>
        /// <param name="span">The input <see cref="Span{T}"/> value to write to the buffer</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(Span<T> span) where T : unmanaged
        {
            int
                elementSize = Unsafe.SizeOf<T>(),
                totalSize = elementSize * span.Length;

            EnsureCapacity(totalSize);

            ref T r0 = ref span.GetPinnableReference();
            ref byte r1 = ref Unsafe.As<T, byte>(ref r0);

            MemoryMarshal.CreateSpan(ref r1, totalSize).CopyTo(_Buffer.AsSpan(_Position, totalSize));
            _Position += totalSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            _Position += count;
        }

        /// <summary>
        /// Ensures the buffer in use has the capacity to contain the specified amount of new data
        /// </summary>
        /// <param name="count">The size in bytes of the new data to insert into the buffer</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int count)
        {
            int
                currentLength = _Buffer.Length,
                requiredLength = _Position + count;

            if (requiredLength <= currentLength) return;

            if (currentLength == 0x7FFFFFC7) throw new InvalidOperationException("Maximum size for a byte[] array exceeded (0x7FFFFFC7), see: https://msdn.microsoft.com/en-us/library/system.array");

            // Calculate the new size of the target array
            int targetLength = requiredLength.UpperBoundLog2();
            if (targetLength < 0) targetLength = 0x7FFFFFC7;

            // Rent the new array and copy the content of the current array
            byte[] rent = ArrayPool<byte>.Shared.Rent(targetLength);
            Unsafe.CopyBlock(ref rent[0], ref _Buffer[0], (uint)_Position);

            // Return the old buffer and swap it
            ArrayPool<byte>.Shared.Return(_Buffer);
            _Buffer = rent;
        }

        /// <summary>
        /// Disposes the current instance, like <see cref="IDisposable.Dispose"/>
        /// </summary>
        public void Dispose() => ArrayPool<byte>.Shared.Return(_Buffer);
    }

    /// <summary>
    /// A <see langword="class"/> that provides extension methods for numeric types
    /// </summary>
    internal static class NumericExtensions
    {
        /// <summary>
        /// C# no-alloc optimization that maps to the data section, see <see href="https://github.com/dotnet/roslyn/pull/24621"/>
        /// </summary>
        private static ReadOnlySpan<byte> Log2DeBruijn => new byte[]
        {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
        };

        /// <summary>
        /// Calculates the upper bound of the log base 2 of the input value
        /// </summary>
        /// <param name="n">The input value to compute the bound for</param>
        /// <remarks>Main body pulled from <see href="https://source.dot.net/#System.Private.CoreLib/shared/System/Numerics/BitOperations.cs"/></remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UpperBoundLog2(this int n)
        {
            uint value = (uint)n - 1;

            // Fill trailing zeros with ones, eg 00010010 becomes 00011111
            value |= value >> 01;
            value |= value >> 02;
            value |= value >> 04;
            value |= value >> 08;
            value |= value >> 16;

            // Compute the log2 and adjust for the upper bound
            return 1 << (Unsafe.AddByteOffset(
                ref MemoryMarshal.GetReference(Log2DeBruijn),
                (IntPtr)(int)((value * 0x07C4ACDDu) >> 27)) + 1);
        }
    }
}
