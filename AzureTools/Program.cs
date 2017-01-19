namespace JSCProjects.Azure.AzureTools
{
    using System.Configuration;
    using System.Net;

    using Autofac;

    using CommandLine;

    using JSCProjects.Azure.AzureTools.Handlers.AzureServiceBus;
    using JSCProjects.Azure.AzureTools.Handlers.MetaData;
    using JSCProjects.Azure.AzureTools.Options;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage;

    using IContainer = Autofac.IContainer;

    class Program
    {
        private static IContainer container;

        static int Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = int.Parse(ConfigurationManager.AppSettings["NumberOfIOThreads"]);

            IoC();

            var optionResult = Parser.Default.ParseArguments<SetMetaDataForBlob, ResendFromDeadLetterQueue>(args);

            return optionResult.MapResult(
                (SetMetaDataForBlob cmd) =>
                    {
                        var handler = container.Resolve<MetaDataHandler>();
                        return handler.Handle(cmd);
                    },
                (ResendFromDeadLetterQueue cmd) =>
                    {
                        var handler = container.Resolve<DeadLetterQueueResendHandler>();
                        return handler.Handle(cmd);
                    },
                errors => 1);
        }

        static void IoC()
        {
            var builder = new ContainerBuilder();
            builder.Register(
                context =>
                    {
                        var storageAccount =
                            CloudStorageAccount.Parse(
                                ConfigurationManager.ConnectionStrings["Default"].ConnectionString);

                        return storageAccount.CreateCloudBlobClient();
                    }).AsSelf();

            builder.Register(
                context =>
                    MessagingFactory.CreateFromConnectionString(
                        ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString)).AsSelf();
            builder.Register(
                context =>
                    NamespaceManager.CreateFromConnectionString(
                        ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString)).AsSelf();
            builder.RegisterType<MetaDataHandler>().AsSelf();
            builder.RegisterType<DeadLetterQueueResendHandler>().AsSelf();
            container = builder.Build();
        }
    }
}