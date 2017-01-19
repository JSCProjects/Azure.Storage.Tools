namespace JSCProjects.Azure.AzureTools.Options
{
    using CommandLine;

    [Verb("set-metadata-for-blob", HelpText = "Set meta data of a file")]
    public class SetMetaDataForBlob
    {
        [Option('c', "container", HelpText = "Blob container", Required = true)]
        public string Container { get; set; }

        [Option('k', "key", HelpText = "Key of metadata", Required = true)]
        public string Key { get; set; }

        public string Value { get; set; }

        [Option("valueResolverType", HelpText = "ValueResolverType")]
        public string ValueResolverType { get; set; }
    }
}
