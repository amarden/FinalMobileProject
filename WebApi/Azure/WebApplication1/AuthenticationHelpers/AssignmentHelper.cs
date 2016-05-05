using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.AuthenticationHelpers
{
    public static class AssignmentHelper
    {
        public static bool IsAssignedToPatient(int patientId, int providerId)
        {
            using(DataContext db = new DataContext())
            {
                return db.PatientProviders.Any(x => x.PatientId == patientId && x.ProviderId == providerId); 
            }
        }
    }
}