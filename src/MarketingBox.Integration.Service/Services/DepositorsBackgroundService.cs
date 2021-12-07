using System;
using System.Threading.Tasks;
using MarketingBox.Integration.Bridge.Client;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;


namespace MarketingBox.Integration.Service.Services
{
    public class DepositorsBackgroundService
    {
        private const int TimerSpan10Min = 10*60;
        private const int TimerSpan30Sec = 30;
        private readonly MyTaskTimer _operationsTimer;
        private readonly ILogger<DepositorsBackgroundService> _logger;
        private readonly IDepositUpdateStorage _depositUpdateStorage;
        private readonly IDepositService _depositRegistrationService;
        private readonly BridgeStorage _bridgeStorage;

        public DepositorsBackgroundService(
            ILogger<DepositorsBackgroundService> logger,
            IDepositUpdateStorage depositUpdateStorage,
            IDepositService depositRegistrationService,
            BridgeStorage bridgeStorage)
        {
            _logger = logger;
            _depositUpdateStorage = depositUpdateStorage;
            _depositRegistrationService = depositRegistrationService;
            _operationsTimer = new MyTaskTimer(nameof(DepositorsBackgroundService), TimeSpan.FromSeconds(TimerSpan30Sec), logger, Process);
            _bridgeStorage = bridgeStorage;
        }
        public void Start()
        {
            _operationsTimer.Start();
        }

        public void Stop()
        {
            _operationsTimer.Stop();
        }

        private async Task Process()
        {
            try
            {
                var bridges = _bridgeStorage.GetAll();
                foreach (var bridge in bridges)
                {

                    var dtStartFrom = DateTime.Parse("2021-01-01");
                    var dtCurrent = dtStartFrom;
                    var request = new ReportingRequest()
                    {
                        DateFrom = dtCurrent,
                        DateTo = dtCurrent.AddMonths(1),
                        PageIndex = 1,
                        PageCount = 100,
                    };
                    do
                    {
                        int count = 0;
                        do
                        {
                            var depositors = await bridge.Value.GetDepositorsPerPeriodAsync(request);
                            if (depositors.Items == null)
                            {
                                break;
                            }
                            count = depositors.Items.Count;
                            request.PageIndex++;

                        } while (count == request.PageCount);
                        request.DateFrom = request.DateFrom.AddMonths(1);
                        request.DateTo = request.DateTo.AddMonths(1);
                        request.PageIndex = 1;
                    } while (request.DateTo <= DateTime.UtcNow);
                }

                //var messages = _depositUpdateStorage.GetAll();
                //foreach (var operation in messages)
                //{
                //    var message = operation.Value;
                //    var storeResponse = await _depositRegistrationService.RegisterDepositAsync(
                //        new MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts.DepositCreateRequest()
                //        {
                //            CustomerId = message.CustomerId,
                //            Email = message.Email,
                //            BrandName = message.IntegrationName,
                //            BrandId = message.IntegrationId,
                //            TenantId = message.TenantId,
                //            CreatedAt = DateTime.UtcNow,
                //        });
                //    _depositUpdateStorage.Remove(operation.Key);
                //}
            }
            catch (Exception e)
            {
                _logger.LogError("IntegrationGrpcService exception {@Message}", e.Message);
            }
        }
    }
}