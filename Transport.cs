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
    public interface IVolatileTransportMetadata : ITransportMetadata
    {
        void Add(string id, byte[] data);
        void AddOrSet(string id, byte[] data);

        byte[] Pluck(string id);
        bool TryPluck(string id, out byte[] data);
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
            return new TransportMetadata(source.Frames.ToArray());
        }

        public readonly ITransportMetadataFrame[] Frames;
        IEnumerable<ITransportMetadataFrame> ITransportMetadata.Frames => this.Frames;

        public TransportMetadata()
        {
            this.Frames = new ITransportMetadataFrame[] { };
        }
        public TransportMetadata(ITransportMetadataFrame[] frames)
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
    public class VolatileTransportMetadata : IVolatileTransportMetadata
    {
        public static VolatileTransportMetadata FromMetadata(ITransportMetadata source)
        {
            return new VolatileTransportMetadata(source.Frames.ToList());
        }

        public readonly List<ITransportMetadataFrame> Frames;
        IEnumerable<ITransportMetadataFrame> ITransportMetadata.Frames => this.Frames;

        public VolatileTransportMetadata()
        {
            this.Frames = new List<ITransportMetadataFrame>();
        }
        public VolatileTransportMetadata(List<ITransportMetadataFrame> frames)
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
            this.Frames.Add(new TransportMetadataFrame(id, data));
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
                this.Frames.Add(new TransportMetadataFrame(id, data));
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
            data = metadata.Data;

            if (metadata != null)
                this.Frames.Remove(metadata);

            return metadata != null;
        }
    }

    public class TransportMetadataFrame : ITransportMetadataFrame
    {
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

    public interface ITransportMessage
    {
        byte[] Payload { get; }
        IVolatileTransportMetadata Metadata { get; }
    }

    public class TransportMessage : ITransportMessage
    {
        public static TransportMessage FromMessage(ITransportMessage source)
        {
            return new TransportMessage(source.Payload, VolatileTransportMetadata.FromMetadata(source.Metadata));
        }

        public readonly byte[] Payload;
        byte[] ITransportMessage.Payload => this.Payload;

        public readonly VolatileTransportMetadata Metadata;
        IVolatileTransportMetadata ITransportMessage.Metadata => this.Metadata;

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
    }

    public class TaggedTransportMessage
    {
        public readonly string Id;
        public readonly ITransportMessage Message;

        public TaggedTransportMessage(string id, ITransportMessage message)
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
        public ITransportMessage Message { get; private set; }

        public MessageReceivedEventArgs(ITransportMessage message)
            : base()
        {
            this.Message = message;
        }
    }
    public class MessageSentEventArgs : EventArgs
    {
        public ITransportMessage Message { get; private set; }

        public MessageSentEventArgs(ITransportMessage message)
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

        Task Send(ITransportMessage message);
        Task Send(string messageId, ITransportMessage message);

        Task<ITransportMessage> Receive();
        Task<ITransportMessage> Receive(string messageId);

        Task<TaggedTransportMessage> ReceiveTagged();

        Task<Func<Task<ITransportMessage>>> SendAndReceive(ITransportMessage message);
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

        public abstract Task Send(ITransportMessage message);
        public abstract Task Send(string messageId, ITransportMessage message);

        public abstract Task<ITransportMessage> Receive();
        public abstract Task<ITransportMessage> Receive(string messageId);

        public abstract Task<TaggedTransportMessage> ReceiveTagged();

        public abstract Task<Func<Task<ITransportMessage>>> SendAndReceive(ITransportMessage message);

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

        protected virtual void OnMessageReceived(ITransportMessage message)
        {
            this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }
        protected virtual void OnMessageSent(ITransportMessage message)
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