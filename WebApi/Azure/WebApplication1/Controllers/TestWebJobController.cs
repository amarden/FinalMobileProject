using Azure.DataObjects;
using Azure.EhrAssets;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Azure.Controllers
{
    public class TestWebJobController : ApiController
    {
        [HttpGet]
        public List<Patient> Get(int howMany)
        {
            EHR ehr = new EHR();
            ehr.CreateNewPatients(howMany);
            using (var db = new DataContext())
            {
                var patients = db.Patients
                    .Include("DiagnosisCode")
                    .Include("Biometrics")
                    .Include("PatientChatLogs")
                    .Include("PatientProviders")
                    .Include("PatientProcedures")
                    .Include("PatientImagings")
                    .Include("Biometrics")
                    .ToList();
                return patients;
            }
        }

        private string JobCredentials
        {
            get
            {
                string result = string.Empty;
                try
                {
                    // Retrieve the connection string from the website configuration
                    result = ConfigurationManager.ConnectionStrings["JobCredentials"].ConnectionString;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(
                        "JobCredentials connection string not defined. Define in the connection string for the site userName:userPWD. Obtain from the publisher profile. Ex $csci-e64-webjobs:adei3kskzkela=");
                    System.Diagnostics.Trace.WriteLine("Exception ex: ", ex.Message);
                }

                return result;
            }
        }

        [HttpGet]
        private async Task NotifyJob()
        {
            try
            {

                // Create the http client to invoke the job dynamically
                HttpClient client = new HttpClient();

                // Set the credentials
                // The credentials need to be added to the web site as a connection string setting
                // Key  : JobCredentials
                // Value: userName:userPassword
                // Example Value: $csci-e64-webjobs01:ADADEadkeaeiqdileisliTESeenalz1
                string encodedCredentials = "Basic " + Base64Encode(JobCredentials);

                // Add the header so the job can be dynamically invoked
                client.DefaultRequestHeaders.Add("Authorization", encodedCredentials);

                // Retrieve the public facing Url for the mobile service
                string webJobUrl =
                    "https://e64webjobsapidemo.scm.azurewebsites.net/api/triggeredwebjobs/WebJob1/run";

                // Invoke the job dynamically
                HttpResponseMessage x = await client.PostAsync(new Uri(webJobUrl), null);

                // If the request failed log an error
                if (!x.IsSuccessStatusCode)
                {
                    System.Diagnostics.Trace.WriteLine("Unable to request processing of work list");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Exception during NotifyJob: " + ex.Message);
            }
        }

        public static string Base64Encode(string unencodedString)
        {
            if (string.IsNullOrWhiteSpace(unencodedString))
            {
                return "";
            }

            byte[] unencodedBytes = System.Text.Encoding.UTF8.GetBytes(unencodedString);

            return System.Convert.ToBase64String(unencodedBytes);
        }

        [HttpGet]
        public string Get(string test)
        {
            return test;
        }
    }
}
