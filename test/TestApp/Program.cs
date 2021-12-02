using System;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Client;
using ProtoBuf.Grpc.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var factory = new IntegrationServiceClientFactory("http://localhost:12347");
            var client = factory.GetIntegrationService();

            var check = await client.SendRegisterationAsync(new MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration.RegistrationRequest()
            {
                TenantId = "test-tenant-id",
                
            });

            //var testTenant = "Test-Tenant";
            //var request = new MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge.RegistrationRequest()
            //{
            //    TenantId = testTenant,
            //};
            //request.Info = new RegistrationInfo()
            //{
            //    //Currency = Currency.CHF,
            //    //Email = "email@email.com",
            //    //Password = "sadadadwad",
            //    //Phone = "+79990999999",
            //    //Skype = "skype",
            //    //Type = LeadType.Active,
            //    //Username = "User",
            //    //ZipCode = "414141"
            //};

            //var leadCreated = (await  client.CreateAsync(request)).RegistrationBrandInfo;

            //Console.WriteLine(leadCreated.RegistrationId);

            //var partnerUpdated = (await client.UpdateAsync(new LeadUpdateRequest()
            //{
            //    RegistrationId = leadCreated.RegistrationId,
            //    TenantId = leadCreated.TenantId,
            //    Info = request.Info,
            //    Sequence = 1
            //})).RegistrationInfo;

            //await client.DeleteAsync(new LeadDeleteRequest()
            //{
            //    RegistrationId = partnerUpdated.RegistrationId,
            //});

            //var shouldBeNull =await client.GetAsync(new LeadGetRequest()
            //{
            //    RegistrationId = partnerUpdated.RegistrationId,
            //});

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
