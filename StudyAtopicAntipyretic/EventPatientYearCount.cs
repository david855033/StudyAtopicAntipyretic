using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyAtopicAntipyretic
{
    class EventPatientYearCount
    {
        Dictionary<int, int> eventCount = new Dictionary<int, int>();
        Dictionary<int, double> patientYearCount = new Dictionary<int, double>();
        Dictionary<int, double> patientYearCountDiseaseFree = new Dictionary<int, double>();
        public EventPatientYearCount()
        {
            for (int i = 0; i < 100; i++)
            {
                eventCount.Add(i, 0);
                patientYearCount.Add(i, 0);
                patientYearCountDiseaseFree.Add(i, 0);
            }
        }

        public int getInRange(int input)
        {
            return Math.Max(0, Math.Min(99, input));
        }

        public void addPatientYear(double startAge, double endAge)
        {
            int startAgeRoundUp = Convert.ToInt32(Math.Ceiling(startAge));
            double startSideRemain = startAgeRoundUp - startAge;

            int endAgeRoundDown = Convert.ToInt32(Math.Floor(endAge));
            double endSideRemain = startAgeRoundUp - startAge;

            for (int i = startAgeRoundUp; i <= endAgeRoundDown; i++)
            {
                patientYearCount[getInRange(i)] += 1;
            }
            patientYearCount[getInRange(startAgeRoundUp - 1)] += startSideRemain;

            patientYearCount[getInRange(endAgeRoundDown + 1)] += endSideRemain;
        }

        public void addPatientYearDiseaseFree(double startAge, double endAge)
        {
            int startAgeRoundUp = Convert.ToInt32(Math.Ceiling(startAge));
            double startSideRemain = startAgeRoundUp - startAge;

            int endAgeRoundDown = Convert.ToInt32(Math.Floor(endAge));
            double endSideRemain = startAgeRoundUp - startAge;

            for (int i = startAgeRoundUp; i <= endAgeRoundDown; i++)
            {
                patientYearCountDiseaseFree[getInRange(i)] += 1;
            }
            patientYearCountDiseaseFree[getInRange(startAgeRoundUp - 1)] += startSideRemain;

            patientYearCountDiseaseFree[getInRange(endAgeRoundDown + 1)] += endSideRemain;
        }

        public void addEvent(double eventAge)
        {
            eventCount[getInRange(Convert.ToInt32(Math.Round(eventAge, 0)))]++;
        }

        public int getEventCount(int age)
        {
            return eventCount[age];
        }
        public double getPatientYearCount(int age)
        {
            return patientYearCount[age];
        }
        public double getPatientYearCountDiseaseFree(int age)
        {
            return patientYearCountDiseaseFree[age];
        }
        public void countPatient(PatientData patient)
        {
            addPatientYear(patient.startAge, patient.endAge);
            addPatientYearDiseaseFree(patient.startAge, Math.Min(patient.endAge, patient.eventAge));
            if (patient.hasEvent && patient.endAge >= patient.eventAge)
            {
                addEvent(patient.eventAge);
            }
        }
    }

}
