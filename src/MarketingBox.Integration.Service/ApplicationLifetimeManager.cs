using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;

using DepositorsBackgroundService = MarketingBox.Integration.Service.Services.DepositorsBackgroundService;

namespace MarketingBox.Integration.Service
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly DepositorsBackgroundService _myBackgroundService;

        public ApplicationLifetimeManager(
            IHostApplicationLifetime appLifetime, 
            ILogger<ApplicationLifetimeManager> logger,
            DepositorsBackgroundService myBackgroundService)
            : base(appLifetime)
        {
            _logger = logger;
            _myBackgroundService = myBackgroundService;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myBackgroundService.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
