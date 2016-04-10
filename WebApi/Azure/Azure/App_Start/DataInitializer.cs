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
    public class DataInitializer : CreateDatabaseIfNotExists<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            List<DiagnosisCode> diagnoses = getDiagnoses();
            List<ProcedureCode> procedures = getProcedures();
            using (var db = new DataContext())
            {
                db.DiagnosisCodes.AddRange(diagnoses);
                db.ProcedureCodes.AddRange(procedures);
                db.SaveChanges();
            }

            base.Seed(context);
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
