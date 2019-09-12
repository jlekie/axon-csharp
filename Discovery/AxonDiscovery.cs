//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace Axon
//{
//    public interface IAxonAnnouncer : IAnnouncer
//    {
//        IAxonDiscoveryClient ServiceClient { get; }
//    }
//    public class AxonAnnouncer : AAnnouncer, IAxonAnnouncer
//    {
//        private readonly IAxonDiscoveryClient serviceClient;
//        public IAxonDiscoveryClient ServiceClient
//        {
//            get
//            {
//                return this.serviceClient;
//            }
//        }

//        public AxonAnnouncer(string identifier, IAxonDiscoveryClient serviceClient)
//            : base(identifier)
//        {
//            this.serviceClient = serviceClient;
//        }

//        public override async Task Register(IEncodableEndpoint endpoint)
//        {
//            await this.ServiceClient.RegisterService(this.Identifier, System.Text.Encoding.UTF8.GetString(endpoint.Encode()));
//        }
//    }

//    public interface IAxonDiscoverer<TEndpoint> : IDiscoverer<TEndpoint> where TEndpoint : IEncodableEndpoint
//    {
//        IAxonDiscoveryClient ServiceClient { get; }
//    }
//    public class AxonDiscoverer<TEndpoint> : ADiscoverer<TEndpoint>, IAxonDiscoverer<TEndpoint> where TEndpoint : IEncodableEndpoint
//    {
//        private readonly IAxonDiscoveryClient serviceClient;
//        public IAxonDiscoveryClient ServiceClient
//        {
//            get
//            {
//                return this.serviceClient;
//            }
//        }

//        public AxonDiscoverer(string identifier, IEndpointDecoder<TEndpoint> endpointDecoder, IAxonDiscoveryClient serviceClient)
//            : base(identifier, endpointDecoder)
//        {
//            this.serviceClient = serviceClient;
//        }

//        public override async Task<TEndpoint> Discover(int timeout = 0)
//        {
//            var encodedEndpoint = System.Text.Encoding.UTF8.GetBytes(await this.ServiceClient.ResolveRegisteredService(this.Identifier, timeout));

//            return this.EndpointDecoder.Decode(encodedEndpoint);
//        }

//        public override async Task<TEndpoint[]> DiscoverAll(int timeout = 0)
//        {
//            var encodedEndpoints = (await this.ServiceClient.ResolveRegisteredServices(this.Identifier, timeout)).Select(e => System.Text.Encoding.UTF8.GetBytes(e));
//            var endpoints = encodedEndpoints.Select(e => this.EndpointDecoder.Decode(e));

//            return endpoints.ToArray();
//        }

//        public override Task Blacklist(TEndpoint endpoint)
//        {
//            Console.WriteLine("BLACKLISTING " + System.Text.Encoding.UTF8.GetString(endpoint.Encode()));

//            return Task.FromResult(true);
//        }
//    }





//    public partial interface IAxonDiscoveryClient : Axon.IServiceClient
//    {
//        Task RegisterService(string serviceName, string endpoint);

//        Task<string> ResolveRegisteredService(string serviceName, int timeout);

//        Task<IEnumerable<string>> ResolveRegisteredServices(string serviceName, int timeout);
//    }

//    public partial interface IAxonDiscoveryServer : Axon.IServiceServer
//    {
//        IAxonDiscoveryHandler Handler
//        {
//            get;
//        }
//    }

//    public partial interface IAxonDiscoveryHandler
//    {
//        Task RegisterService(string serviceName, string endpoint);

//        Task<string> ResolveRegisteredService(string serviceName, int timeout);

//        Task<IEnumerable<string>> ResolveRegisteredServices(string serviceName, int timeout);
//    }

//    public partial class AxonDiscoveryClient : Axon.AServiceClient, IAxonDiscoveryClient
//    {
//        public AxonDiscoveryClient(Axon.IClientTransport transport, Axon.IProtocol protocol)
//            : base(transport, protocol)
//        {
//            // class/constructor/initializer

//            this.Initialize();
//        }

//        public async Task RegisterService(string serviceName, string endpoint)
//        {
//            var readHandler = await this.Protocol.WriteAndReadData(this.Transport, new Dictionary<string, byte[]>(), (protocol) =>
//            {
//                protocol.WriteRequestHeader(new Axon.RequestHeader("registerService", 2));

//                if (serviceName != null)
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("serviceName", "string"));

//                    protocol.WriteStringValue(serviceName);
//                }
//                else
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("serviceName", "null"));
//                }

