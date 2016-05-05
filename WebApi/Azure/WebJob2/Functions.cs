using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using WebJob1.EHRASsets;

namespace WebJob2
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage()
        {
            var ehr = new EHR();
            ehr.PatientBiometricScan();
            Console.WriteLine("There were " + ehr.getPatientChangeNumber() + " patients changes");
        }
    }
}
