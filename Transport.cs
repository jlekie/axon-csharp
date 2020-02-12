using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axon
{
    public interface ITransportMetadata
    {
        IEnumerable<ITransportMetadataFrame> Frames { get; }

        bool Has(string id);

        byte[] Find(string id);
        byte[] Get(string id);
        bool TryGet(string id, out byte[] data);

        byte[][] GetAll(string id);

        byte[] FindFirst(string id);
        byte[] GetFirst(string id);
        bool TryGetFirst(string id, out byte[] data);

        byte[] FindLast(string id);
        byte[] GetLast(string id);
        bool TryGetLast(string id, out byte[] data);
    }
    public interface ITransportMetadataFrame
    {
        string Id { get; }
        byte[] Data { get; }
    }

    public class TransportMetadata : ITransportMetadata
    {
        public static TransportMetadata FromMetadata(ITransportMetadata source)
        {
            return new TransportMetadata(source.Frames.Select(f => TransportMetadataFrame.FromMetadataFrame(f)).ToArray());
        }

        public readonly TransportMetadataFrame[] Frames;
        IEnumerable<ITransportMetadataFrame> ITransportMetadata.Frames => this.Frames;

        public TransportMetadata()
        {
            this.Frames = new TransportMetadataFrame[] { };
        }
        public TransportMetadata(TransportMetadataFrame[] frames)
        {
            this.Frames = frames;
        }

        public bool Has(string id)
        {
            return this.Frames.Any(m => m.Id == id);
        }
        public byte[] Find(string id)
        {
            this.TryGet(id, out var data);

            return data;
        }
        public byte[] Get(string id)
        {
            var success = this.TryGet(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryGet(string id, out byte[] data)
        {
            var metadata = this.Frames.SingleOrDefault(m => m.Id == id);
            data = metadata?.Data;

            return metadata != null;
        }

        public byte[][] GetAll(string id)
        {
            return this.Frames.Where(m => m.Id == id).Select(m => m.Data).ToArray();
        }

        public byte[] FindFirst(string id)
        {
            this.TryGetFirst(id, out var data);

            return data;
        }
        public byte[] GetFirst(string id)
        {
            var success = this.TryGetFirst(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryGetFirst(string id, out byte[] data)
        {
            var metadata = this.Frames.FirstOrDefault(m => m.Id == id);
            data = metadata?.Data;

            return metadata != null;
        }

        public byte[] FindLast(string id)
        {
            this.TryGetLast(id, out var data);

            return data;
        }
        public byte[] GetLast(string id)
        {
            var success = this.TryGetLast(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryGetLast(string id, out byte[] data)
        {
            var metadata = this.Frames.LastOrDefault(m => m.Id == id);
            data = metadata?.Data;

            return metadata != null;
        }
    }
    public class VolatileTransportMetadata : ITransportMetadata
    {
        public static VolatileTransportMetadata FromMetadata(ITransportMetadata source)
        {
            return new VolatileTransportMetadata(source.Frames.Select(f => VolatileTransportMetadataFrame.FromMetadataFrame(f)).ToList());
        }

        public readonly List<VolatileTransportMetadataFrame> Frames;
        IEnumerable<ITransportMetadataFrame> ITransportMetadata.Frames => this.Frames;

        public VolatileTransportMetadata()
        {
            this.Frames = new List<VolatileTransportMetadataFrame>();
        }
        public VolatileTransportMetadata(List<VolatileTransportMetadataFrame> frames)
        {
            this.Frames = frames;
        }

        public bool Has(string id)
        {
            return this.Frames.Any(m => m.Id == id);
        }
        public byte[] Find(string id)
        {
            this.TryGet(id, out var data);

            return data;
        }
        public byte[] Get(string id)
        {
            var success = this.TryGet(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryGet(string id, out byte[] data)
        {
            var metadata = this.Frames.SingleOrDefault(m => m.Id == id);
            data = metadata?.Data;

            return metadata != null;
        }

        public byte[][] GetAll(string id)
        {
            return this.Frames.Where(m => m.Id == id).Select(m => m.Data).ToArray();
        }

        public byte[] FindFirst(string id)
        {
            this.TryGetFirst(id, out var data);

            return data;
        }
        public byte[] GetFirst(string id)
        {
            var success = this.TryGetFirst(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryGetFirst(string id, out byte[] data)
        {
            var metadata = this.Frames.FirstOrDefault(m => m.Id == id);
            data = metadata?.Data;

            return metadata != null;
        }

        public byte[] FindLast(string id)
        {
            this.TryGetLast(id, out var data);

            return data;
        }
        public byte[] GetLast(string id)
        {
            var success = this.TryGetLast(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryGetLast(string id, out byte[] data)
        {
            var metadata = this.Frames.LastOrDefault(m => m.Id == id);
            data = metadata?.Data;

            return metadata != null;
        }

        public byte[] GetOrAdd(string id, byte[] data)
        {
            if (this.TryGet(id, out var existingData))
                return existingData;

            this.Add(id, data);

            return data;
        }
        public byte[] GetOrAdd(string id, Func<string, byte[]> dataFactory)
        {
            if (this.TryGet(id, out var existingData))
                return existingData;

            var data = dataFactory(id);

            this.Add(id, data);

            return data;
        }

        public void Add(string id, byte[] data)
        {
            this.Frames.Add(new VolatileTransportMetadataFrame(id, data));
        }
        public void AddOrSet(string id, byte[] data)
        {
            var frame = this.Frames.SingleOrDefault(m => m.Id == id);
            if (frame != null)
            {
                frame.Data = data;
            }
            else
            {
                this.Frames.Add(new VolatileTransportMetadataFrame(id, data));
            }
        }

        public byte[] Pluck(string id)
        {
            var success = this.TryPluck(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryPluck(string id, out byte[] data)
        {
            var metadata = this.Frames.SingleOrDefault(m => m.Id == id);
            data = metadata?.Data;

            if (metadata != null)
                this.Frames.Remove(metadata);

            return metadata != null;
        }

        public byte[] PluckFirst(string id)
        {
            var success = this.TryPluckFirst(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryPluckFirst(string id, out byte[] data)
        {
            var metadata = this.Frames.FirstOrDefault(m => m.Id == id);
            data = metadata?.Data;

            if (metadata != null)
                this.Frames.Remove(metadata);

            return metadata != null;
        }

        public byte[] PluckLast(string id)
        {
            var success = this.TryPluckLast(id, out var data);
            if (!success)
                throw new Exception($"Metadata ${id} unavailable");

            return data;
        }
        public bool TryPluckLast(string id, out byte[] data)
        {
            var metadata = this.Frames.LastOrDefault(m => m.Id == id);
            data = metadata?.Data;

            if (metadata != null)
                this.Frames.Remove(metadata);

            return metadata != null;
        }
    }

    public class TransportMetadataFrame : ITransportMetadataFrame
    {
        public static TransportMetadataFrame FromMetadataFrame(ITransportMetadataFrame source)
        {
            return new TransportMetadataFrame(source.Id, source.Data);
        }

        public readonly string Id;
        string ITransportMetadataFrame.Id => this.Id;

        public readonly byte[] Data;
        byte[] ITransportMetadataFrame.Data => this.Data;

        public TransportMetadataFrame(string id, byte[] data)
        {
            this.Id = id;
            this.Data = data;
        }
    }
    public class VolatileTransportMetadataFrame : ITransportMetadataFrame
    {
        public static VolatileTransportMetadataFrame FromMetadataFrame(ITransportMetadataFrame source)
        {
            return new VolatileTransportMetadataFrame(source.Id, source.Data);
        }

        public string Id;
        string ITransportMetadataFrame.Id => this.Id;

        public byte[] Data;
        byte[] ITransportMetadataFrame.Data => this.Data;

        public VolatileTransportMetadataFrame()
        {
        }
        public VolatileTransportMetadataFrame(string id, byte[] data)
        {
            this.Id = id;
            this.Data = data;
        }
    }

    public class TransportMessage
    {
        public static TransportMessage FromMessage(TransportMessage source)
        {
            return new TransportMessage(source.Payload, VolatileTransportMetadata.FromMetadata(source.Metadata));
        }

        public readonly byte[] Payload;

        public readonly VolatileTransportMetadata Metadata;

        public TransportMessage(byte[] payload)
        {
            this.Payload = payload;
            this.Metadata = new VolatileTransportMetadata();
        }
        public TransportMessage(byte[] payload, VolatileTransportMetadata metadata)
        {
            this.Payload = payload;
            this.Metadata = metadata;
        }

        public void WriteDebug(string label = "")
        {
            Console.WriteLine(
                $"{(string.IsNullOrEmpty(label) ? "Message" : label)}" +
                (this.Metadata.Frames.Count > 0 ? $"\n  Metadata:" : "") +
                (this.Metadata.Frames.Count > 0 ? $"\n{string.Join("\n", this.Metadata.Frames.Select(frame => $"    {frame.Id}: [ " + frame.Data.WriteHex(20) + " ] " + frame.Data.WriteDataSize()))}" : "") +
                $"\n  Payload:" +
                $"\n" + "    [ " + this.Payload.WriteHex(20) + " ] " + this.Payload.WriteDataSize());
        }
    }

    public class TaggedTransportMessage
    {
        public readonly string Id;
        public readonly TransportMessage Message;

        public TaggedTransportMessage(string id, TransportMessage message)
        {
            this.Id = id;
            this.Message = message;
        }
    }

    //public class TransportMessage : ITransportMessage
    //{
    //    public readonly byte[] Payload;
    //    byte[] ITransportMessage.Payload => this.Payload;

    //    public readonly IDictionary<string, byte[]> Metadata;
    //    IDictionary<string, byte[]> ITransportMessage.Metadata => this.Metadata;

    //    public TransportMessage(byte[] payload)
    //    {
    //        this.Payload = payload;
    //        this.Metadata = new Dictionary<string, byte[]>();
    //    }
    //    public TransportMessage(byte[] payload, IDictionary<string, byte[]> metadata)
    //    {
    //        this.Payload = payload;
    //        this.Metadata = metadata;
    //    }

    //    public byte[] GetMetadata(string id)
    //    {
    //        return this.Metadata[id];
    //    }
    //}
    //public class TaggedTransportMessage : TransportMessage, ITaggedTransportMessage
    //{
    //    public readonly string MessageId;
    //    string ITaggedTransportMessage.MessageId => this.MessageId;

    //    public TaggedTransportMessage(string messageId, byte[] payload)
    //        : base(payload)
    //    {
    //        this.MessageId = messageId;
    //    }
    //    public TaggedTransportMessage(string messageId, byte[] payload, IDictionary<string, byte[]> metadata)
    //        : base(payload, metadata)
    //    {
    //        this.MessageId = messageId;
    //    }
    //}

    //public struct ReceivedData
    //{
    //    public readonly byte[] Data;
    //    public readonly Dictionary<string, byte[]> Metadata;

    //    public ReceivedData(byte[] data)
    //    {
    //        this.Data = data;
    //        this.Metadata = new Dictionary<string, byte[]>();
    //    }
    //    public ReceivedData(byte[] data, Dictionary<string, byte[]> metadata)
    //    {
    //        this.Data = data;
    //        this.Metadata = new Dictionary<string, byte[]>(metadata);
    //    }
    //}
    //public struct ReceivedTaggedData
    //{
    //    public readonly string MessageId;
    //    public readonly byte[] Data;
    //    public readonly Dictionary<string, byte[]> Metadata;

    //    public ReceivedTaggedData(string messageId, byte[] data)
    //    {
    //        this.MessageId = messageId;
    //        this.Data = data;
    //        this.Metadata = new Dictionary<string, byte[]>();
    //    }
    //    public ReceivedTaggedData(string messageId, byte[] data, Dictionary<string, byte[]> metadata)
    //    {
    //        this.MessageId = messageId;
    //        this.Data = data;
    //        this.Metadata = new Dictionary<string, byte[]>(metadata);
    //    }
    //}
    //public struct ReceiveAndSendData
    //{
    //    public readonly ReceivedData ReceivedData;
    //    public readonly Func<byte[], Dictionary<string, byte[]>, Task> SendHandler;

    //    public ReceiveAndSendData(ReceivedData receivedData, Func<byte[], Dictionary<string, byte[]>, Task> sendHandler)
    //    {
    //        this.ReceivedData = receivedData;
    //        this.SendHandler = sendHandler;
    //    }
    //}

    public class MessagingEventArgs : EventArgs
    {
        public TransportMessage Message { get; private set; }

        public MessagingEventArgs(TransportMessage message)
            : base()
        {
            this.Message = message;
        }
    }

    public class DiagnosticMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public DiagnosticMessageEventArgs(string message)
            : base()
        {
            this.Message = message;
        }
    }

    public interface ITransport
    {
        event EventHandler<MessagingEventArgs> MessageReceived;
        event EventHandler<MessagingEventArgs> MessageSent;

        event EventHandler<MessagingEventArgs> MessageReceiving;
        event EventHandler<MessagingEventArgs> MessageSending;

        string Identity { get; }

        Task Send(TransportMessage message);
        Task Send(string messageId, TransportMessage message);

        Task<TransportMessage> Receive();
        Task<TransportMessage> Receive(string messageId);

        Task<TaggedTransportMessage> ReceiveTagged();

        Task<Func<Task<TransportMessage>>> SendAndReceive(TransportMessage message);
    }

    public interface IServerTransport : ITransport
    {
        //bool IsListening { get; }

        Task Listen();
        Task Close();
    }

    public interface IClientTransport : ITransport
    {
        bool IsConnected { get; }

        Task Connect();
        Task Connect(CancellationToken cancellationToken);

        Task Close();
    }

    public abstract class ATransport : ITransport
    {
        public event EventHandler<MessagingEventArgs> MessageReceived;
        public event EventHandler<MessagingEventArgs> MessageSent;

        public event EventHandler<MessagingEventArgs> MessageReceiving;
        public event EventHandler<MessagingEventArgs> MessageSending;

        public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessage;

        public string Identity { get; private set; }
        public bool IsRunning { get; protected set; }

        public ATransport()
        {
            this.Identity = Guid.NewGuid().ToString().Replace("-", "").ToLowerInvariant();
        }
        public ATransport(string identity)
        {
            this.Identity = identity;
        }

        public abstract Task Send(TransportMessage message);
        public abstract Task Send(string messageId, TransportMessage message);

        public abstract Task<TransportMessage> Receive();
        public abstract Task<TransportMessage> Receive(string messageId);

        public abstract Task<TaggedTransportMessage> ReceiveTagged();

        public abstract Task<Func<Task<TransportMessage>>> SendAndReceive(TransportMessage message);

        protected virtual void OnMessageReceived(TransportMessage message)
        {
            this.MessageReceived?.Invoke(this, new MessagingEventArgs(message));
        }
        protected virtual void OnMessageSent(TransportMessage message)
        {
            this.MessageSent?.Invoke(this, new MessagingEventArgs(message));
        }

        protected virtual void OnMessageReceiving(TransportMessage message)
        {
            this.MessageReceiving?.Invoke(this, new MessagingEventArgs(message));
        }
        protected virtual void OnMessageSending(TransportMessage message)
        {
            this.OnDiagnosticMessage("OnMessageSending - Before " + (this.MessageSending != null ? this.MessageSending.GetInvocationList().Length.ToString() : ""));
            if (this.MessageSending != null)
            {
                foreach (var del in this.MessageSending?.GetInvocationList())
                {
                    this.OnDiagnosticMessage("OnMessageSending Delegate - Before");
                    try
                    {
                        del.DynamicInvoke(this, new MessagingEventArgs(message));
                    }
                    catch
                    {
                        this.OnDiagnosticMessage("ERROR WILL ROBINSON");
                    }
                }
            }
            //this.MessageSending?.Invoke(this, new MessagingEventArgs(message));
            this.OnDiagnosticMessage("OnMessageSending - After");
        }

        protected virtual void OnDiagnosticMessage(string message)
        {
            this.DiagnosticMessage?.Invoke(this, new DiagnosticMessageEventArgs(message));
        }
    }

    public abstract class AServerTransport : ATransport, IServerTransport
    {
        public bool IsListening { get; protected set; }

        public abstract Task Listen();
        public abstract Task Close();
    }
    public abstract class AClientTransport : ATransport, IClientTransport
    {
        public bool IsConnected { get; protected set; }
   
        public abstract Task Connect();
        public abstract Task Connect(CancellationToken cancellationToken);

        public abstract Task Close();
    }

    internal static class Helpers
    {
        public static string WriteHex(this byte[] data, int maxLength = 0)
        {
            string content;
            if (maxLength > 0 && data.Length > maxLength)
                content = BitConverter.ToString(data.Take(maxLength).ToArray()).Replace(" - ", " ") + "...";
            else
                content = BitConverter.ToString(data).Replace(" - ", " ");

            return content;
        }

        public static string WriteDataSize(this byte[] data)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = data.Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##}{1}", len, sizes[order]);

            return result;
        }
    }
}