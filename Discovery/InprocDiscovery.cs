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
        public static ConcurrentDictionary<string, byte[][]> Endpoints = new ConcurrentDictionary<string, byte[][]>();

        public static void AddEndpoint(string identifier, byte[] endpoint)
        {
            Endpoints.AddOrUpdate(identifier, new byte[][] { endpoint }, (_, existingEndpoints) => existingEndpoints.Concat(new byte[][] { endpoint }).ToArray());
        }
        public static byte[] GetEndpoint(string identifier)
        {
            var endpoints = Endpoints.GetOrAdd(identifier, new byte[][] { });
            var queue = new Queue<byte[]>(endpoints);

            var endpoint = queue.Dequeue();
            queue.Enqueue(endpoint);

            Endpoints.AddOrUpdate(identifier, queue.ToArray(), (i, e) => queue.ToArray());

            return endpoint;
        }
        public static void RemoveEndpoint(string identifier, byte[] endpoint)
        {
            var endpoints = Endpoints.GetOrAdd(identifier, new byte[][] { });

            var filteredEndpoints = endpoints.Where(e => !e.SequenceEqual(endpoint));

            Endpoints.AddOrUpdate(identifier, filteredEndpoints.ToArray(), (i, e) => filteredEndpoints.ToArray());
        }
    }

    public class InprocAnnouncer : AAnnouncer
    {
        public InprocAnnouncer(string identifier)
            : base(identifier)
        {
        }

        public override async Task Register(IEncodableEndpoint endpoint)
        {
            InprocData.AddEndpoint(this.Identifier, endpoint.Encode());
        }

        public void Deregister(IEncodableEndpoint endpoint)
        {
            InprocData.RemoveEndpoint(this.Identifier, endpoint.Encode());
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
            var encodedEndpoint = InprocData.GetEndpoint(this.Identifier);

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
