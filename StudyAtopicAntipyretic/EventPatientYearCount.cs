using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyAtopicAntipyretic
{
    class EventPatientYearCount
    {
        Dictionary<int, int> eventCount;
        Dictionary<int, double> patientYearCount;
        public EventPatientYearCount()
        {
            for (int i = 0; i < 100; i++)
            {
                eventCount.Add(i, 0);
                patientYearCount.Add(i, 0);
            }
        }

        public void addPatientYear(double startAge, double endAge)
        {
            int startAgeRoundUp = Convert.ToInt32(Math.Ceiling(startAge));
            double startSideRemain = startAgeRoundUp - startAge;

            int endAgeRoundDown = Convert.ToInt32(Math.Floor(endAge));
            double endSideRemain = startAgeRoundUp - startAge;

            for (int i = startAgeRoundUp; i <= endAgeRoundDown; i++)
            {
                patientYearCount[i] += 1;
            }
            patientYearCount[startAgeRoundUp - 1] += startSideRemain;

            patientYearCount[endAgeRoundDown + 1] += endSideRemain;
        }

        public void addEvent(int eventAge)
        {
            patientYearCount[Math.Max(99, eventAge)]++;
        }

        public int getEventCount(int age)
        {
            return eventCount[age];
        }
        public double getPatientYearCount(int age)
        {
            return patientYearCount[age];
        }
    }

}
