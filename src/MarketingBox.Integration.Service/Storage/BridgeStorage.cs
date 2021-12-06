using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MarketingBox.Integration.Bridge.Client;

namespace MarketingBox.Integration.Service.Storage
{
    public interface IBridgeStorage
    {
        void Add(string requestLeadUniqueId, IBridgeService depositUpdateMessage);
        List<KeyValuePair<string, IBridgeService>> GetAll();
    }

    public class BridgeStorage : IBridgeStorage
    {
        private ConcurrentDictionary<string, IBridgeService> _data = new ConcurrentDictionary<string, IBridgeService>();

        public BridgeStorage(IReadOnlyCollection<(string, IBridgeService)> pairs)
        {
            foreach (var pair in pairs)
            {
                Add(pair.Item1, pair.Item2);
            }
        }

        public void Add(string bridgeName, IBridgeService bridge)
        {
            _data[bridgeName] = bridge;
        }

        public List<KeyValuePair<string, IBridgeService>> GetAll()
        {
            return _data.ToList();
        }
    }
}