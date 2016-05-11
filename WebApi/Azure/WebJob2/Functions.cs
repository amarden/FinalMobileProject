using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using WebJob1.EHRASsets;
using WebJob1.NotificationClass;
using static WebJob1.EHRASsets.EHR;

namespace WebJob1
{
    public class Functions
    {
        // This will be triggerd on demand
        [NoAutomaticTrigger]
        public async static void ManualTrigger()
        {
            //Update Biometrics
            var ehr = new EHR();
            ehr.PatientBiometricScan();

            //Send Notification
            string message = generateNotificationMessage(ehr.changes);
            Console.WriteLine("Message that was sent: " + message);
            // Windows 8.1 / Windows Phone 8.1
            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                            message + "</text></binding></visual></toast>";
            try
            {
                await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in sending Notification message: " + e.Message);
            }
            Console.WriteLine("Triggered Notification");
        }

        /// <summary>
        /// Generates the notification message
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static string generateNotificationMessage(List<PatientStatusChange> info)
        {
            int changes = info.Count();

            return changes + " patient(s) have new medical statuses";
        }
    }
}
