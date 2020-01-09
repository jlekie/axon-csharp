using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axon
{
    public interface ITransportMetadata
    {
        IEnumerable<ITransportMetadataFrame> Frames { get; }

        byte[] Find(string id);
        byte[] Get(string id);
        bool TryGet(string id, out byte[] data);
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
            var metadata = this.Frames.FirstOrDefault(m => m.Id == id);
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
            var metadata = this.Frames.FirstOrDefault(m => m.Id == id);
            data = metadata?.Data;

            return metadata != null;
        }

        public void Add(string id, byte[] data)
        {
            this.Frames.Add(new VolatileTransportMetadataFrame(id, data));
        }
        public void AddOrSet(string id, byte[] data)
        {
            var frame = this.Frames.FirstOrDefault(m => m.Id == id);
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
            var metadata = this.Frames.FirstOrDefault(m => m.Id == id);
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

        public void WriteDebug()
        {
            foreach (var frame in this.Metadata.Frames)
                Console.WriteLine("  " + frame.Id + " [ " + BitConverter.ToString(frame.Data).Replace("-", " ") + " ]");
            Console.WriteLine("  Payload" + " [ " + BitConverter.ToString(this.Payload).Replace("-", " ") + " ]");
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

    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }
        public Dictionary<string, byte[]> Metadata { get; private set; }

        public DataReceivedEventArgs(byte[] data, IDictionary<string, byte[]> metadata)
            : base()
        {
            this.Data = data;
            this.Metadata = new Dictionary<string, byte[]>(metadata);
        }
    }
    public class DataSentEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }
        public Dictionary<string, byte[]> Metadata { get; private set; }

        public DataSentEventArgs(byte[] data, IDictionary<string, byte[]> metadata)
            : base()
        {
            this.Data = data;
            this.Metadata = new Dictionary<string, byte[]>(metadata);
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public TransportMessage Message { get; private set; }

        public MessageReceivedEventArgs(TransportMessage message)
            : base()
        {
            this.Message = message;
        }
    }
    public class MessageSentEventArgs : EventArgs
    {
        public TransportMessage Message { get; private set; }

        public MessageSentEventArgs(TransportMessage message)
            : base()
        {
            this.Message = message;
        }
    }

    public interface ITransport
    {
        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<DataSentEventArgs> DataSent;

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<MessageSentEventArgs> MessageSent;

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

        Task Connect(int timeout = 0);
        Task Close();
    }

    public abstract class ATransport : ITransport
    {
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<DataSentEventArgs> DataSent;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageSentEventArgs> MessageSent;

        public bool IsRunning { get; protected set; }

        public abstract Task Send(TransportMessage message);
        public abstract Task Send(string messageId, TransportMessage message);

        public abstract Task<TransportMessage> Receive();
        public abstract Task<TransportMessage> Receive(string messageId);

        public abstract Task<TaggedTransportMessage> ReceiveTagged();

        public abstract Task<Func<Task<TransportMessage>>> SendAndReceive(TransportMessage message);

        protected virtual void OnDataReceived(byte[] data, IDictionary<string, byte[]> metadata)
        {
            if (this.DataReceived != null)
                this.DataReceived(this, new DataReceivedEventArgs(data, metadata));
        }
        protected virtual void OnDataSent(byte[] data, IDictionary<string, byte[]> metadata)
        {
            if (this.DataSent != null)
                this.DataSent(this, new DataSentEventArgs(data, metadata));
        }

        protected virtual void OnMessageReceived(TransportMessage message)
        {
            this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }
        protected virtual void OnMessageSent(TransportMessage message)
        {
            this.MessageSent?.Invoke(this, new MessageSentEventArgs(message));
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
   
        public abstract Task Connect(int timeout = 0);
        public abstract Task Close();
    }
}