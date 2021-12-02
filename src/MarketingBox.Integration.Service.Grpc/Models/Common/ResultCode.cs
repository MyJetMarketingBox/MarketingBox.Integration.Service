using System.ComponentModel;

namespace MarketingBox.Integration.Service.Grpc.Models.Common
{
    public enum ResultCode
    {
        [Description("RegistrationBridgeRequest failed")]
        Failed = 0,
        [Description("RegistrationBridgeRequest completed successfully")]
        CompletedSuccessfully = 1,
        [Description("Required brand authentication")]
        RequiredAuthentication = 2,
    }
}