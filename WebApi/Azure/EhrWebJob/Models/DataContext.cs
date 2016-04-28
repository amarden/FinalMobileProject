using Azure.DataObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Models
{
    public class DataContext : DbContext
    {

        public DataContext() : base("name=MS_TableConnectionString")
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Biometric>  Biometrics { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
             .HasMany(x => x.Biometrics)
             .WithRequired(x => x.Patient)
             .HasForeignKey(x => x.PatientId);
        }
    }
}
