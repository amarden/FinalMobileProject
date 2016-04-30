using Azure.DataObjects;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Azure.App_Start
{
    public class DataInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            List<DiagnosisCode> diagnoses = getDiagnoses();
            List<ProcedureCode> procedures = getProcedures();
            List<Provider> providers = getProviders();
            using (var db = new DataContext())
            {
                db.DiagnosisCodes.AddRange(diagnoses);
                db.ProcedureCodes.AddRange(procedures);
                db.Providers.AddRange(providers);
                db.SaveChanges();
            }

            base.Seed(context);
        }

        public List<Provider> getProviders()
        {
            var providers = new List<Provider>();
            string path = HttpContext.Current.Server.MapPath("~/StartingData/ProviderStartList.txt");
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(',');
                    var provider = new Provider();
                    provider.Name = data[0];
                    provider.Role = data[1];
                    providers.Add(provider);
                }
            }
            return providers;
        }

        public List<DiagnosisCode> getDiagnoses()
        {
            var codes = new List<DiagnosisCode>();
            string path = HttpContext.Current.Server.MapPath("~/StartingData/DiagnosisCodes.txt");
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var code = new DiagnosisCode();
                    code.Diagnosis = line;
                    codes.Add(code);
                }
            }
            return codes;
        }

        public List<ProcedureCode> getProcedures()
        {
            var codes = new List<ProcedureCode>();
            string path = HttpContext.Current.Server.MapPath("~/StartingData/ProcedureCodes.txt");
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] items = line.Split(',');
                    var code = new ProcedureCode();
                    code.Procedure = items[0];
                    code.Role = items[1];
                    codes.Add(code);
                }
                // Read the stream to a string, and write the string to the console.
            }
            return codes;
        }
    }
}
