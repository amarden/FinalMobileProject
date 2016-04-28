using Azure;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class PatientImagingController : ApiController
    {
        private const string PUBLIC_CONTAINER = "patientimaging";
        private DataContext db = new DataContext();


        [HttpGet]
        public List<PatientImaging> GetByPatient(int patientId)
        {
            return db.PatientImagings.Where(x => x.PatientId == patientId).ToList();
        }

        [HttpGet]
        public string GetByPatient(string id)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(WebApiConfig.StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(PUBLIC_CONTAINER);

            // Retrieve reference to a blob named the blob specified by the caller
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);

            return blockBlob.DownloadText();
        }


        public class BlobName
        {
            /// <summary>
            /// Gets or sets the blob's  container.
            /// </summary>
            /// <value>The blob's container.</value>
            public string Container { get; set; }
            /// <summary>
            /// Gets or sets the blob name.
            /// </summary>
            /// <value>The blob's name.</value>
            public string Name { get; set; }
        }

        private static void GetBlobNames(CloudBlobContainer container, List<BlobName> blobNames)
        {
            if (container.Exists())
            {
                // Loop over items within the container and output the length and URI.
                foreach (var blob in container.ListBlobs(null, false))
                {
                    if (blob is CloudBlockBlob)
                    {
                        blobNames.Add(new BlobName() { Container = container.Name, Name = ((CloudBlockBlob)(blob)).Name });
                    }
                }
            }
        }

        public BlobName[] Get()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(WebApiConfig.StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            List<BlobName> blobNames = new List<BlobName>();

            // Get the blobs in the public container
            GetBlobNames(blobClient.GetContainerReference(PUBLIC_CONTAINER), blobNames);

            // Get the blobs in the secret container
            if (blobNames.Count > 0)
            {
                return blobNames.ToArray();
            }

            return null;
        }

        [HttpGet]
        public void fake(string fake)
        {
            string fileString = "C:\\FinalMobileProject\\WebApi\\Azure\\Client\\usertable.png";
            FileStream stream;
            using (FileStream SourceStream = File.Open(fileString, FileMode.Open, FileAccess.Read))
            {
                stream = SourceStream;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(WebApiConfig.StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(PUBLIC_CONTAINER);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // DEMO
            // Set permissions on the blob container to prevent public access
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off});

            // DEMO
            // Set permissions on the blob container to allow public access
            //container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            string guid = Guid.NewGuid().ToString();
             
            // Retrieve reference to a blob named the blob specified by the caller
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(guid);
            blockBlob.UploadFromStream(stream);
            }

            // Create or overwrite the blob with contents of the message provided
            //blockBlob.UploadFromStream(imageData);
        }

        [HttpPost]
        public void CreateImage(SubmitImage image)
        {
            string guid = Guid.NewGuid().ToString();
            PatientImaging pi = new PatientImaging();
            pi.PatientId = image.PatientId;
            pi.UploadDate = DateTime.Now;
            pi.ImageBlobId = guid;
            pi.ImageType = image.ImageType;

            db.PatientImagings.Add(pi);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(WebApiConfig.StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(PUBLIC_CONTAINER);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // DEMO
            // Set permissions on the blob container to prevent public access
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // DEMO
            // Set permissions on the blob container to allow public access
            //container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });


            // Retrieve reference to a blob named the blob specified by the caller
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(guid);
            blockBlob.UploadFromStream(new MemoryStream(image.ImageStream));
            db.SaveChanges();
        }
    }
}
