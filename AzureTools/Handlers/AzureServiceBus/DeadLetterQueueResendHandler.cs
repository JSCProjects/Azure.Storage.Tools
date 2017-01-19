namespace JSCProjects.Azure.AzureTools.Handlers.AzureServiceBus
{
    using System;
    using System.Threading.Tasks;

    using JSCProjects.Azure.AzureTools.Options;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class DeadLetterQueueResendHandler
    {
        private readonly MessagingFactory messagingFactory;

        private readonly NamespaceManager namespaceManager;

        public DeadLetterQueueResendHandler(MessagingFactory messagingFactory, NamespaceManager namespaceManager)
        {
            this.messagingFactory = messagingFactory;
            this.namespaceManager = namespaceManager;
        }

        public int Handle(ResendFromDeadLetterQueue cmd)
        {
            var dlqClient = this.messagingFactory.CreateQueueClient(
                QueueClient.FormatDeadLetterPath(cmd.Name),
                ReceiveMode.ReceiveAndDelete);
            var qClient = this.messagingFactory.CreateQueueClient(cmd.Name);
            
            if (cmd.All)
            {
                QueueDescription queueDescription = null;
                do
                {
                    Resend(cmd, dlqClient, qClient);
                    queueDescription = this.namespaceManager.GetQueue(cmd.Name);
                }
                while (queueDescription.MessageCountDetails.DeadLetterMessageCount > 0); 
            }
            else
            {
                Resend(cmd, dlqClient, qClient);
            }

            Console.WriteLine("Finished resending to queue");

            return 0;
        }

        private static void Resend(ResendFromDeadLetterQueue cmd, QueueClient dlqClient, QueueClient qClient)
        {
            var brokeredMessages = dlqClient.ReceiveBatch(cmd.BatchSize);

            Parallel.ForEach(brokeredMessages, qClient.Send);
            Console.Write(".");
        }
    }
}
