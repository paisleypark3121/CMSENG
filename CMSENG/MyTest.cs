using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BlobManager;
using System.Net;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace CMSENG
{
    public static class MyTest
    {
        [FunctionName("MyTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountcmsen927e;AccountKey=X63IKbAfC70fS/jYBJPyyK3ofSFdUeKSyTvCxLiAEgrOe9US+ylyez8KnDIrQDxGx6M9WHY5dF5D2FrSb5mhXQ==;EndpointSuffix=core.windows.net";
            log.LogInformation("MYTEST started");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //CloudBlobContainer blobContainerAAA = blobClient.GetContainerReference("uploader");
            //CloudBlockBlob cloudBlockBlob = blobContainerAAA.GetBlockBlobReference("1010.mp3");
            //if (cloudBlockBlob == null)
            //    throw new Exception("Cannot retrieve blob");

            var blobContainer = blobClient.GetContainerReference("uploader");
            var blob = blobContainer.GetBlobReference("1010.mp3");

            //await cloudBlockBlob.DownloadToFileAsync(target, FileMode.Create);

            //using (var fs = new FileStream(target, FileMode.Create))
            //{
            //    await cloudBlockBlob.DownloadToStreamAsync(fs);
            //}

            await blob.FetchAttributesAsync();
            long blobLengthRemaining = blob.Properties.Length;
            long blobLength = blob.Properties.Length;
            long startPosition = 0;

            int segmentSize = 1 * 1024 * 1024; //1 MB chunk

            WebRequest ftpRequest = WebRequest.Create("ftp://demo.wftpserver.com/upload/1011.mp3");
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.Credentials = new NetworkCredential("demo-user", "demo-user");

            using (Stream requestStream = ftpRequest.GetRequestStream())
            {
                do
                {
                    long blockSize = Math.Min(segmentSize, blobLengthRemaining);
                    byte[] blobContents = new byte[blockSize];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await blob.DownloadRangeToStreamAsync(ms, startPosition, blockSize);
                        ms.Position = 0;
                        ms.Read(blobContents, 0, blobContents.Length);

                        byte[] array=ms.ToArray();
                        requestStream.Write(array, 0, array.Length);

                        //ms.CopyTo(requestStream);
                    }
                    startPosition += blockSize;
                    blobLengthRemaining -= blockSize;
                    if (blobLength > 0)
                    {
                        decimal totalSize = Convert.ToDecimal(blobLength);
                        decimal downloaded = totalSize - Convert.ToDecimal(blobLengthRemaining);
                        decimal blobPercent = (downloaded / totalSize) * 100;
                        //worker.ReportProgress(Convert.ToInt32(blobPercent));
                    }
                }
                while (blobLengthRemaining > 0);
            }
            

            //using (WebClient client = new WebClient())
            //{
            //    // Use login credentails if required
            //    client.Credentials = new NetworkCredential("demo-user", "demo-user");
            //    do
            //    {
            //        long blockSize = Math.Min(segmentSize, blobLengthRemaining);
            //        byte[] blobContents = new byte[blockSize];
            //        using (MemoryStream ms = new MemoryStream())
            //        {
            //            await blob.DownloadRangeToStreamAsync(ms, startPosition, blockSize);
            //            ms.Position = 0;
            //            ms.Read(blobContents, 0, blobContents.Length);

            //            client.UploadData("ftp://demo.wftpserver.com/upload/1010.mp3", WebRequestMethods.Ftp.UploadFile, ms.ToArray());

            //            //using (FileStream fs = new FileStream(localFile, FileMode.OpenOrCreate))
            //            //{
            //            //    fs.Position = startPosition;
            //            //    fs.Write(blobContents, 0, blobContents.Length);
            //            //}
            //        }
            //        startPosition += blockSize;
            //        blobLengthRemaining -= blockSize;
            //        if (blobLength > 0)
            //        {
            //            decimal totalSize = Convert.ToDecimal(blobLength);
            //            decimal downloaded = totalSize - Convert.ToDecimal(blobLengthRemaining);
            //            decimal blobPercent = (downloaded / totalSize) * 100;
            //            //worker.ReportProgress(Convert.ToInt32(blobPercent));
            //        }
            //    }
            //    while (blobLengthRemaining > 0);
            //}


            log.LogInformation("MYTEST finished");

            return (ActionResult)new OkObjectResult($"Hello World");
        }
    }
}
