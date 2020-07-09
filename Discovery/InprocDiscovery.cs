using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace Axon
{
    internal static class InprocData
    {
        public static ConcurrentDictionary<string, BlockingCollection<byte[]>> Endpoints = new ConcurrentDictionary<string, BlockingCollection<byte[]>>();
    }

    public class InprocAnnouncer : AAnnouncer
    {
        public InprocAnnouncer(string identifier)
            : base(identifier)
        {
        }

        public override async Task Register(IEncodableEndpoint endpoint)
        {
            InprocData.Endpoints.AddOrUpdate(this.Identifier, (identifier) => new BlockingCollection<byte[]>() { endpoint.Encode() }, (identifier, endpoints) =>
            {
                endpoints.Add(endpoint.Encode());
                return endpoints;
            });
        }
    }

    public class InprocDiscoverer<TEndpoint> : ADiscoverer<TEndpoint> where TEndpoint : IEncodableEndpoint
    {
        public InprocDiscoverer(string identifier, IEndpointDecoder<TEndpoint> endpointDecoder)
            : base(identifier, endpointDecoder)
        {
        }

        public override Task<TEndpoint> Discover(int timeout = 0)
        {
            if (!InprocData.Endpoints.TryGetValue(this.Identifier, out var endpoints))
                throw new Exception();

            var encodedEndpoint = endpoints.Take();
            endpoints.Add(encodedEndpoint);

            return Task.FromResult(this.EndpointDecoder.Decode(encodedEndpoint));
        }
        public override Task<TEndpoint[]> DiscoverAll(int timeout = 0)
        {
            if (!InprocData.Endpoints.TryGetValue(this.Identifier, out var endpoints))
                throw new Exception();

            return Task.FromResult(endpoints.Select(encodedEndpoint => this.EndpointDecoder.Decode(encodedEndpoint)).ToArray());
        }
        public override Task Blacklist(TEndpoint endpoint)
        {
            Console.WriteLine("BLACKLISTING " + Encoding.UTF8.GetString(endpoint.Encode()));

            return Task.FromResult(true);
        }
    }
}
