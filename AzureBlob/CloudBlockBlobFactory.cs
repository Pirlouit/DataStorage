using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace DataStorage.AzureBlob
{
    public class CloudBlockBlobFactory
    {
        public static CloudBlockBlob Create(string connectionString, string containerName, string fileName)
        {
            if(string.IsNullOrEmpty(connectionString))
                throw new NullReferenceException("ConnectionString cannot be null or empty.");
            if(string.IsNullOrEmpty(containerName))
                throw new NullReferenceException("ContainerName cannot be null or empty.");
            if(string.IsNullOrEmpty(fileName))
                throw new NullReferenceException("FileName cannot be null or empty.");

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference(containerName);
            // Create container if it does not exists
            container.CreateIfNotExists();
            // Connect to the blob file
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);             

            return blob;
        }
    }
}