using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Domain.Repositories;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;


namespace MarketingBox.Integration.Service.Services
{
    public class RegistrationsBackgroundService
    {
        private const int TimerSpan10Min = 10 * 60;
        private const int TimerSpan30Sec = 30;
        private const int PageSize100 = 100;
        private const string StartFrom2021 = "2021-01-01";
        private readonly MyTaskTimer _operationsTimer;
        private readonly ILogger<RegistrationsBackgroundService> _logger;
        private readonly IDepositService _depositRegistrationService;
        private readonly BridgeStorage _bridgeStorage;
        private readonly IRegistrationsLogRepository _repository;

        public RegistrationsBackgroundService(
            ILogger<RegistrationsBackgroundService> logger,
            IDepositService depositRegistrationService,
            BridgeStorage bridgeStorage,
            IRegistrationsLogRepository repository)
        {
            _logger = logger;
            _depositRegistrationService = depositRegistrationService;
            _operationsTimer = new MyTaskTimer(nameof(RegistrationsBackgroundService), TimeSpan.FromSeconds(TimerSpan30Sec), logger, Process);
            _bridgeStorage = bridgeStorage;
            _repository = repository;
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

            var bridges = _bridgeStorage.GetAll();
            foreach (var bridge in bridges)
            {
                try
                {
                    var potentionalCrmStatusUpdaters = (await _repository.GetPotentionalDepositorsByBrandAsync(
                        bridge.Value.TenantId, bridge.Value.IntegrationId))
                        .Select(i => new RegistrationReporting
                        {
                            CustomerEmail = i.CustomerEmail,
                            CustomerId = i.CustomerId,
                            CreatedAt = i.CreatedAt,
                            Crm = i.Crm,
                            CrmStatusChangedAt = i.CrmUpdatedAt

                        });

                    var request = ReportingRequest.Create(DateTime.Parse(StartFrom2021), PageSize100);
                    // Search by Period starting from the 2021
                    do
                    {
                        var count = 0;
                        // Search by PageSize? default 100
                        do
                        {
                            var realCrmStatusUpdaters = await bridge.Value.Service.GetRegistrationsPerPeriodAsync(request);
                            if (realCrmStatusUpdaters.Items == null)
                            {
                                break;
                            }
                            // Notify only new crm updates
                            var updateList = realCrmStatusUpdaters.Items.Intersect(potentionalCrmStatusUpdaters);
                            foreach (var item in updateList)
                            {
                                // TODO Add new method for crm update
                                //var storeResponse = await _depositRegistrationService.RegisterDepositAsync(
                                //    MapToRequest(item, bridge.Value));
                                _logger.LogInformation("Get new crm status {@Registration}", item);
                            }

                            count = realCrmStatusUpdaters.Items.Count;
                            request.NextPage();

                        } while (count == request.PageSize);
                        request.NextMonth();
                    } while (request.DateTo <= DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    _logger.LogError("RegistrationsBackgroundService exception {@Message}", e.Message);
                }
            }
        }

        private MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts.DepositCreateRequest MapToRequest(
            RegistrationReporting message,
            MarketingBox.Integration.Service.Storage.Bridge bridge)
        {
            return new MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts.DepositCreateRequest()
            {
                CustomerId = message.CustomerId,
                Email = message.CustomerEmail,
                BrandName = bridge.IntegrationName,
                BrandId = bridge.IntegrationId,
                TenantId = bridge.TenantId,
                CreatedAt = DateTime.UtcNow,
            };
        }

    }
}