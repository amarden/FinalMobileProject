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
        private const string MY_CONTAINER = "patientimaging";
        private DataContext db = new DataContext();

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

        [HttpGet]
        public List<PatientImaging> GetByPatient(int patientId)
        {
            return db.PatientImagings.Where(x => x.PatientId == patientId).ToList();
        }

        [HttpGet]
        public byte[] GetImage(string blobId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(WebApiConfig.StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(MY_CONTAINER);

            // Retrieve reference to a blob named the blob specified by the caller
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobId);
            MemoryStream stream = new MemoryStream();

            blockBlob.DownloadToStream(stream);

            return stream.ToArray();
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
            CloudBlobContainer container = blobClient.GetContainerReference(MY_CONTAINER);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Set permissions on the blob container to prevent public access
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Retrieve reference to a blob named the blob specified by the caller
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(guid);
            blockBlob.UploadFromStream(new MemoryStream(image.ImageStream));
            db.SaveChanges();
        }
    }
}
