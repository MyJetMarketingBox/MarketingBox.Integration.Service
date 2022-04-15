using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Integration.Service.Utils;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using MarketingBox.Integration.Service.Domain.Repositories;
using MarketingBox.Integration.Service.Domain.Registrations;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using IntegrationRegistration = MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration.Registration;

namespace MarketingBox.Integration.Service.Services
{
    public class SendRegistrationsService : IIntegrationService
    {
        private readonly ILogger<SendRegistrationsService> _logger;
        private readonly BridgeStorage _bridgeStorage;
        private readonly IRegistrationsLogRepository _repository;


        public SendRegistrationsService(ILogger<SendRegistrationsService> logger,
            BridgeStorage bridgeStorage, 
            IRegistrationsLogRepository repository)
        {
            _logger = logger;
            _bridgeStorage = bridgeStorage;
            _repository = repository;
        }

        public async Task<Response<IntegrationRegistration>> SendRegisterationAsync(Grpc.Models.Registrations.Contracts.Integration.RegistrationRequest request)
        {
            _logger.LogInformation("Creating new RegistrationInfo {@context}", request); 
            try
            {
                 var registration =  new Grpc.Models.Registrations.Contracts.Bridge.RegistrationRequest()
                {
                    Info = new RegistrationInfo()
                        { 
                            Email = request.Info.Email,
                            Password = request.Info.Password,
                            Country = request.Info.Country,
                            FirstName = request.Info.FirstName,
                            Ip = request.Info.Ip,
                            Language = LanguageUtils.GetIso2LanguageFromCountryFirstOrDefault(request.Info.Country),
                            LastName = request.Info.LastName,
                            Phone = request.Info.Phone
                        },
                    AdditionalInfo = new RegistrationAdditionalInfo()
                    {
                        So = request.AdditionalInfo.So,
                        Sub = request.AdditionalInfo.Sub,
                        Sub1 = request.AdditionalInfo.Sub1,
                        Sub2 = request.AdditionalInfo.Sub2,
                        Sub3 = request.AdditionalInfo.Sub3,
                        Sub4 = request.AdditionalInfo.Sub4,
                        Sub5 = request.AdditionalInfo.Sub5,
                        Sub6 = request.AdditionalInfo.Sub6,
                        Sub7 = request.AdditionalInfo.Sub7,
                        Sub8 = request.AdditionalInfo.Sub8,
                        Sub9 = request.AdditionalInfo.Sub9,
                        Sub10 = request.AdditionalInfo.Sub10,
                    },
                 };

                var bridge = _bridgeStorage.Get(request.IntegrationId);
                if (bridge == null)
                {
                    _logger.LogWarning("Can't find bridge for integration {@context}", request);
                    throw new NotFoundException($"Bridge for {nameof(request.IntegrationName)} {request.IntegrationName}", request.IntegrationId);
                }

                var customerInfo = await bridge.Service.SendRegistrationAsync(registration);

                //Store successfull customers registrations
                if (customerInfo.Status == ResponseStatus.Ok)
                {
                    var dt = DateTime.UtcNow;
                    await _repository.SaveAsync(new RegistrationLog
                    {
                        TenantId = request.TenantId,
                        RegistrationId = request.RegistrationId,
                        RegistrationUniqueId = request.RegistrationUniqueId,
                        CreatedAt = dt,
                        AffiliateId = request.AffiliateId,
                        CustomerId = customerInfo.Data.CustomerId,
                        CustomerEmail = request.Info.Email,
                        CustomerCreatedAt = dt,
                        Depositor = false,
                        //DepositedAt = null,
                        Crm = CrmStatus.New,
                        CrmUpdatedAt = dt,
                        IntegrationName = request.IntegrationName,
                        IntegrationId = request.IntegrationId,
                        Sequence = 0
                    });
                }

                _logger.LogInformation("Created RegistrationInfo {@context}", customerInfo);

                return new Response<IntegrationRegistration>
                {
                    Status = ResponseStatus.Ok,
                    Data = new IntegrationRegistration
                    {
                        Customer = customerInfo.Data,
                        Message = customerInfo.Data.LoginUrl
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

                return e.FailedResponse<IntegrationRegistration>();
            }
        }
    }
}
