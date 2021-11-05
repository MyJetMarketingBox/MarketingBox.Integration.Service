using MarketingBox.Integration.Service.Grpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Integration.Bridge.Client;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Integration.Service.Grpc.Models.Common;
using MarketingBox.Integration.Service.Grpc.Models.Leads;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Integration.Service.Storage;
using MarketingBox.Integration.Service.Utils;
using Error = MarketingBox.Integration.Service.Grpc.Models.Common.Error;
using ErrorType = MarketingBox.Integration.Service.Grpc.Models.Common.ErrorType;


namespace MarketingBox.Integration.Service.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly ILogger<IntegrationService> _logger;
        private readonly BridgeServiceWrapper _bridgeServiceWrapper;
        private readonly IDepositUpdateStorage _depositUpdateStorage;


        public IntegrationService(ILogger<IntegrationService> logger,
            BridgeServiceWrapper bridgeServiceWrapper,
            IDepositUpdateStorage depositUpdateStorage
            )
        {
            _logger = logger;
            _bridgeServiceWrapper = bridgeServiceWrapper;
            _depositUpdateStorage = depositUpdateStorage;
        }

        public async Task<RegistrationResponse> RegisterLeadAsync(RegistrationRequest request)
        {
            _logger.LogInformation("Creating new RegistrationLeadInfo {@context}", request); 
            try
            {
                 var registration =  new RegistrationBridgeRequest()
                {
                    Info = new RegistrationLeadInfo()
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
                    AdditionalInfo = new RegistrationLeadAdditionalInfo()
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
                     await _bridgeServiceWrapper.TryGetService(request.IntegrationName).RegisterCustomerAsync(registration);

                //TODO: Move deposit generator to another service
                if (customerInfo.ResultCode == ResultCode.CompletedSuccessfully)
                {
                    _depositUpdateStorage.Add(request.LeadUniqueId, new DepositUpdateMessage()
                    {
                        IntegrationName = request.IntegrationName,
                        CustomerId = customerInfo.RegistrationInfo.CustomerId,
                        Email = request.Info.Email,
                        TenantId = request.TenantId,
                        Sequence = 0,
                        IntegrationId = request.IntegrationId,
                    });
                }

                _logger.LogInformation("Created RegistrationLeadInfo {@context}", customerInfo);

                return MapToGrpc(customerInfo, request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

                return new RegistrationResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }


        private static RegistrationResponse MapToGrpc(RegistrationBridgeResponse brandInfo,
            RegistrationRequest registrationRequest)
        {
            if (brandInfo.ResultCode == ResultCode.CompletedSuccessfully)
            {
                return Successfully(new RegisteredLeadInfo()
                {
                    CustomerId = brandInfo.RegistrationInfo.CustomerId,
                    LoginUrl = brandInfo.RegistrationInfo.LoginUrl,
                    Token = brandInfo.RegistrationInfo.Token,
                });
            }

            return Failed(new Error()
                {
                    Message = brandInfo.ResultMessage,
                    Type = brandInfo.Error.Type
                }, 
                new RegistrationLeadInfo()
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

        private static RegistrationResponse Successfully(RegisteredLeadInfo brandRegisteredLeadInfo)
        {
            return new RegistrationResponse()
            {
                Status = ResultCode.CompletedSuccessfully,
                Message = brandRegisteredLeadInfo.LoginUrl,
                RegisteredLeadInfo = brandRegisteredLeadInfo
            };
        }

        private static RegistrationResponse Failed(Error error, RegistrationLeadInfo originalData)
        {
            return new RegistrationResponse()
            {
                Status = ResultCode.Failed,
                Message = error.Message,
                Error = error,
                OriginalData = originalData
            };
        }

        private static RegistrationResponse RequiredAuthentication(Error error, RegistrationLeadInfo originalData)
        {
            return new RegistrationResponse()
            {
                Status = ResultCode.RequiredAuthentication,
                Message = error.Message,
                Error = error,
                OriginalData = originalData
            };
        }
    }
}
