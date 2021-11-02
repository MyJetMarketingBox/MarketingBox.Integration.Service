using System.Collections.Generic;
using System.Linq;
using Autofac;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.Integration.Bridge.Client
{
    //public static class AutofacHelper
    //{
    //    public static void RegisterSimpleTradingBridgeClient(this ContainerBuilder builder, string brandName, string grpcServiceUrl)
    //    {
    //        var factory = new BridgeServiceClientFactory(grpcServiceUrl);
    //        builder.RegisterInstance(factory.GetBridgeService()).As<IBridgeService>().SingleInstance();
    //        //builder.RegisterInstance(factory.GetBridgeService()).As<IBridgeService>().Keyed<IBridgeService>(brandName); 
    //        //builder.RegisterInstance(factory.GetBridgeService()).As<IBridgeService>().Named<string>(brandName);
    //    }
    //}

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
