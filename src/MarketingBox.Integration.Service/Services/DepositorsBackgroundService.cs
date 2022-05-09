using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Domain.Registrations;
using MarketingBox.Integration.Service.Domain.Repositories;
using MarketingBox.Integration.Service.Domain.Utils;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Bridge;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Deposits;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;


namespace MarketingBox.Integration.Service.Services
{
    public class DepositorsBackgroundService
    {
        private const int TimerSpan10Min = 10*60;
        private const int TimerSpan60Sec = 60;
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
            _operationsTimer = new MyTaskTimer(nameof(DepositorsBackgroundService), TimeSpan.FromSeconds(TimerSpan10Min), logger, Process);
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
                Console.WriteLine($"{DateTime.UtcNow}\tBridge {bridge.Value.IntegrationName} start check depositors statuses for {bridge.Value.TenantId}");
                try
                {
                    var potentionalDepositorsFromDb = await _repository.GetPotentionalDepositorsByBrandAsync(
                        bridge.Value.TenantId, bridge.Value.IntegrationId);

                    // Nothing to find
                    if (potentionalDepositorsFromDb.Count == 0)
                    {
                        Console.WriteLine($"{DateTime.UtcNow}\tBridge {bridge.Value.IntegrationName} finished check depositors statuses for {bridge.Value.TenantId} - no potentional depositors");
                        continue;
                    }

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
                            if (realDepositors.Data == null)
                            {
                                break;
                            }
                            // Update and notify only new potentional depositors
                            var intersectList = realDepositors.Data
                                .Select(a => a.CustomerId)
                                .Intersect(potentionalDepositors.Select(b => b.CustomerId));

                            var updateList = realDepositors.Data.Where(x => intersectList.Contains(x.CustomerId));

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
                                    MapToRequest(itemFromDb));
                                _logger.LogInformation("New depositor added {@Registration}", itemFromDb);
                            }

                            count = realDepositors.Data.Count;
                            request.NextPage();

                        } while (count == request.PageSize);
                        request.NextMonth();
                    } while (request.DateTo <= CalendarUtils.EndOfMonth(DateTime.UtcNow));
                }
                catch (Exception e)
                {
                    _logger.LogError("DepositorsBackgroundService exception {@Message}", e.Message);
                }
                Console.WriteLine($"{DateTime.UtcNow}\tBridge {bridge.Value.IntegrationName} finished check depositors statuses for {bridge.Value.TenantId}");
            }
        }
        private DepositCreateRequest MapToRequest(
            RegistrationLog message)
        {
            return new DepositCreateRequest()
            {
                RegistrationId = message.RegistrationId,
                TenantId = message.TenantId,
            };
        }
    }
}