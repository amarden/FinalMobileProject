using Azure.DataObjects;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJob1.EHRASsets
{
    /// <summary>
    /// A class that tries to mock certain aspects of an EHR system. Primarily used generating fake biometrics for existing patients
    /// </summary>
    public class EHR
    {
        /// <summary>
        /// Class that will determine if a status has changed used for notifications and to keep track of what happened to a patient
        /// </summary>
        public class PatientStatusChange
        {
            public PatientStatusChange(string oldStatus, string newStatus)
            {
                if(oldStatus != newStatus)
                {
                    changeTo = newStatus;
                }
            }

            public string changeTo { get; set; }
        }

        //Stable ranges and critical ranges to help calculate medical status
        private Tuple<int, int> systolicStableRange = new Tuple<int, int>(80, 145);
        private Tuple<int, int> diastolicStableRange = new Tuple<int, int>(50, 90);
        private Tuple<int, int> glucoseStableRange = new Tuple<int, int>(50, 110);
        private Tuple<int, int> oxygenStableRange = new Tuple<int, int>(85, 100);
        private Tuple<int, int> systolicCriticalBounds = new Tuple<int, int>(60, 195);
        private Tuple<int, int> diastolicCrticalBounds = new Tuple<int, int>(20, 120);
        private Tuple<int, int> glucoseCriticalBounds = new Tuple<int, int>(20, 160);
        private int oxygenCriticalLowerBound = 65;
        public List<PatientStatusChange> changes = new List<PatientStatusChange>();
        //Random number generator used in several methods to generate fake data
        Random r = new Random();

        public EHR()
        {
        }

        /// <summary>
        /// Scans All patients and 25% of the time will generate new biometrics as long as the patient has had less than 10 measurements today
        /// </summary>
        public void PatientBiometricScan()
        {
            using (var db = new DataContext())
            {
                //only do this for patients that are not dead and are not discharged
                var patients = db.Patients.Include("Biometrics").Where(x=>x.MedicalStatus == "stable" || x.MedicalStatus == "critical");
                foreach(var p in patients)
                {
                    bool shouldMeasure = r.Next(0, 100) < 25 ? true : false;
                    if(shouldMeasure)
                    {
                        bool hasMaxMeasurements = doesPatientHaveMaxMeasurementsToday(p, 10);
                        if(!hasMaxMeasurements)
                        {
                            Biometric measure = generateBiometric(p);
                            var status = imputeStatus(p.Biometrics, measure);
                            measure.DeathModifier = status.Item2;
                            changes.Add(new PatientStatusChange(p.MedicalStatus, status.Item1));
                            p.MedicalStatus = status.Item1;
                            db.Entry(p).State = EntityState.Modified;
                            db.Biometrics.Add(measure);
                        }
                    } 
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Imputes status based on patient biometris and returns status and deathmodifier
        /// </summary>
        /// <param name="allMeasures"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        private Tuple<string, int> imputeStatus(IEnumerable<Biometric> allMeasures, Biometric measure)
        {
            int lengthOfStay = allMeasures.Count();
            int averageDeathModifier = lengthOfStay == 0 ? 0 : (int)Math.Round(allMeasures.Average(x => x.DeathModifier)/10);
            int minRandomModifider = -15 + averageDeathModifier;
            int pastDeathModifier = r.Next(minRandomModifider, 10);
            //status for each measurement can be stable, unstable, or critical
            string bpStatus = checkBpStatus(measure.Systolic, measure.Diastolic);
            string glucoseStatus = checkGlucoseStatus(measure.Glucose);
            string oxygenStatus = checkOxygenStatus(measure.Oxygen);

            //Calculate deathModifier that raises likelihood of patient death or critical status
            List<string> statuses = new List<string> { bpStatus, glucoseStatus, oxygenStatus };
            int criticalCount = statuses.Where(x => x == "critical").Count();
            int unstableCount = statuses.Where(x => x == "unstable").Count();
            int stableCount = statuses.Where(x => x == "stable").Count();
            int deathModifier = lengthOfStay + (criticalCount * 10) + (unstableCount * 2) + (-stableCount * 2) + pastDeathModifier;
            int chanceOfDeath = lengthOfStay == 0 ? 0 : deathModifier; //Cannot be dead on first measurement
            int randomInt = r.Next(0, 100);
            string patientStatus;
            if (randomInt < chanceOfDeath && chanceOfDeath > 0)
            {
                patientStatus = "death";
            }
            else if(randomInt - chanceOfDeath < 10)
            {
                patientStatus = "critical";
            }
            else
            {
                patientStatus = "stable";
            }
            return new Tuple<string, int>(patientStatus, deathModifier);

        }

        /// <summary>
        /// Takes oxygen number and determines whether the range is stable, unstable, or critical
        /// </summary>
        /// <param name="oxygen"></param>
        /// <returns>metric status</returns>
        private string checkOxygenStatus(int oxygen)
        {
            string status;
            if (this.oxygenStableRange.Item1 <= oxygen && this.oxygenStableRange.Item2 >= oxygen)
            {
                status = "stable";
            }
            else if (oxygen < this.oxygenCriticalLowerBound)
            {
                status = "critical";
            }
            else
            {
                status = "unstable";
            }
            return status;
        }

        /// <summary>
        /// Takes glucose number and determines whether the range is stable, unstable, or critical
        /// </summary>
        /// <param name="glucose"></param>
        /// <returns>metric status</returns>
        private string checkGlucoseStatus(int glucose)
        {
            string status;
            if (this.glucoseStableRange.Item1 <= glucose && this.glucoseStableRange.Item2 >= glucose)
            {
                status = "stable";
            }
            else if (glucose > this.glucoseCriticalBounds.Item2 || glucose < this.glucoseCriticalBounds.Item1)
            {
                status = "critical";
            }
            else
            {
                status = "unstable";
            }
            return status;
        }

        /// <summary>
        /// Takes BP numbers and determines whether the range is stable, unstable, or critical
        /// </summary>
        /// <param name="systolic"></param>
        /// <param name="diastolic"></param>
        /// <returns>metric status</returns>
        private string checkBpStatus(int systolic, int diastolic)
        {
            string status;
            if(this.systolicStableRange.Item1 <= systolic && this.systolicStableRange.Item2 >= systolic
                && this.diastolicStableRange.Item1 <= diastolic && this.diastolicStableRange.Item2 >= diastolic)
            {
                status = "stable";
            }
            else if(systolic > this.systolicCriticalBounds.Item2 || systolic < this.systolicCriticalBounds.Item1
                || diastolic > this.diastolicCrticalBounds.Item2 || diastolic < this.diastolicCrticalBounds.Item1)
            {
                status = "critical";
            }
            else
            {
                status = "unstable";
            }
            return status;
        }

        /// <summary>
        /// generates a new biometric reading for a patient
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        private Biometric generateBiometric(Patient patient)
        {
            Biometric newReading = new Biometric();
            var lastReading = patient.Biometrics.OrderByDescending(x => x.MeasurementDate).First();
            Tuple<int, int> systolicAndDiastolic = generateBloodPressure(patient.MedicalStatus, lastReading);
            newReading.Systolic = systolicAndDiastolic.Item1;
            newReading.Diastolic = systolicAndDiastolic.Item2;
            newReading.Oxygen = generateOxygen(patient.MedicalStatus, lastReading);
            newReading.Glucose = generateGlucose(patient.MedicalStatus, lastReading);
            newReading.MeasurementDate = DateTime.Now;
            newReading.PatientId = patient.PatientId;
            return newReading;
        }

        /// <summary>
        /// Generates glucose reading, will be more volatile if patient is critical
        /// </summary>
        /// <param name="status"></param>
        /// <param name="lastReading"></param>
        /// <returns>integer of representing glucose measurement</returns>
        private int generateGlucose(string status, Biometric lastReading)
        {
            int maxChange = status == "critical" ? 25 : 10;
            var percentChange = r.Next(-maxChange, maxChange) / 100f;
            int newGlucose = (int)Math.Round(lastReading.Glucose * percentChange + lastReading.Glucose);
            return newGlucose;
        }

        /// <summary>
        /// Generates glucose reading, will be more volatile if patient is critical
        /// </summary>
        /// <param name="status"></param>
        /// <param name="lastReading"></param>
        /// <returns>Tuple of integers of representing BP measurement</returns>
        private Tuple<int, int> generateBloodPressure(string status, Biometric lastReading)
        {
            int maxChange = status == "critical" ? 20 : 10;
            var percentChange = r.Next(-maxChange, maxChange) / 100f;
            int newSystolic = (int)Math.Round(lastReading.Systolic * percentChange + lastReading.Systolic);
            percentChange = r.Next(0, maxChange) / 100f;
            int newDiastolic = (int)Math.Round(lastReading.Diastolic * percentChange + lastReading.Diastolic);
            return new Tuple<int, int>(newSystolic, newDiastolic);
        }

        /// <summary>
        /// Generates glucose reading, will be more volatile if patient is critical
        /// </summary>
        /// <param name="status"></param>
        /// <param name="lastReading"></param>
        /// <returns>integer of representing oxygen measurement</returns>
        private int generateOxygen(string status, Biometric lastReading)
        {
            int maxChange = status == "critical" ? 10 : 5;
            var percentChange = r.Next(-maxChange, maxChange) / 100f;
            int newOxygen = (int)Math.Round(lastReading.Oxygen * percentChange + lastReading.Oxygen);
            newOxygen = Math.Min(newOxygen, 100);
            return newOxygen;
        }

        /// <summary>
        /// Takes patient and number which represents max number in a day patient can have readings
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="max"></param>
        /// <returns>boolean</returns>
        private bool doesPatientHaveMaxMeasurementsToday(Patient patient, int max)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var measurementsToday = patient.Biometrics.Where(x => today > x.MeasurementDate).Count();
            return measurementsToday == max;
        }
    }
}
