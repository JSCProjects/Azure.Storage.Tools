namespace JSCProjects.Azure.AzureTools.Options
{
    using CommandLine;

    [Verb("resend-dlq", HelpText = "Resend DLQ messages")]
    public class ResendFromDeadLetterQueue
    {
        [Option('q', "queue", HelpText = "Queue name", Required = true)]
        public string Name { get; set; }

        [Option('s', "batchSize", HelpText = "BatchSize of messages to resend", Default = 10)]
        public int BatchSize { get; set; }

        [Option('a', "all", HelpText = "Do we need to resend all the messages?", Default = false)]
        public bool All { get; set; }
    }
}
