using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Integration.Service.Grpc.Models.Common;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Integration.Service.Utils;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using Error = MarketingBox.Integration.Service.Grpc.Models.Common.Error;
using ErrorType = MarketingBox.Integration.Service.Grpc.Models.Common.ErrorType;
using MarketingBox.Integration.Service.Domain.Repositories;
using MarketingBox.Integration.Service.Domain.Registrations;

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

        public async Task<Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse> SendRegisterationAsync(Grpc.Models.Registrations.Contracts.Integration.RegistrationRequest request)
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
                    return new Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse()
                    {
                        Error = new Error()
                        {
                            Message = $"Can't find bridge for integration {request.IntegrationName} {request.IntegrationId}",
                            Type = ErrorType.NoIntegration
                        }
                    };
                }

                var customerInfo = await bridge.Service.SendRegistrationAsync(registration);

                //Store successfull customers registrations
                if (customerInfo.ResultCode == ResultCode.CompletedSuccessfully)
                {
                    var dt = DateTime.UtcNow;
                    await _repository.SaveAsync(new RegistrationLog
                    {
                        TenantId = request.TenantId,
                        RegistrationId = request.RegistrationId,
                        RegistrationUniqueId = request.RegistrationUniqueId,
                        CreatedAt = dt,
                        AffiliateId = request.AffiliateId,
                        CustomerId = customerInfo.CustomerInfo.CustomerId,
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

                return MapToGrpc(customerInfo, request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

                return new Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }


        private static Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse MapToGrpc(
            Grpc.Models.Registrations.Contracts.Bridge.RegistrationResponse brandInfo,
            Grpc.Models.Registrations.Contracts.Integration.RegistrationRequest registrationRequest)
        {
            if (brandInfo.ResultCode == ResultCode.CompletedSuccessfully)
            {
                return Successfully(new CustomerInfo()
                {
                    CustomerId = brandInfo.CustomerInfo.CustomerId,
                    LoginUrl = brandInfo.CustomerInfo.LoginUrl,
                    Token = brandInfo.CustomerInfo.Token,
                });
            }

            return Failed(new Error()
                {
                    Message = brandInfo.ResultMessage,
                    Type = brandInfo.Error.Type
                }, 
                new RegistrationInfo()
                {
                    Email = registrationRequest.Info.Email,
                    Password = registrationRequest.Info.Password,
                    Country = registrationRequest.Info.Country,
                    FirstName = registrationRequest.Info.FirstName,
                    Ip = registrationRequest.Info.Ip,
                    Language = registrationRequest.Info.Language,
                    LastName = registrationRequest.Info.LastName,
                    Phone = registrationRequest.Info.Phone
                }
            );
        }

        private static Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse Successfully(CustomerInfo brandRegisteredLeadInfo)
        {
            return new Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse()
            {
                Status = ResultCode.CompletedSuccessfully,
                Message = brandRegisteredLeadInfo.LoginUrl,
                Customer = brandRegisteredLeadInfo
            };
        }

        private static Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse Failed(Error error, RegistrationInfo originalData)
        {
            return new Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse()
            {
                Status = ResultCode.Failed,
                Message = error.Message,
                Error = error,
                OriginalData = originalData
            };
        }

        private static Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse RequiredAuthentication(
            Error error, RegistrationInfo originalData)
        {
            return new Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse()
            {
                Status = ResultCode.RequiredAuthentication,
                Message = error.Message,
                Error = error,
                OriginalData = originalData
            };
        }

        public Task<Grpc.Models.Registrations.Contracts.Integration.RegistrationResponse> GetRegistrationPerPeriodAsync(Grpc.Models.Registrations.Contracts.Integration.RegistrationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
