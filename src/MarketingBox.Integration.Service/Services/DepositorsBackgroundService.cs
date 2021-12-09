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
    public class DepositorsBackgroundService
    {
        private const int TimerSpan10Min = 10*60;
        private const int TimerSpan30Sec = 30;
        private const int PageSize100 = 100;
        private const string StartFrom2021 = "2021-01-01";
        private readonly MyTaskTimer _operationsTimer;
        private readonly ILogger<DepositorsBackgroundService> _logger;
        private readonly IDepositService _depositRegistrationService;
        private readonly BridgeStorage _bridgeStorage;
        private readonly IRegistrationsLogRepository _repository;

        public DepositorsBackgroundService(
            ILogger<DepositorsBackgroundService> logger,
            IDepositService depositRegistrationService,
            BridgeStorage bridgeStorage, 
            IRegistrationsLogRepository repository)
        {
            _logger = logger;
            _depositRegistrationService = depositRegistrationService;
            _operationsTimer = new MyTaskTimer(nameof(DepositorsBackgroundService), TimeSpan.FromSeconds(TimerSpan30Sec), logger, Process);
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
                    var potentionalDepositorsFromDb = await _repository.GetPotentionalDepositorsByBrandAsync(
                        bridge.Value.TenantId, bridge.Value.IntegrationId);

                    var potentionalDepositors = potentionalDepositorsFromDb
                        .Select(i => new DepositorReporting
                        {
                            CustomerEmail = i.CustomerEmail,
                            CustomerId = i.CustomerId,
                            DepositedAt = i.DepositedAt
                        });


                    var request = ReportingRequest.Create(DateTime.Parse(StartFrom2021), PageSize100);
                    // Search by Period starting from the 2021
                    do
                    {
                        var count = 0;
                        // Search by PageSize, default 100
                        do
                        {
                            var realDepositors = await bridge.Value.Service.GetDepositorsPerPeriodAsync(request);
                            if (realDepositors.Items == null)
                            {
                                break;
                            }
                            // Update and notify only new potentional depositors
                            var updateList = realDepositors.Items.Intersect(potentionalDepositors);
                            foreach (var updateItem in updateList)
                            {
                                var itemFromDb = potentionalDepositorsFromDb.FirstOrDefault(x => x.CustomerId.Equals(updateItem.CustomerId));
                                if (itemFromDb == null)
                                {
                                    _logger.LogWarning("Can't find depositor in db {@Registration}", updateItem);
                                    continue;
                                };

                                itemFromDb.Depositor = true;
                                itemFromDb.DepositedAt = updateItem.DepositedAt;
                                itemFromDb.Sequence++;
                                await _repository.SaveAsync(itemFromDb);

                                var storeResponse = await _depositRegistrationService.RegisterDepositAsync(
                                    MapToRequest(updateItem, bridge.Value));
                                _logger.LogInformation("New depositor added {@Registration}", itemFromDb);
                            }

                            count = realDepositors.Items.Count;
                            request.NextPage();

                        } while (count == request.PageSize);
                        request.NextMonth();
                    } while (request.DateTo <= DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    _logger.LogError("DepositorsBackgroundService exception {@Message}", e.Message);
                }
            }
        }
        private MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts.DepositCreateRequest MapToRequest(
            MarketingBox.Integration.Service.Grpc.Models.Registrations.DepositorReporting message,
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