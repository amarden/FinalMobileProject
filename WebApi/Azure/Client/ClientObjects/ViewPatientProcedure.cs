using System;

namespace Client.ClientObjects
{
    public class ViewPatientProcedure
    {
        public int PatientProcedureId { get; set; }
        public string Procedure { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string CompletedBy { get; set; }
        public VisibilityRules ShowRules { get; set; }
    }

    public class VisibilityRules
    {
        public bool Completed { get; set; }
        public string userRole { get; set; }
        public string procedureRole { get; set; }
    }
}