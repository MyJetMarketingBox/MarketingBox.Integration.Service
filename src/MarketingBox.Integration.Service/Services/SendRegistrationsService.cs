using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Integration.Bridge.Client;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Integration.Service.Grpc.Models.Common;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Integration.Service.Utils;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using Error = MarketingBox.Integration.Service.Grpc.Models.Common.Error;
using ErrorType = MarketingBox.Integration.Service.Grpc.Models.Common.ErrorType;


namespace MarketingBox.Integration.Service.Services
{
    public class SendRegistrationsService : IIntegrationService
    {
        private readonly ILogger<SendRegistrationsService> _logger;
        private readonly BridgeServiceWrapper _bridgeServiceWrapper;
        private readonly IDepositUpdateStorage _depositUpdateStorage;
        private readonly BridgeStorage _bridgeStorage;


        public SendRegistrationsService(ILogger<SendRegistrationsService> logger,
            BridgeServiceWrapper bridgeServiceWrapper,
            IDepositUpdateStorage depositUpdateStorage,
            BridgeStorage bridgeStorage)
        {
            _logger = logger;
            _bridgeServiceWrapper = bridgeServiceWrapper;
            _depositUpdateStorage = depositUpdateStorage;
            _bridgeStorage = bridgeStorage;
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

                var customerInfo =
                    await _bridgeServiceWrapper.TryGetService(request.IntegrationName).SendRegistrationAsync(registration);

                var bridges = _bridgeStorage.GetAll();
                foreach (var bridge in bridges)
                {
                    if (bridge.Key.Equals(request.IntegrationName))
                    {
                        var customer =
                            await bridge.Value.SendRegistrationAsync(registration);
                    }
                }

                //TODO: Move deposit generator to another service
                if (customerInfo.ResultCode == ResultCode.CompletedSuccessfully)
                {
                    _depositUpdateStorage.Add(request.RegistrationUniqueId, new DepositUpdateMessage()
                    {
                        IntegrationName = request.IntegrationName,
                        CustomerId = customerInfo.CustomerInfo.CustomerId,
                        Email = request.Info.Email,
                        TenantId = request.TenantId,
                        Sequence = 0,
                        IntegrationId = request.IntegrationId,
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
