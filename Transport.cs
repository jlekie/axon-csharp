using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axon
{
    public struct ReceivedData
    {
        public readonly byte[] Data;
        public readonly Dictionary<string, byte[]> Metadata;

        public ReceivedData(byte[] data)
        {
            this.Data = data;
            this.Metadata = new Dictionary<string, byte[]>();
        }
        public ReceivedData(byte[] data, Dictionary<string, byte[]> metadata)
        {
            this.Data = data;
            this.Metadata = new Dictionary<string, byte[]>(metadata);
        }
    }

    public struct ReceivedTaggedData
    {
        public readonly string MessageId;
        public readonly byte[] Data;
        public readonly Dictionary<string, byte[]> Metadata;

        public ReceivedTaggedData(string messageId, byte[] data)
        {
            this.MessageId = messageId;
            this.Data = data;
            this.Metadata = new Dictionary<string, byte[]>();
        }
        public ReceivedTaggedData(string messageId, byte[] data, Dictionary<string, byte[]> metadata)
        {
            this.MessageId = messageId;
            this.Data = data;
            this.Metadata = new Dictionary<string, byte[]>(metadata);
        }
    }

    public struct ReceiveAndSendData
    {
        public readonly ReceivedData ReceivedData;
        public readonly Func<byte[], Dictionary<string, byte[]>, Task> SendHandler;

        public ReceiveAndSendData(ReceivedData receivedData, Func<byte[], Dictionary<string, byte[]>, Task> sendHandler)
        {
            this.ReceivedData = receivedData;
            this.SendHandler = sendHandler;
        }
    }

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

    public interface ITransport
    {
        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<DataSentEventArgs> DataSent;

        Task Send(byte[] data, IDictionary<string, byte[]> metadata);
        Task Send(string messageId, byte[] data, IDictionary<string, byte[]> metadata);

        Task<ReceivedData> Receive();
        Task<ReceivedData> Receive(string messageId);

        Task<ReceivedTaggedData> ReceiveTagged();

        //Task<>

        Task<Func<Task<ReceivedData>>> SendAndReceive(byte[] data, IDictionary<string, byte[]> metadata);
        Task<ReceiveAndSendData> ReceiveAndSend();
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

        public bool IsRunning { get; protected set; }

        public abstract Task Send(byte[] data, IDictionary<string, byte[]> metadata);
        public virtual Task Send(string messageId, byte[] data, IDictionary<string, byte[]> metadata)
        {
            throw new NotImplementedException();
        }

        public abstract Task<ReceivedData> Receive();
        public virtual Task<ReceivedData> Receive(string messageId)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ReceivedTaggedData> ReceiveTagged()
        {
            throw new NotImplementedException();
        }

        public abstract Task<Func<Task<ReceivedData>>> SendAndReceive(byte[] data, IDictionary<string, byte[]> metadata);
        public Task<ReceiveAndSendData> ReceiveAndSend()
        {
            throw new NotImplementedException();
        }

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