//                if (endpoint != null)
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("endpoint", "string"));

//                    protocol.WriteStringValue(endpoint);
//                }
//                else
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("endpoint", "null"));
//                }
//            });

//            await readHandler((protocol, metadata) => {
//                var responseHeader = protocol.ReadResponseHeader();

//                if (!responseHeader.Success)
//                {
//                    var modelHeader = protocol.ReadModelHeader();

//                    var error = new Axon.RequestError();
//                    error.Read(protocol, modelHeader);

//                    throw error.ToException();
//                }
//            });
//        }

//        public async Task<string> ResolveRegisteredService(string serviceName, int timeout)
//        {
//            var readHandler = await this.Protocol.WriteAndReadData<string>(this.Transport, new Dictionary<string, byte[]>(), (protocol) =>
//            {
//                protocol.WriteRequestHeader(new Axon.RequestHeader("resolveRegisteredService", 2));

//                if (serviceName != null)
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("serviceName", "string"));

//                    protocol.WriteStringValue(serviceName);
//                }
//                else
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("serviceName", "null"));
//                }

//                protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("timeout", "int"));

//                protocol.WriteIntegerValue(timeout);
//            });

//            var responseData = await readHandler((protocol, metadata) => {
//                string result;

//                var responseHeader = protocol.ReadResponseHeader();

//                if (!responseHeader.Success)
//                {
//                    var modelHeader = protocol.ReadModelHeader();

//                    var error = new Axon.RequestError();
//                    error.Read(protocol, modelHeader);

//                    throw error.ToException();
//                }
//                else
//                {
//                    result = protocol.ReadStringValue();

//                    return result;
//                }
//            });

//            return responseData;
//        }

//        public async Task<IEnumerable<string>> ResolveRegisteredServices(string serviceName, int timeout)
//        {
//            var readHandler = await this.Protocol.WriteAndReadData<List<string>>(this.Transport, new Dictionary<string, byte[]>(), (protocol) =>
//            {
//                protocol.WriteRequestHeader(new Axon.RequestHeader("resolveRegisteredServices", 2));

//                if (serviceName != null)
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("serviceName", "string"));

//                    protocol.WriteStringValue(serviceName);
//                }
//                else
//                {
//                    protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("serviceName", "null"));
//                }

//                protocol.WriteRequestArgumentHeader(new Axon.RequestArgumentHeader("timeout", "int"));

//                protocol.WriteIntegerValue(timeout);
//            });

//            var responseData = await readHandler((protocol, metadata) => {
//                List<string> result;

//                var responseHeader = protocol.ReadResponseHeader();

//                if (!responseHeader.Success)
//                {
//                    var modelHeader = protocol.ReadModelHeader();

//                    var error = new Axon.RequestError();
//                    error.Read(protocol, modelHeader);

//                    throw error.ToException();
//                }
//                else
//                {
//                    var arrayHeader = protocol.ReadArrayHeader();

//                    result = new List<string>();

//                    for (var i = 0; i < arrayHeader.ItemCount; i++)
//                    {
//                        var arrayItemHeader = protocol.ReadArrayItemHeader();

//                        if (arrayItemHeader.Type != "string")
//                            throw new Exception("Invalid array item type (" + arrayItemHeader.Type + " | string)");

//                        string item;
//                        item = protocol.ReadStringValue();

//                        result.Add(item);
//                    }

//                    return result;
//                }
//            });

//            return responseData;
//        }

//        partial void Initialize();
//    }

//    public partial class AxonDiscoveryServer : Axon.AServiceServer, IAxonDiscoveryServer
//    {
//        private readonly IAxonDiscoveryHandler handler;

//        public IAxonDiscoveryHandler Handler
//        {
//            get
//            {
//                return this.handler;
//            }
//        }

//        public AxonDiscoveryServer(Axon.IServerTransport transport, Axon.IProtocol protocol, IAxonDiscoveryHandler handler)
//            : base(transport, protocol)
//        {
//            // class/constructor/initializer
//            this.handler = handler;

//            this.Initialize();
//        }

//        public AxonDiscoveryServer(Axon.IServerTransport transport, Axon.IProtocol protocol, IAxonDiscoveryHandler handler, Axon.IAnnouncer announcer, Axon.IEncodableEndpoint endpoint)
//            : base(transport, protocol, announcer, endpoint)
//        {
//            // class/constructor/initializer
//            this.handler = handler;

//            this.Initialize();
//        }

