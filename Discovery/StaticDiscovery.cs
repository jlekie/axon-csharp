using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Axon
{
    public class StaticDiscoverer<TEndpoint> : ADiscoverer<TEndpoint> where TEndpoint : IEncodableEndpoint
    {
        private string[] StaticHosts;
        private Random DiscoveryRandomizer;

        public StaticDiscoverer(string identifier, IEndpointDecoder<TEndpoint> endpointDecoder, IEnumerable<string> staticHosts)
            : base(identifier, endpointDecoder)
        {
            this.StaticHosts = staticHosts.ToArray();
            this.DiscoveryRandomizer = new Random();
        }

        public override Task<TEndpoint> Discover(int timeout = 0)
        {
            var staticHost = this.StaticHosts[this.DiscoveryRandomizer.Next(this.StaticHosts.Length)];

            var facets = staticHost.Split(':');
            var hostname = facets[0];
            var port = int.Parse(facets[1]);

            return Task.FromResult(this.EndpointDecoder.Create(hostname, port));
        }
        public override Task<TEndpoint[]> DiscoverAll(int timeout = 0)
        {
            return Task.FromResult(this.StaticHosts.Select(staticHost =>
            {
                var facets = staticHost.Split(':');
                var hostname = facets[0];
                var port = int.Parse(facets[1]);

                return this.EndpointDecoder.Create(hostname, port);
            }).ToArray());
        }
        public override Task Blacklist(TEndpoint endpoint)
        {
            Console.WriteLine("BLACKLISTING " + Encoding.UTF8.GetString(endpoint.Encode()));

            return Task.FromResult(true);
        }
    }
}
