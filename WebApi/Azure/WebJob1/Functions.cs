using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System;
using WebJob1.EHRASsets;

namespace WebJob1
{
    public class Functions
    {
        // This function will be triggered based on the schedule you have set for this WebJob
        // This function will enqueue a message on an Azure Queue called queue
        [NoAutomaticTrigger]
        public static void ManualTrigger()
        {
            Console.WriteLine("I am HerE");
            var ehr = new EHR();
            ehr.PatientBiometricScan();
            Console.WriteLine("I am HerE");
        }
    }
}
