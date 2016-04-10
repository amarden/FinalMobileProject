using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class PatientChatLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientChatLogId { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public string message { get; set; }
        public DateTime Created { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Provider Provider { get; set; }
    }
}
