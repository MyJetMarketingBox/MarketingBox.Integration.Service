using System;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;


namespace MarketingBox.Integration.Service.Services
{
    public class BackgroundService
    {
        private readonly MyTaskTimer _operationsTimer;
        private readonly ILogger<BackgroundService> _logger;
        private readonly IDepositUpdateStorage _depositUpdateStorage;
        private readonly IDepositService _depositRegistrationService;

        public BackgroundService(ILogger<BackgroundService> logger,
            IDepositUpdateStorage depositUpdateStorage, 
            IDepositService depositRegistrationService)
        {
            _logger = logger;
            _depositUpdateStorage = depositUpdateStorage;
            _depositRegistrationService = depositRegistrationService;
            _operationsTimer = new MyTaskTimer(nameof(BackgroundService), TimeSpan.FromSeconds(10), logger, Process);

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
                var messages = _depositUpdateStorage.GetAll();
                foreach (var operation in messages)
                {
                    var message = operation.Value;
                    var storeResponse = await _depositRegistrationService.RegisterDepositAsync(
                        new MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts.DepositCreateRequest()
                    {
                        CustomerId = message.CustomerId,
                        Email = message.Email,
                        BrandName = message.IntegrationName,
                        BrandId = message.IntegrationId,
                        TenantId = message.TenantId,
                        CreatedAt = DateTime.UtcNow,
                    });
                    _depositUpdateStorage.Remove(operation.Key);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("IntegrationGrpcService exception {@Message}", e.Message);
            }
        }
    }
}