﻿using Autofac;
using MarketingBox.Integration.Service.Messages;
using MarketingBox.Integration.Service.Messages.Deposits;
using MarketingBox.Integration.Service.MyNoSql.Leads;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Integration.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    ApplicationEnvironment.HostName, Program.LogFactory);

            //var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            #region Leads

            // publisher (IPublisher<DepositUpdateMessage>)
            builder.RegisterMyServiceBusPublisher<DepositUpdateMessage>(serviceBusClient, Topics.LeadDepositUpdateTopic, false);

            // publisher (IPublisher<PartnerRemoved>)
            //builder.RegisterMyServiceBusPublisher<PartnerRemoved>(serviceBusClient, Topics.LeadUpdatedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<LeadNoSql>)
            builder.RegisterMyNoSqlWriter<LeadNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), LeadNoSql.TableName);
            
            #endregion
        }
    }
}
