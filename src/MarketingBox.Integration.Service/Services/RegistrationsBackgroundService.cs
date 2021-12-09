using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Domain.Extensions;
using MarketingBox.Integration.Service.Domain.Repositories;
using MarketingBox.Integration.Service.Domain.Utils;
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
        private readonly ICrmService _depositRegistrationService;
        private readonly BridgeStorage _bridgeStorage;
        private readonly IRegistrationsLogRepository _repository;

        public RegistrationsBackgroundService(
            ILogger<RegistrationsBackgroundService> logger,
            ICrmService depositRegistrationService,
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
                    var potentionalCrmStatusUpdatersDb = await _repository.GetPotentionalDepositorsByBrandAsync(
                        bridge.Value.TenantId, bridge.Value.IntegrationId);

                    // Nothing to find
                    if (potentionalCrmStatusUpdatersDb.Count == 0)
                    {
                        continue;
                    }

                    var potentionalCrmStatusUpdaters = potentionalCrmStatusUpdatersDb
                        .Select(i => new RegistrationReporting
                        {
                            CustomerEmail = i.CustomerEmail,
                            CustomerId = i.CustomerId,
                            CreatedAt = i.CreatedAt,
                            Crm = i.Crm,
                            CrmUpdatedAt = i.CrmUpdatedAt

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
                            var intersectList = realCrmStatusUpdaters.Items
                                .Select(a => a.CustomerId)
                                .Intersect(potentionalCrmStatusUpdaters.Select(b => b.CustomerId));

                            var updateList = realCrmStatusUpdaters.Items.Where(x => intersectList.Contains(x.CustomerId));

                            foreach (var updateItem in updateList)
                            {
                                var itemFromDb = potentionalCrmStatusUpdatersDb.FirstOrDefault(x => x.CustomerId.Equals(updateItem.CustomerId));
                                if (itemFromDb == null)
                                {
                                    _logger.LogWarning("Can't find registration in db {@Registration}", updateItem);
                                    continue;
                                };

                                if (itemFromDb.Crm == updateItem.Crm)
                                {
                                    continue;
                                }
                                itemFromDb.CrmUpdatedAt = updateItem.CrmUpdatedAt;
                                itemFromDb.Crm = updateItem.Crm;
                                itemFromDb.Sequence++;
                                await _repository.SaveAsync(itemFromDb);

                                // TODO Add new method for crm update
                                await _depositRegistrationService
                                    .SetCrmStatusAsync(MapToRequest(itemFromDb));
                                _logger.LogInformation("New crm status {@Registration}", itemFromDb);
                            }

                            count = realCrmStatusUpdaters.Items.Count;
                            request.NextPage();

                        } while (count == request.PageSize);
                        request.NextMonth();
                    } while (request.DateTo <= CalendarUtils.EndOfMonth(DateTime.UtcNow));
                }
                catch (Exception e)
                {
                    _logger.LogError("RegistrationsBackgroundService exception {@Message}", e.Message);
                }
            }
        }

        private MarketingBox.Registration.Service.Grpc.Models.Crm.UpdateCrmStatusRequest MapToRequest(
            MarketingBox.Integration.Service.Domain.Registrations.RegistrationLog message)
        {
            return new MarketingBox.Registration.Service.Grpc.Models.Crm.UpdateCrmStatusRequest()
            {
                CustomerId = message.CustomerId,
                Crm = message.Crm.MapEnum<Registration.Service.Domain.Crm.CrmStatus>(),  
                CrmUpdatedAt = message.CrmUpdatedAt,
                IntegrationId = message.IntegrationId,
                IntegrationName = message.IntegrationName,
                RegistrationId = message.RegistrationId,
                RegistrationUniqueId = message.RegistrationUniqueId,
            };
        }

    }
}