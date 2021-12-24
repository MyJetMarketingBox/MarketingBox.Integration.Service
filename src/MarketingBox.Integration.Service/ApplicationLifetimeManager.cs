using MarketingBox.Integration.Service.Domain.Repositories;
using MarketingBox.Integration.Service.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;

namespace MarketingBox.Integration.Service
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly DepositorsBackgroundService _depositorsBackgroundService;
        private readonly RegistrationsBackgroundService _registrationsBackgroundService;
        private readonly IRegistrationsLogRepository _registrationsLogRepository;

        public ApplicationLifetimeManager(
            IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            DepositorsBackgroundService depositorsBackgroundService,
            RegistrationsBackgroundService registrationsBackgroundService, IRegistrationsLogRepository registrationsLogRepository)
            : base(appLifetime)
        {
            _logger = logger;
            _depositorsBackgroundService = depositorsBackgroundService;
            _registrationsBackgroundService = registrationsBackgroundService;
            _registrationsLogRepository = registrationsLogRepository;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _depositorsBackgroundService.Start();
            _registrationsBackgroundService.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _depositorsBackgroundService.Stop();
            _registrationsBackgroundService.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
