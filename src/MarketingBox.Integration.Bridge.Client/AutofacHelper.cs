using System.Collections.Generic;
using System.Linq;

namespace MarketingBox.Integration.Bridge.Client
{
    public class BridgeServiceWrapper
    {
        private readonly Dictionary<string, IBridgeService> _services;


        public BridgeServiceWrapper(IReadOnlyCollection<(string, IBridgeService)> pairs)
        {
            _services = pairs.ToDictionary(x => x.Item1, y=>y.Item2);
        }

        public IBridgeService TryGetService(string name)
        {
            if (_services.TryGetValue(name, out var service))
            {
                return service;
            }

            return null;
        }
    }
}
