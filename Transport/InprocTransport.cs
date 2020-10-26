using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axon
{
    //internal static class InprocTransportData
    //{
    //    public static ConcurrentDictionary<string, BlockingCollection<TransportMessage>> ServerBuffers { get; } = new ConcurrentDictionary<string, BlockingCollection<TransportMessage>>();
    //    public static ConcurrentDictionary<string, BlockingCollection<TransportMessage>> ClientBuffers { get; } = new ConcurrentDictionary<string, BlockingCollection<TransportMessage>>();
    //}

    public class MessageStream
    {
        public event EventHandler<MessageEnqueuedEventArgs> MessageEnqueued;

        public void EnqueueMessage(TransportMessage message)
        {
            this.OnMessageEnqueued(message);
        }

        protected virtual void OnMessageEnqueued(TransportMessage message)
        {
            this.MessageEnqueued?.Invoke(this, new MessageEnqueuedEventArgs(message));
        }
    }

    public class MessageEnqueuedEventArgs : EventArgs
    {
        public TransportMessage Message { get; }

        public MessageEnqueuedEventArgs(TransportMessage message)
            : base()
        {
            this.Message = message;
        }
    }
    public class TransportMessageBuffer
    {
        public event EventHandler<MessageEnqueuedEventArgs> MessageEnqueued;

        private static ConcurrentDictionary<string, TransportMessageBuffer> SharedBuffers = new ConcurrentDictionary<string, TransportMessageBuffer>();
        public static TransportMessageBuffer GetSharedBuffer(string identifier)
        {
            return SharedBuffers.GetOrAdd(identifier, (_) => new TransportMessageBuffer(identifier));
        }

        public string Identifier { get; }

        private BlockingCollection<TransportMessage> Buffer { get; }

        public TransportMessageBuffer()
        {
            this.Identifier = Guid.NewGuid().ToString("N").ToLowerInvariant();
            this.Buffer = new BlockingCollection<TransportMessage>();
        }
        public TransportMessageBuffer(string identifier)
        {
            this.Identifier = identifier;

            this.Buffer = new BlockingCollection<TransportMessage>();
        }

        public TransportMessage GetNextMessage()
        {
            return this.Buffer.Take();
        }
        public TransportMessage GetNextMessage(CancellationToken cancellationToken)
        {
            return this.Buffer.Take(cancellationToken);
        }

        public void QueueMessage(TransportMessage message)
        {
            this.Buffer.Add(message);

            this.OnMessageEnqueued(message);
        }
        public void QueueMessage(TransportMessage message, CancellationToken cancellationToken)
        {
            this.Buffer.Add(message, cancellationToken);

            this.OnMessageEnqueued(message);
        }

        protected virtual void OnMessageEnqueued(TransportMessage message)
        {
            this.MessageEnqueued?.Invoke(this, new MessageEnqueuedEventArgs(message));
        }
    }

    public class TaggedMessageEnqueuedEventArgs : EventArgs
    {
        public string Key { get; }
        public TransportMessage Message { get; }

        public TaggedMessageEnqueuedEventArgs(string key, TransportMessage message)
            : base()
        {
            this.Key = key;
            this.Message = message;
        }
    }
    public class TaggedTransportMessageBuffer
    {
        public event EventHandler<TaggedMessageEnqueuedEventArgs> MessageEnqueued;

        private static ConcurrentDictionary<string, TaggedTransportMessageBuffer> SharedBuffers = new ConcurrentDictionary<string, TaggedTransportMessageBuffer>();
        public static TaggedTransportMessageBuffer GetSharedBuffer(string identifier)
        {
            return SharedBuffers.GetOrAdd(identifier, (_) => new TaggedTransportMessageBuffer(identifier));
        }

        public string Identifier { get; }

        private ConcurrentDictionary<string, BlockingCollection<TransportMessage>> Buffer { get; }

        public TaggedTransportMessageBuffer()
        {
            this.Identifier = Guid.NewGuid().ToString("N").ToLowerInvariant();
            this.Buffer = new ConcurrentDictionary<string, BlockingCollection<TransportMessage>>();
        }
        public TaggedTransportMessageBuffer(string identifier)
        {
            this.Identifier = identifier;

            this.Buffer = new ConcurrentDictionary<string, BlockingCollection<TransportMessage>>();
        }

        public TransportMessage GetNextMessage(string key)
        {
            var receiveBuffer = this.Buffer.GetOrAdd(key, (_) => new BlockingCollection<TransportMessage>());

            var data = receiveBuffer.Take();

            if (receiveBuffer.Count < 1)
                this.Buffer.TryRemove(key, out _);

            return data;
        }
        public TransportMessage GetNextMessage(string key, CancellationToken cancellationToken)
        {
            var receiveBuffer = this.Buffer.GetOrAdd(key, (_) => new BlockingCollection<TransportMessage>());

            var data = receiveBuffer.Take(cancellationToken);

            if (receiveBuffer.Count < 1)
                this.Buffer.TryRemove(key, out _);

            return data;
        }

        public void QueueMessage(string key, TransportMessage message)
        {
            this.Buffer.GetOrAdd(key, (_) => new BlockingCollection<TransportMessage>()).Add(message);

            this.OnMessageEnqueued(key, message);
        }
        public void QueueMessage(string key, TransportMessage message, CancellationToken cancellationToken)
        {
            this.Buffer.GetOrAdd(key, (_) => new BlockingCollection<TransportMessage>()).Add(message, cancellationToken);

            this.OnMessageEnqueued(key, message);
        }

        protected virtual void OnMessageEnqueued(string key, TransportMessage message)
        {
            this.MessageEnqueued?.Invoke(this, new TaggedMessageEnqueuedEventArgs(key, message));
        }
    }

    public class ServerTransportCreatedEventArgs : EventArgs
    {
        public string Identifier { get; }
        public InprocServerTransport ServerTransport { get; }

        public ServerTransportCreatedEventArgs(string identifier, InprocServerTransport serverTransport)
            : base()
        {
            this.Identifier = identifier;
            this.ServerTransport = serverTransport;
        }
    }
    public class ClientTransportCreatedEventArgs : EventArgs
    {
        public string Identifier { get; }
        public InprocClientTransport ClientTransport { get; }

        public ClientTransportCreatedEventArgs(string identifier, InprocClientTransport clientTransport)
            : base()
        {
            this.Identifier = identifier;
            this.ClientTransport = clientTransport;
        }
    }
    //public class InprocTransportScope
    //{
    //    public event EventHandler<ServerTransportCreatedEventArgs> ServerTransportCreated;
    //    public event EventHandler<ServerTransportCreatedEventArgs> ServerTransportRemoved;
    //    public event EventHandler<ClientTransportCreatedEventArgs> ClientTransportCreated;
    //    public event EventHandler<ClientTransportCreatedEventArgs> ClientTransportRemoved;

    //    public InprocServerTransport ServerTransport { get; } = new InprocServerTransport();

    //    //private ConcurrentDictionary<string, TransportMessageBuffer> SharedBuffers { get; } = new ConcurrentDictionary<string, TransportMessageBuffer>();

    //    private ConcurrentDictionary<string, ConcurrentQueue<InprocServerTransport>> ServerTransports { get; } = new ConcurrentDictionary<string, ConcurrentQueue<InprocServerTransport>>();
    //    //private ConcurrentDictionary<string, ConcurrentQueue<InprocClientTransport>> ClientTransports { get; } = new ConcurrentDictionary<string, ConcurrentQueue<InprocClientTransport>>();

    //    public InprocServerTransport CreateServerTransport(string identifier)
    //    {
    //        //var instanceIdentifier = Guid.NewGuid().ToString();

    //        //var receiveBuffer = this.GetSharedBuffer($"{identifier}.{instanceIdentifier}.from-service");
    //        //var sendBuffer = this.GetSharedBuffer($"{identifier}.{instanceIdentifier}.to-service");
    //        //var taggedReceiveBuffer = TaggedTransportMessageBuffer.GetSharedBuffer($"{identifier}.receive");
    //        //var taggedSendBuffer = TaggedTransportMessageBuffer.GetSharedBuffer($"{identifier}.send");

    //        var serverTransport = new InprocServerTransport();

    //        serverTransport.Listening += (sender, e) =>
    //        {
    //            Console.WriteLine($"Inproc server listening [{identifier}/{serverTransport.Identity}]");

    //            this.ServerTransports.GetOrAdd(identifier, (_) => new ConcurrentQueue<InprocServerTransport>()).Enqueue(serverTransport);
    //            this.OnServerTransportCreated(identifier, serverTransport);
    //        };
    //        serverTransport.Closed += (sender, e) =>
    //        {
    //            Console.WriteLine($"Inproc server closed [{identifier}/{serverTransport.Identity}]");
    //        };

    //        return serverTransport;
    //    }
    //    public InprocClientTransport CreateClientTransport(string identifier)
    //    {
    //        var clientTransport = this.ServerTransport.CreateClient();

    //        this.OnClientTransportCreated(identifier, clientTransport);
    //        Console.WriteLine($"Inproc client connected [{identifier}/{clientTransport.Identity}]");

    //        return clientTransport;
    //    }

    //    //private TransportMessageBuffer GetSharedBuffer(string identifier)
    //    //{
    //    //    return this.SharedBuffers.GetOrAdd(identifier, (_) => new TransportMessageBuffer(identifier));
    //    //}

    //    protected virtual void OnServerTransportCreated(string identifier, InprocServerTransport serverTransport)
    //    {
    //        this.ServerTransportCreated?.Invoke(this, new ServerTransportCreatedEventArgs(identifier, serverTransport));
    //    }
    //    protected virtual void OnServerTransportRemoved(string identifier, InprocServerTransport serverTransport)
    //    {
    //        this.ServerTransportRemoved?.Invoke(this, new ServerTransportCreatedEventArgs(identifier, serverTransport));
    //    }
    //    protected virtual void OnClientTransportCreated(string identifier, InprocClientTransport clientTransport)
    //    {
    //        this.ClientTransportCreated?.Invoke(this, new ClientTransportCreatedEventArgs(identifier, clientTransport));
    //    }
    //}

    public class InprocServerTransport : AServerTransport
    {
        //public static InprocServerTransport CreateSharedTransport(string identifier)
        //{
        //    var receiveBuffer = TransportMessageBuffer.GetSharedBuffer($"{identifier}.from-service");
        //    var sendBuffer = TransportMessageBuffer.GetSharedBuffer($"{identifier}.to-service");
        //    //var taggedReceiveBuffer = TaggedTransportMessageBuffer.GetSharedBuffer($"{identifier}.receive");
        //    //var taggedSendBuffer = TaggedTransportMessageBuffer.GetSharedBuffer($"{identifier}.send");

        //    return new InprocServerTransport(receiveBuffer, sendBuffer);
        //}

        public event EventHandler Listening;
        public event EventHandler Closed;

        private bool isListening = false;
        public override bool IsListening
        {
            get => this.isListening;
        } 

        public MessageStream ReceiveStream { get; } = new MessageStream();
        private TransportMessageBuffer MessageBuffer { get; } = new TransportMessageBuffer();

        private ConcurrentDictionary<string, InprocClientTransport> RegisteredClients { get; } = new ConcurrentDictionary<string, InprocClientTransport>();

        //public bool IsListening { get; private set; }

        public InprocServerTransport()
            : base()
        {
            this.ReceiveStream.MessageEnqueued += this.ReceiveBufferMessageEnqueued;
        }

        public override async Task Listen()
        {
            this.isListening = true;
            this.OnListening();
        }

        public override async Task Close()
        {
            this.isListening = false;
            this.OnClosed();
        }
        public override async Task Close(CancellationToken cancellationToken)
        {
            this.isListening = false;
            this.OnClosed();
        }

        public override async Task<TransportMessage> Receive()
        {
            var message = this.MessageBuffer.GetNextMessage();

            this.OnMessageReceived(message);

            return message;
        }
        public override async Task<TransportMessage> Receive(CancellationToken cancellationToken)
        {
            var message = this.MessageBuffer.GetNextMessage(cancellationToken);

            this.OnMessageReceived(message);

            return message;
        }
        public override Task<TransportMessage> Receive(string messageId)
        {
            throw new NotImplementedException();
        }
        public override Task<TransportMessage> Receive(string messageId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<TaggedTransportMessage> ReceiveTagged()
        {
            throw new NotImplementedException();
        }
        public override Task<TaggedTransportMessage> ReceiveTagged(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task Send(TransportMessage message)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);
            
            if (!forwardedMessage.Metadata.TryPluckLast($"clientIdentity[{this.Identity}]", out var encodedClientIdentity))
                throw new Exception("Client identity not found in message");
            var clientIdentity = Encoding.UTF8.GetString(encodedClientIdentity);

            if (!this.RegisteredClients.TryGetValue(clientIdentity, out var inprocClient))
                throw new Exception($"Registered client not found [{clientIdentity}]");

            inprocClient.ReceiveStream.EnqueueMessage(forwardedMessage);

            //if (forwardedMessage.Metadata.TryPluckLast($"tag[{this.Identity}]", out var encodedTag))
            //{
            //    var tag = Encoding.UTF8.GetString(encodedTag);

            //    inprocClient.TaggedReceiveBuffer.QueueMessage(tag, forwardedMessage);
            //}
            //else
            //{
            //    inprocClient.ReceiveBuffer.QueueMessage(forwardedMessage);
            //}

            this.OnMessageSent(forwardedMessage);
        }
        public override async Task Send(TransportMessage message, CancellationToken cancellationToken)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);

            if (!forwardedMessage.Metadata.TryPluckLast($"clientIdentity[{this.Identity}]", out var encodedClientIdentity))
                throw new Exception("Client identity not found in message");
            var clientIdentity = Encoding.UTF8.GetString(encodedClientIdentity);

            if (!this.RegisteredClients.TryGetValue(clientIdentity, out var inprocClient))
                throw new Exception($"Registered client not found [{clientIdentity}]");

            inprocClient.ReceiveStream.EnqueueMessage(forwardedMessage);

            this.OnMessageSent(forwardedMessage);
        }
        public override async Task Send(string messageId, TransportMessage message)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);

            if (!forwardedMessage.Metadata.TryPluckLast($"clientIdentity[{this.Identity}]", out var encodedClientIdentity))
                throw new Exception("Client identity not found in message");
            var clientIdentity = Encoding.UTF8.GetString(encodedClientIdentity);

            if (!this.RegisteredClients.TryGetValue(clientIdentity, out var inprocClient))
                throw new Exception($"Registered client not found [{clientIdentity}]");

            forwardedMessage.Metadata.Add($"mid[{inprocClient.Identity}]", Encoding.UTF8.GetBytes(messageId));

            inprocClient.ReceiveStream.EnqueueMessage(forwardedMessage);

            this.OnMessageSent(forwardedMessage);
        }
        public override async Task Send(string messageId, TransportMessage message, CancellationToken cancellationToken)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);

            if (!forwardedMessage.Metadata.TryPluckLast($"clientIdentity[{this.Identity}]", out var encodedClientIdentity))
                throw new Exception("Client identity not found in message");
            var clientIdentity = Encoding.UTF8.GetString(encodedClientIdentity);

            if (!this.RegisteredClients.TryGetValue(clientIdentity, out var inprocClient))
                throw new Exception($"Registered client not found [{clientIdentity}]");

            forwardedMessage.Metadata.Add($"mid[{inprocClient.Identity}]", Encoding.UTF8.GetBytes(messageId));

            inprocClient.ReceiveStream.EnqueueMessage(forwardedMessage);

            this.OnMessageSent(forwardedMessage);
        }

        public override Task<Func<Task<TransportMessage>>> SendAndReceive(TransportMessage message)
        {
            throw new NotImplementedException();
        }
        public override Task<Func<Task<TransportMessage>>> SendAndReceive(TransportMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public InprocClientTransport CreateClient()
        {
            var clientTransport = new InprocClientTransport(this);
            if (!this.RegisteredClients.TryAdd(clientTransport.Identity, clientTransport))
                throw new Exception($"Could not register new client [{clientTransport.Identity}]");

            return clientTransport;
        }

        protected virtual void OnListening()
        {
            this.Listening?.Invoke(this, new EventArgs());
        }
        protected virtual void OnClosed()
        {
            this.Closed?.Invoke(this, new EventArgs());
        }

        private void ReceiveBufferMessageEnqueued(object sender, MessageEnqueuedEventArgs e)
        {
            this.OnMessageReceiving(e.Message);

            var forwardedMessage = TransportMessage.FromMessage(e.Message);
            this.MessageBuffer.QueueMessage(forwardedMessage);
        }
    }

    public class InprocClientTransport : AClientTransport
    {
        public override bool IsConnected
        {
            get => this.ServerTransport.IsListening;
        }

        public InprocServerTransport ServerTransport { get; }

        public MessageStream ReceiveStream { get; } = new MessageStream();
        private TransportMessageBuffer MessageBuffer { get; } = new TransportMessageBuffer();
        private TaggedTransportMessageBuffer TaggedMessageBuffer { get; } = new TaggedTransportMessageBuffer();
        
        internal InprocClientTransport(InprocServerTransport serverTransport)
            : base()
        {
            this.ServerTransport = serverTransport;

            this.ReceiveStream.MessageEnqueued += this.ReceiveBufferMessageEnqueued;
            //this.TaggedReceiveBuffer.MessageEnqueued += this.TaggedReceiveBufferMessageEnqueued;

            //this.ReceiveBuffer = new TransportMessageBuffer();
            //this.SendBuffer = new TransportMessageBuffer();
            //this.TaggedReceiveBuffer = new TaggedTransportMessageBuffer();
            //this.TaggedSendBuffer = new TaggedTransportMessageBuffer();
        }

        public override async Task Connect()
        {
        }
        public override async Task Connect(CancellationToken cancellationToken)
        {
        }

        public override async Task Close()
        {
        }
        public override async Task Close(CancellationToken cancellationToken)
        {
        }

        public override async Task Send(TransportMessage message)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);
            forwardedMessage.Metadata.Add($"clientIdentity[{this.ServerTransport.Identity}]", Encoding.UTF8.GetBytes(this.Identity));

            this.ServerTransport.ReceiveStream.EnqueueMessage(forwardedMessage);

            this.OnMessageSent(forwardedMessage);
        }
        public override async Task Send(TransportMessage message, CancellationToken cancellationToken)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);
            forwardedMessage.Metadata.Add($"clientIdentity[{this.ServerTransport.Identity}]", Encoding.UTF8.GetBytes(this.Identity));

            this.ServerTransport.ReceiveStream.EnqueueMessage(forwardedMessage);

            this.OnMessageSent(forwardedMessage);
        }
        public override async Task Send(string messageId, TransportMessage message)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);
            forwardedMessage.Metadata.Add($"clientIdentity[{this.ServerTransport.Identity}]", Encoding.UTF8.GetBytes(this.Identity));
            forwardedMessage.Metadata.Add($"mid[{this.Identity}]", Encoding.UTF8.GetBytes(messageId));

            this.ServerTransport.ReceiveStream.EnqueueMessage(forwardedMessage);

            this.OnMessageSent(forwardedMessage);
        }
        public override async Task Send(string messageId, TransportMessage message, CancellationToken cancellationToken)
        {
            this.OnMessageSending(message);

            var forwardedMessage = TransportMessage.FromMessage(message);
            forwardedMessage.Metadata.Add($"clientIdentity[{this.ServerTransport.Identity}]", Encoding.UTF8.GetBytes(this.Identity));
            forwardedMessage.Metadata.Add($"mid[{this.Identity}]", Encoding.UTF8.GetBytes(messageId));

            this.ServerTransport.ReceiveStream.EnqueueMessage(forwardedMessage);

            this.OnMessageSent(forwardedMessage);
        }

        public override async Task<TransportMessage> Receive()
        {
            var message = this.MessageBuffer.GetNextMessage();

            this.OnMessageReceived(message);

            return message;
        }
        public override async Task<TransportMessage> Receive(CancellationToken cancellationToken)
        {
            var message = this.MessageBuffer.GetNextMessage(cancellationToken);

            this.OnMessageReceived(message);

            return message;
        }
        public override async Task<TransportMessage> Receive(string messageId)
        {
            var message = this.TaggedMessageBuffer.GetNextMessage(messageId);

            this.OnMessageReceived(message);

            return message;
        }
        public override async Task<TransportMessage> Receive(string messageId, CancellationToken cancellationToken)
        {
            var message = this.TaggedMessageBuffer.GetNextMessage(messageId, cancellationToken);

            this.OnMessageReceived(message);

            return message;
        }

        public override Task<TaggedTransportMessage> ReceiveTagged()
        {
            throw new NotImplementedException();
        }
        public override Task<TaggedTransportMessage> ReceiveTagged(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<Func<Task<TransportMessage>>> SendAndReceive(TransportMessage message)
        {
            throw new NotImplementedException();
        }
        public override Task<Func<Task<TransportMessage>>> SendAndReceive(TransportMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void ReceiveBufferMessageEnqueued(object sender, MessageEnqueuedEventArgs e)
        {
            this.OnMessageReceiving(e.Message);

            var forwardedMessage = TransportMessage.FromMessage(e.Message);
            if (forwardedMessage.Metadata.TryPluck($"mid[{this.Identity}]", out var encodedMessageId))
            {
                var messageId = Encoding.UTF8.GetString(encodedMessageId);
                this.TaggedMessageBuffer.QueueMessage(messageId, forwardedMessage);
            }
            else
            {
                this.MessageBuffer.QueueMessage(forwardedMessage);
            }
        }
    }

    internal static class MessageHelpers
    {
        public static void Write(this TransportMessage message, BinaryWriter writer)
        {
            writer.Write(0);

            writer.Write(message.Metadata.Frames.Count);

            foreach (var frame in message.Metadata.Frames)
            {
                var encodedId = Encoding.UTF8.GetBytes(frame.Id);
                writer.Write(encodedId.Length);
                writer.Write(encodedId);

                writer.Write(frame.Data.Length);
                writer.Write(frame.Data);
            }

            writer.Write(message.Payload.Length);
            writer.Write(message.Payload);
        }

        public static TransportMessage ReadTransportMessage(this BinaryReader reader)
        {
            var metadata = new VolatileTransportMetadata();

            var signal = reader.ReadInt32();
            if (signal != 0)
                throw new Exception("Message received with signal code " + signal.ToString());

            var frameCount = reader.ReadInt32();
            for (var a = 0; a < frameCount; a++)
            {
                var idLength = reader.ReadInt32();
                var encodedId = reader.ReadBytes(idLength);

                var dataLength = reader.ReadInt32();
                var data = reader.ReadBytes(dataLength);

                metadata.Frames.Add(new VolatileTransportMetadataFrame(Encoding.UTF8.GetString(encodedId), data));
            }

            var payloadLength = reader.ReadInt32();
            var payload = reader.ReadBytes(payloadLength);

            return new TransportMessage(payload, metadata);
        }
    }
}