//        protected async override Task HandleRequest()
//        {
//            var readHandler = await this.Protocol.ReadData<Func<Task<Axon.HandledRequestMessage>>>(this.Transport, (protocol, metadata) => new Func<Task<Axon.HandledRequestMessage>>(async () => {
//                object requestData = null;

//                var requestHeader = protocol.ReadRequestHeader();

//                switch (requestHeader.ActionName)
//                {
//                    case "registerService":
//                        {
//                            var args = new Dictionary<string, object>();

//                            for (var i = 0; i < requestHeader.ArgumentCount; i++)
//                            {
//                                var requestArgumentHeader = protocol.ReadRequestArgumentHeader();

//                                switch (requestArgumentHeader.ArgumentName)
//                                {
//                                    case "serviceName":
//                                        {
//                                            string param;

//                                            if (requestArgumentHeader.Type == "null")
//                                            {
//                                                param = null;
//                                            }
//                                            else
//                                            {
//                                                param = protocol.ReadStringValue();
//                                            }

//                                            args.Add("serviceName", param);
//                                        }; break;
//                                    case "endpoint":
//                                        {
//                                            string param;

//                                            if (requestArgumentHeader.Type == "null")
//                                            {
//                                                param = null;
//                                            }
//                                            else
//                                            {
//                                                param = protocol.ReadStringValue();
//                                            }

//                                            args.Add("endpoint", param);
//                                        }; break;
//                                    default:
//                                        throw new Exception("Unknown action argument " + requestArgumentHeader.ArgumentName + " for action " + requestHeader.ActionName);
//                                }
//                            }

//                            try
//                            {
//                                await this.Handler.RegisterService((string)args["serviceName"], (string)args["endpoint"]);
//                            }
//                            catch (Exception ex)
//                            {
//                                return new Axon.HandledRequestMessage(requestHeader.ActionName, metadata, args, ex);
//                            }
//                        }; break;
//                    case "resolveRegisteredService":
//                        {
//                            var args = new Dictionary<string, object>();

//                            for (var i = 0; i < requestHeader.ArgumentCount; i++)
//                            {
//                                var requestArgumentHeader = protocol.ReadRequestArgumentHeader();

//                                switch (requestArgumentHeader.ArgumentName)
//                                {
//                                    case "serviceName":
//                                        {
//                                            string param;

//                                            if (requestArgumentHeader.Type == "null")
//                                            {
//                                                param = null;
//                                            }
//                                            else
//                                            {
//                                                param = protocol.ReadStringValue();
//                                            }

//                                            args.Add("serviceName", param);
//                                        }; break;
//                                    case "timeout":
//                                        {
//                                            int param;

//                                            param = protocol.ReadIntegerValue();

//                                            args.Add("timeout", param);
//                                        }; break;
//                                    default:
//                                        throw new Exception("Unknown action argument " + requestArgumentHeader.ArgumentName + " for action " + requestHeader.ActionName);
//                                }
//                            }

//                            try
//                            {
//                                requestData = await this.Handler.ResolveRegisteredService((string)args["serviceName"], (int)args["timeout"]);
//                            }
//                            catch (Exception ex)
//                            {
//                                return new Axon.HandledRequestMessage(requestHeader.ActionName, metadata, ex);
//                            }
//                        }; break;
//                    case "resolveRegisteredServices":
//                        {
//                            var args = new Dictionary<string, object>();

//                            for (var i = 0; i < requestHeader.ArgumentCount; i++)
//                            {
//                                var requestArgumentHeader = protocol.ReadRequestArgumentHeader();

//                                switch (requestArgumentHeader.ArgumentName)
//                                {
//                                    case "serviceName":
//                                        {
//                                            string param;

//                                            if (requestArgumentHeader.Type == "null")
//                                            {
//                                                param = null;
//                                            }
//                                            else
//                                            {
//                                                param = protocol.ReadStringValue();
//                                            }

//                                            args.Add("serviceName", param);
//                                        }; break;
//                                    case "timeout":
//                                        {
//                                            int param;

//                                            param = protocol.ReadIntegerValue();

//                                            args.Add("timeout", param);
//                                        }; break;
//                                    default:
//                                        throw new Exception("Unknown action argument " + requestArgumentHeader.ArgumentName + " for action " + requestHeader.ActionName);
//                                }
//                            }

