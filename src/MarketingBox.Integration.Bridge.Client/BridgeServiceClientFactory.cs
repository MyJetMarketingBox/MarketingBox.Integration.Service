using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.Integration.Bridge.Client
{
    [UsedImplicitly]
    public class BridgeServiceClientFactory: MyGrpcClientFactory
    {
        public BridgeServiceClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IBridgeService GetBridgeService() => CreateGrpcService<IBridgeService>();
    }
}
