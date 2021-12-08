using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MarketingBox.Integration.Bridge.Client;

namespace MarketingBox.Integration.Service.Storage
{
    public interface IBridgeStorage
    {
        void Add(long bridgeId, Bridge bridge);
        List<KeyValuePair<long, Bridge>> GetAll();
    }

    public class Bridge
    {
        public string TenantId { get; set; }
        public string IntegrationName { get; set; }
        public long IntegrationId { get; set; }
        public IBridgeService Service { get; set; }
    }

    public class BridgeStorage : IBridgeStorage
    {
        private ConcurrentDictionary<long, Bridge> _data = new ConcurrentDictionary<long, Bridge>();

        public BridgeStorage(IReadOnlyCollection<(long, Bridge)> pairs)
        {
            foreach (var pair in pairs)
            {
                Add(pair.Item1, pair.Item2);
            }
        }

        public void Add(long bridgeId, Bridge bridge)
        {
            _data[bridgeId] = bridge;
        }

        public List<KeyValuePair<long, Bridge>> GetAll()
        {
            return _data.ToList();
        }

        public Bridge Get(long bridgeId)
        {
            if (_data.TryGetValue(bridgeId, out var bridge))
            { 
                return bridge;
            }
            return null;
        }
    }
}