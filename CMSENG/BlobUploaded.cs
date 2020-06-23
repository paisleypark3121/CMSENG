using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CMSENG
{
    public static class BlobUploaded
    {
        //[FunctionName("BlobUploaded")]
        //public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "DefaultEndpointsProtocol=https;AccountName=storageaccountcmsen927e;AccountKey=X63IKbAfC70fS/jYBJPyyK3ofSFdUeKSyTvCxLiAEgrOe9US+ylyez8KnDIrQDxGx6M9WHY5dF5D2FrSb5mhXQ==;EndpointSuffix=core.windows.net")]Stream myBlob, string name, ILogger log)
        //{
        //    log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        //}
    }
}