//                            try
//                            {
//                                requestData = await this.Handler.ResolveRegisteredServices((string)args["serviceName"], (int)args["timeout"]);
//                            }
//                            catch (Exception ex)
//                            {
//                                return new Axon.HandledRequestMessage(requestHeader.ActionName, metadata, ex);
//                            }
//                        }; break;
//                    default:
//                        throw new Exception("Unknown action " + requestHeader.ActionName);
//                }

//                return new Axon.HandledRequestMessage(requestHeader.ActionName, metadata, requestData);
//            }));

//            var writeTask = readHandler().ContinueWith(async (handledRequestTask) => {
//                var handledRequest = handledRequestTask.Result;

//                await this.Protocol.WriteData(this.Transport, handledRequest.Metadata, (protocol) =>
//                {
//                    if (handledRequest.Success)
//                    {
//                        switch (handledRequest.ActionName)
//                        {
//                            case "resolveRegisteredService":
//                                {
//                                    protocol.WriteResponseHeader(new Axon.ResponseHeader(true, "string"));

//                                    var handledResult = (string)handledRequest.Result;

//                                    protocol.WriteStringValue(handledResult);
//                                }; break;
//                            case "resolveRegisteredServices":
//                                {
//                                    protocol.WriteResponseHeader(new Axon.ResponseHeader(true, "array"));

//                                    var handledResult = (IEnumerable<string>)handledRequest.Result;

//                                    protocol.WriteArrayHeader(new Axon.ArrayHeader(handledResult.Count()));
//                                    foreach (var item in handledResult)
//                                    {
//                                        protocol.WriteArrayItemHeader(new Axon.ArrayItemHeader("string"));
//                                        protocol.WriteStringValue(item);
//                                    }
//                                }; break;
//                        }
//                    }
//                    else
//                    {
//                        Console.WriteLine(handledRequest.Exception.Message + ": " + handledRequest.Exception.StackTrace);

//                        protocol.WriteResponseHeader(new Axon.ResponseHeader(false, "any"));

//                        var error = new Axon.RequestError(handledRequest.Exception.Message);

//                        protocol.WriteModelHeader(new Axon.ModelHeader("requestError", 1));
//                        error.Write(protocol);
//                    }
//                });
//            });
//        }

//        partial void Initialize();
//    }

//    public class AxonDiscoveryService : IAxonDiscoveryHandler
//    {
//        private readonly Dictionary<string, Queue<string>> RegisteredServices;

//        public AxonDiscoveryService()
//        {
//            this.RegisteredServices = new Dictionary<string, Queue<string>>();
//        }

//        public Task RegisterService(string serviceName, string endpoint)
//        {
//            //Console.WriteLine("RegisterService(" + serviceName + ")");

//            if (!this.RegisteredServices.ContainsKey(serviceName))
//                this.RegisteredServices.Add(serviceName, new Queue<string>());

//            if (!this.RegisteredServices[serviceName].Contains(endpoint))
//                this.RegisteredServices[serviceName].Enqueue(endpoint);

//            return Task.FromResult(true);
//        }

//        public async Task<string> ResolveRegisteredService(string serviceName, int timeout)
//        {
//            //Console.WriteLine("ResolveRegisteredService()");

//            string endpoint;
//            var startTime = DateTime.UtcNow;
//            while (!this.TryDequeueEndpoint(serviceName, out endpoint))
//            {
//                if (timeout > 0 && (DateTime.UtcNow - startTime).TotalMilliseconds > timeout)
//                    throw new Exception($"Discovery timeout ({serviceName})");

//                await Task.Delay(500);
//            }

//            return endpoint;
//        }

//        public async Task<IEnumerable<string>> ResolveRegisteredServices(string serviceName, int timeout)
//        {
//            //Console.WriteLine("ResolveRegisteredServices()");

//            var startTime = DateTime.UtcNow;
//            while (!this.RegisteredServices.ContainsKey(serviceName))
//            {
//                if (timeout > 0 && (DateTime.UtcNow - startTime).TotalMilliseconds > timeout)
//                    throw new Exception($"Discovery timeout ({serviceName})");

//                await Task.Delay(500);
//            }

//            return this.RegisteredServices[serviceName];
//        }

//        private bool TryDequeueEndpoint(string serviceName, out string endpoint)
//        {
//            endpoint = null;

//            if (!this.RegisteredServices.ContainsKey(serviceName))
//                return false;
//            else if (this.RegisteredServices[serviceName].Count <= 0)
//                return false;

//            endpoint = this.RegisteredServices[serviceName].Dequeue();
//            this.RegisteredServices[serviceName].Enqueue(endpoint);

//            return true;
//        }
//    }
//}
