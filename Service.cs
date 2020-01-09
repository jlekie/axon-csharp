using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Axon
{
    [ComVisible(false)]
    public interface IServiceClient
    {
        IClientTransport Transport { get; }
        IProtocol Protocol { get; }

        bool IsConnected { get; }

        Task Connect(int timeout = 0);
        Task Close();
    }
    [ComVisible(false)]
    public abstract class AServiceClient : IServiceClient
    {
        private readonly IClientTransport transport;
        public IClientTransport Transport
        {
            get
            {
                return this.transport;
            }
        }

        private readonly IProtocol protocol;
        public IProtocol Protocol
        {
            get
            {
                return this.protocol;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.Transport.IsConnected;
            }
        }

        public AServiceClient(IClientTransport transport, IProtocol protocol)
        {
            this.transport = transport;
            this.protocol = protocol;
        }

        public async Task Connect(int timeout = 0)
        {
            await this.Transport.Connect(timeout);
        }
        public async Task Close()
        {
            await this.Transport.Close();
        }
    }

    [ComVisible(false)]
    public interface IServiceServer
    {
        IServerTransport Transport { get; }
        IProtocol Protocol { get; }

        Task Start();
        Task Run();
        Task Close();
    }
    [ComVisible(false)]
    public abstract class AServiceServer : IServiceServer
    {
        private bool IsRunning;
        private Task RunningTask;

        private readonly IServerTransport transport;
        public IServerTransport Transport {
            get
            {
                return transport;
            }
        }

        private readonly IProtocol protocol;
        public IProtocol Protocol {
            get
            {
                return protocol;
            }
        }

        private readonly IAnnouncer announcer;
        public IAnnouncer Announcer {
            get
            {
                return announcer;
            }
        }

        private readonly IEncodableEndpoint endpoint;
        public IEncodableEndpoint Endpoint {
            get
            {
                return endpoint;
            }
        }

        public AServiceServer(IServerTransport transport, IProtocol protocol)
        {
            this.transport = transport;
            this.protocol = protocol;
        }
        public AServiceServer(IServerTransport transport, IProtocol protocol, IAnnouncer announcer, IEncodableEndpoint endpoint)
        {
            this.transport = transport;
            this.protocol = protocol;
            this.announcer = announcer;
            this.endpoint = endpoint;
        }

        public async Task Start()
        {
            await this.Transport.Listen();

            this.IsRunning = true;
            //this.RunningTask = Task.Run(() => this.ServerHandler());
            this.RunningTask = Task.Factory.StartNew(() => this.ServerHandler(), TaskCreationOptions.LongRunning).Unwrap();

            if (this.Announcer != null)
            {
                Console.WriteLine("Registering endpoint");
                await this.Announcer.Register(this.Endpoint);

                //var tmp = Task.Run(() => this.AnnouncerHandler());
                var tmp = Task.Factory.StartNew(() => this.AnnouncerHandler(), TaskCreationOptions.LongRunning).Unwrap();
            }

            // return Task.FromResult(true);
        }
        public async Task Run()
        {
            await this.Start();

            await this.RunningTask;
        }
        public async Task Close()
        {
            this.IsRunning = false;

            await this.Transport.Close();

            await this.RunningTask;
        }
        
        protected abstract Task HandleRequest();

        private async Task ServerHandler()
        {
            // await this.AnnouncerHandler();

            while (this.IsRunning)
            {
                try
                {
                    await this.HandleRequest();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
        }
        private async Task AnnouncerHandler()
        {
            // await this.Announcer.Register(this.Transport.Endpoint);
            while (this.IsRunning)
            {
                await Task.Delay(10000);

                try
                {
                    //Console.WriteLine("Registering endpoint");
                    await this.Announcer.Register(this.Endpoint);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
        }
    }

    [ComVisible(false)]
    public struct HandledRequestMessage
    {
        public readonly string ActionName;
        public readonly bool Success;
        public readonly ITransportMetadata Metadata;
        public readonly ReadOnlyDictionary<string, object> Arguments;
        public readonly object Result;
        public readonly Exception Exception;
        //public readonly string MessageId;

        public HandledRequestMessage(string actionName, ITransportMetadata metadata, IDictionary<string, object> arguments, Exception exception)
        {
            this.ActionName = actionName;
            this.Success = false;
            this.Metadata = metadata;
            this.Arguments = new ReadOnlyDictionary<string, object>(arguments);
            this.Result = null;
            this.Exception = exception;
            //this.MessageId = messageId;
        }
        public HandledRequestMessage(string actionName, ITransportMetadata metadata, IDictionary<string, object> arguments, object result)
        {
            this.ActionName = actionName;
            this.Success = true;
            this.Metadata = metadata;
            this.Arguments = new ReadOnlyDictionary<string, object>(arguments);
            this.Result = result;
            this.Exception = null;
            //this.MessageId = messageId;
        }
        public HandledRequestMessage(string actionName, ITransportMetadata metadata, IDictionary<string, object> arguments)
        {
            this.ActionName = actionName;
            this.Success = true;
            this.Metadata = metadata;
            this.Arguments = new ReadOnlyDictionary<string, object>(arguments);
            this.Result = null;
            this.Exception = null;
            //this.MessageId = messageId;
        }
    }
}