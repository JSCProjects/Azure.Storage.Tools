namespace JSCProjects.Azure.AzureTools.Handlers.MetaData
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using JSCProjects.Azure.AzureTools.Extensions;
    using JSCProjects.Azure.AzureTools.Options;

    using Microsoft.WindowsAzure.Storage.Blob;

    public class MetaDataHandler
    {
        private readonly CloudBlobClient cloudBlobClient;

        public MetaDataHandler(CloudBlobClient cloudBlobClient)
        {
            this.cloudBlobClient = cloudBlobClient;
        }

        public int Handle(SetMetaDataForBlob setting)
        {
            var container = this.cloudBlobClient.GetContainerReference(setting.Container);

            var listBlobItems =
                container.ListBlobs(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.Metadata)
                    .Select(blob => blob as CloudBlockBlob)
                    .Where(x => x != null);

            Parallel.ForEach(
                listBlobItems,
                b =>
                {
                    var cloudBlockBlobMetaDataSetter = new CloudBlockBlobMetaDataSetter(
                        setting.Key,
                        blob => GetValidUntil(blob, TimeSpan.FromDays(31)));

                    cloudBlockBlobMetaDataSetter.Execute(b);
                });
            Console.WriteLine("Finished setting metadata");

            return 0;
        }

        internal static string GetValidUntil(ICloudBlob blob, TimeSpan? timeToBeReceived = null)
        {
            if (timeToBeReceived.HasValue)
            {
                var x = blob.Properties.LastModified ?? DateTime.UtcNow + timeToBeReceived.Value;
                return x.UtcDateTime.ToWireFormattedString();
            }

            return null;
        }
    }
}
