namespace JSCProjects.Azure.AzureTools.Handlers.MetaData
{
    using System;

    using Microsoft.WindowsAzure.Storage.Blob;

    public class CloudBlockBlobMetaDataSetter
    {
        private readonly string key;

        private readonly Func<ICloudBlob, string> getValue;

        public CloudBlockBlobMetaDataSetter(string key, Func<ICloudBlob, string> getValue)
        {
            this.key = key;
            this.getValue = getValue;
        }

        public void Execute(CloudBlockBlob cloudBlockBlob)
        {
            if (cloudBlockBlob.Metadata.ContainsKey(this.key))
            {
                Console.Write("#");
                return;
            }

            var value = this.getValue(cloudBlockBlob);

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            cloudBlockBlob.Metadata[this.key] = value;

            cloudBlockBlob.SetMetadata();

            Console.Write(".");
        }
    }
}
