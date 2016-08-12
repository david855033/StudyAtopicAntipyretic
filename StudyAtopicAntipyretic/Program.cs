using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataSetLibrary;

namespace StudyAtopicAntipyretic
{
    public static class tool
    {
        static readonly string DATA_END_DATE = "2013-12-31";
        static public double getAge(this string theDate, string Birthday)
        {
            if (theDate == "0001-01-01") theDate = DATA_END_DATE;
            return Convert.ToDateTime(theDate).Subtract(Convert.ToDateTime(Birthday)).TotalDays / 365.25;
        }
        static public double round(this double input, int digi)
        {
            return Math.Round(input, digi);
        }
    }

    class Program
    {
        static string outputDir = @"D:\NHIRD";
        static void Main(string[] args)
        {
            using (var sw = new StreamWriter(outputDir + @"\Log.txt"))
            {
                Console.SetOut(sw);
                loadData();
                loadDrugGroupsIDS();
                loadAtopicIDS();
                loadDrugGroupsBPDAtopic();
                //table1_populationCount();
                //table2_atopicCount();
                table3_SIR();
                Console.WriteLine("End of Program.");
            }
        }

        static Dictionary<string, DataSet> groupDrugIDS = new Dictionary<string, DataSet>();
        static Dictionary<string, DataSet> groupDrugPBDAtopic = new Dictionary<string, DataSet>();
        static Dictionary<string, DataSet> groupAtopicIDS = new Dictionary<string, DataSet>();
        static DataSet IDData, ALLPBD, PBD_Anyipyretics, PBD_Atopic;
        static DataSet PBD_AR_joined, PBD_Asthma_Joined, PBD_AD_joined;
        static void loadData()
        {
            string IDFileFolderPath = @"D:\NHIRD\STEP7 出生年2000-2013之ID標準化";
            var IDFiles = Directory.EnumerateFiles(IDFileFolderPath);
            IDData = DataReader.LoadData(IDFiles);
            writeBar();

            string AllYoungerThan1YearFolderPath = @"D:\NHIRD\STEP8 小於一歲所有CD_DD之PBD";
            var AllYoungerThan1YearFiles = Directory.EnumerateFiles(AllYoungerThan1YearFolderPath, "*All*");
            ALLPBD = DataReader.LoadData(AllYoungerThan1YearFiles);
            writeBar();

            string AnyipyreticsFolder = @"D:\NHIRD\STEP9 小於一歲 有用過退燒藥的 PBD";
            var AnyipyreticsFiles = Directory.EnumerateFiles(AnyipyreticsFolder, "*All*");
            PBD_Anyipyretics = DataReader.LoadData(AnyipyreticsFiles);
            writeBar();

            string AtopicFolder = @"D:\NHIRD\STEP10 小於15歲 過敏診斷以及用氣喘藥的PBD";
            var AtopicFiles = Directory.EnumerateFiles(AtopicFolder, "*All*");
            PBD_Atopic = DataReader.LoadData(AtopicFiles);
            writeBar();
        }
        static void loadDrugGroupsIDS()
        {
            string test_title = "有使用退燒藥";
            groupDrugIDS.Add(test_title,
                IDData.selectIn(PBD_Anyipyretics, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "沒有使用退燒藥";
            groupDrugIDS.Add(test_title,
               IDData.selectNotIn(PBD_Anyipyretics, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            var AcetaminophenCriteria = new Criteria("[order]Aceteminopen-最早日期", "", PBD_Anyipyretics.indexTable);
            var IbuprofenCriteria = new Criteria("[order]Ibuprofen-最早日期", "", PBD_Anyipyretics.indexTable);
            var DiclofenicCriteria = new Criteria("[order]Diclofenic-最早日期", "", PBD_Anyipyretics.indexTable);

            test_title = "有使用過Aceteminopen";
            var PBD_Acetaminophen = PBD_Anyipyretics.select(new List<Criteria>(), AcetaminophenCriteria);
            groupDrugIDS.Add(test_title,
                IDData.selectIn(PBD_Acetaminophen, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "有使用過Ibuprofen";
            var PBD_Ibuprofen = PBD_Anyipyretics.select(new List<Criteria>(), IbuprofenCriteria);
            groupDrugIDS.Add(test_title,
                IDData.selectIn(PBD_Ibuprofen, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "有使用過Diclofenic";
            var PBD_Diclofenic = PBD_Anyipyretics.select(new List<Criteria>(), DiclofenicCriteria);
            groupDrugIDS.Add(test_title,
                IDData.selectIn(PBD_Diclofenic, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "有使用過Aceteminopen+Ibuprofen";
            var PBD_Aceta_Ibu = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria });
            groupDrugIDS.Add(test_title,
               IDData.selectIn(PBD_Aceta_Ibu, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );


            test_title = "有使用過Aceteminopen+Diclo";
            var PBD_Aceta_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, DiclofenicCriteria });
            groupDrugIDS.Add(test_title,
               IDData.selectIn(PBD_Aceta_Diclo, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );


            test_title = "有使用過 Ibuprofen+Diclo";
            var PBD_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { DiclofenicCriteria, IbuprofenCriteria });
            groupDrugIDS.Add(test_title,
               IDData.selectIn(PBD_Ibu_Diclo, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );


            test_title = "有使用過 Aceteminopen+Ibuprofen+Diclo";
            var PBD_Aceta_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria, DiclofenicCriteria });
            groupDrugIDS.Add(test_title,
               IDData.selectIn(PBD_Aceta_Ibu_Diclo, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );
        }
        static void loadDrugGroupsBPDAtopic()
        {
            string test_title = "有使用退燒藥";
            groupDrugPBDAtopic.Add(test_title,
                PBD_Atopic.selectIn(PBD_Anyipyretics, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "沒有使用退燒藥";
            groupDrugPBDAtopic.Add(test_title,
               PBD_Atopic.selectNotIn(PBD_Anyipyretics, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            var AcetaminophenCriteria = new Criteria("[order]Aceteminopen-最早日期", "", PBD_Anyipyretics.indexTable);
            var IbuprofenCriteria = new Criteria("[order]Ibuprofen-最早日期", "", PBD_Anyipyretics.indexTable);
            var DiclofenicCriteria = new Criteria("[order]Diclofenic-最早日期", "", PBD_Anyipyretics.indexTable);

            test_title = "有使用過Aceteminopen";
            var PBD_Acetaminophen = PBD_Anyipyretics.select(new List<Criteria>(), AcetaminophenCriteria);
            groupDrugPBDAtopic.Add(test_title,
                PBD_Atopic.selectIn(PBD_Acetaminophen, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "有使用過Ibuprofen";
            var PBD_Ibuprofen = PBD_Anyipyretics.select(new List<Criteria>(), IbuprofenCriteria);
            groupDrugPBDAtopic.Add(test_title,
                PBD_Atopic.selectIn(PBD_Ibuprofen, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "有使用過Diclofenic";
            var PBD_Diclofenic = PBD_Anyipyretics.select(new List<Criteria>(), DiclofenicCriteria);
            groupDrugPBDAtopic.Add(test_title,
                PBD_Atopic.selectIn(PBD_Diclofenic, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            test_title = "有使用過Aceteminopen+Ibuprofen";
            var PBD_Aceta_Ibu = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria });
            groupDrugPBDAtopic.Add(test_title,
               PBD_Atopic.selectIn(PBD_Aceta_Ibu, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );


            test_title = "有使用過Aceteminopen+Diclo";
            var PBD_Aceta_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, DiclofenicCriteria });
            groupDrugPBDAtopic.Add(test_title,
               PBD_Atopic.selectIn(PBD_Aceta_Diclo, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );


            test_title = "有使用過 Ibuprofen+Diclo";
            var PBD_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { DiclofenicCriteria, IbuprofenCriteria });
            groupDrugPBDAtopic.Add(test_title,
               PBD_Atopic.selectIn(PBD_Ibu_Diclo, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );


            test_title = "有使用過 Aceteminopen+Ibuprofen+Diclo";
            var PBD_Aceta_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria, DiclofenicCriteria });
            groupDrugPBDAtopic.Add(test_title,
               PBD_Atopic.selectIn(PBD_Aceta_Ibu_Diclo, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );
        }
        static void loadAtopicIDS()
        {
            var Criteria_AR_OPD_0or1 = new List<Criteria>();
            Criteria_AR_OPD_0or1.Add(new Criteria("Allergic Rhinitis-門診次數", "0", PBD_Atopic.indexTable));
            Criteria_AR_OPD_0or1.Add(new Criteria("Allergic Rhinitis-門診次數", "1", PBD_Atopic.indexTable));
            var Criteria_NoAR_Admission = new List<Criteria>();
            Criteria_NoAR_Admission.Add(new Criteria("Allergic Rhinitis-住院次數", "0", PBD_Atopic.indexTable));
            var PBD_AR_hasMoreThan1OPD = PBD_Atopic.select(new List<Criteria>(), Criteria_AR_OPD_0or1);
            var PBD_AR_hasAdmission = PBD_Atopic.select(new List<Criteria>(), Criteria_NoAR_Admission);
            var PBD_AR_joined = PBD_AR_hasMoreThan1OPD.joinData(PBD_AR_hasAdmission);
            groupAtopicIDS.Add("Allergic Rhinitis",
                IDData.selectIn(PBD_AR_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            var Criteria_AD_OPD_0or1 = new List<Criteria>();
            Criteria_AD_OPD_0or1.Add(new Criteria("Atopic Dermatitis-門診次數", "0", PBD_Atopic.indexTable));
            Criteria_AD_OPD_0or1.Add(new Criteria("Atopic Dermatitis-門診次數", "1", PBD_Atopic.indexTable));
            var Criteria_NoAD_Admission = new List<Criteria>();
            Criteria_NoAD_Admission.Add(new Criteria("Atopic Dermatitis-住院次數", "0", PBD_Atopic.indexTable));
            var PBD_AD_hasOPD = PBD_Atopic.select(new List<Criteria>(), Criteria_AD_OPD_0or1);
            var PBD_AD_hasAdmission = PBD_Atopic.select(new List<Criteria>(), Criteria_NoAD_Admission);
            var PBD_AD_joined = PBD_AD_hasOPD.joinData(PBD_AD_hasAdmission);
            groupAtopicIDS.Add("Atopic Dermatitis",
                IDData.selectIn(PBD_AD_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );

            var Criteria_Asthma_OPD_0or1 = new List<Criteria>();
            Criteria_Asthma_OPD_0or1.Add(new Criteria("Asthma-門診次數", "0", PBD_Atopic.indexTable));
            Criteria_Asthma_OPD_0or1.Add(new Criteria("Asthma-門診次數", "1", PBD_Atopic.indexTable));
            var PBD_Asthma_Has_OPD = PBD_Atopic.select(new List<Criteria>(), Criteria_Asthma_OPD_0or1);
            var Criteria_NoAsthma_Drug = new List<Criteria>();
            Criteria_NoAsthma_Drug.Add(new Criteria("[order]AsthmaDrug-門診次數", "0", PBD_Atopic.indexTable));
            var PBD_Asthma_Has_Drug = PBD_Atopic.select(new List<Criteria>(), Criteria_NoAsthma_Drug);
            var PBD_Asthma_HasOPD_and_Drug = PBD_Asthma_Has_OPD.selectIn(PBD_Asthma_Has_Drug, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });

            var Criteria_Ashtma_NoAdmission = new List<Criteria>();
            Criteria_Ashtma_NoAdmission.Add(new Criteria("Asthma-住院次數", "0", PBD_Atopic.indexTable));
            var PBD_Asthma_Has_Admission = PBD_Atopic.select(new List<Criteria>(), Criteria_Ashtma_NoAdmission);

            var PBD_Asthma_Joined = PBD_Asthma_Has_OPD.joinData(PBD_Asthma_Has_Admission);
            groupAtopicIDS.Add("Asthma",
                IDData.selectIn(PBD_Asthma_Joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
            );


            groupAtopicIDS.Add("Non Atopic Disease",
               IDData.selectNotIn(PBD_AR_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
               .selectNotIn(PBD_AD_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
               .selectNotIn(PBD_Asthma_Joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
           );

        }


        static void table3_SIR()
        {
            string group, outcome;
            group = "有使用退燒藥";
            outcome = "Allergic Rhinitis";
            var set = getASI(groupDrugIDS[group], groupDrugPBDAtopic[group], outcome);
            set.printASI(outputDir + $@"\ASISet_{group}_{outcome}.txt");
        }

        private static EventPatientYearSet getASI(DataSet IDS, DataSet ALLPBD, string titleOfStudy)
        {
            DistinctList<PatientData> patientDataList = new DistinctList<PatientData>();

            EventPatientYearSet eventPatientYearSet = new EventPatientYearSet();

            foreach (var currentIDS in IDS.dataRow)
            {
                string ID = currentIDS[IDS.getIndex("ID")];
                string Birthday = currentIDS[IDS.getIndex("Birthday")] + "-01";
                string gender = currentIDS[IDS.getIndex("Gender")];
                string startDate = currentIDS[IDS.getIndex("firstInDate")];
                string endDate = currentIDS[IDS.getIndex("lastOutDate")];
                double startAge = startDate.getAge(Birthday);
                double endAge = endDate.getAge(Birthday);
                PatientData newPatientData = new PatientData(ID, Birthday);
                newPatientData.Gender = gender;
                newPatientData.startAge = startAge;
                newPatientData.endAge = endAge;
                patientDataList.addDistinct(newPatientData);
            }

            foreach (var currentPBD in
                PBD_Atopic.selectIn(groupAtopicIDS[titleOfStudy], new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" }).dataRow)
            {
                string ID = currentPBD[PBD_Atopic.getIndex("ID")];
                string Birthday = currentPBD[PBD_Atopic.getIndex("Birthday")] + "-01";
                string eventDate = currentPBD[PBD_Atopic.getIndex(titleOfStudy + "-最早日期")];
                var PatientToCompare = new PatientData(ID, Birthday);
                var index = patientDataList.BinarySearch(PatientToCompare);
                if (index >= 0)
                {
                    patientDataList[index].hasEvent = true;
                    patientDataList[index].eventAge = eventDate.getAge(Birthday);
                }
            }

            foreach (var patient in patientDataList)
            {
                eventPatientYearSet.totalEventPatientYear.countPatient(patient);
            }

            return eventPatientYearSet;
        }

        static void table1_populationCount()
        {
            Console.WriteLine("table1_populationCount");


            string IDFileFolderPath = @"D:\NHIRD\STEP7 出生年2000-2013之ID標準化";
            var IDFiles = Directory.EnumerateFiles(IDFileFolderPath);
            var IDData = DataReader.LoadData(IDFiles);
            writeBar();

            string AllYoungerThan1YearFolderPath = @"D:\NHIRD\STEP8 小於一歲所有CD_DD之PBD";
            var AllYoungerThan1YearFiles = Directory.EnumerateFiles(AllYoungerThan1YearFolderPath, "*All*");
            var ALLPBD = DataReader.LoadData(AllYoungerThan1YearFiles);
            writeBar();

            string AnyipyreticsFolder = @"D:\NHIRD\STEP9 小於一歲 有用過退燒藥的 PBD";
            var AnyipyreticsFiles = Directory.EnumerateFiles(AnyipyreticsFolder, "*All*");
            var PBD_Anyipyretics = DataReader.LoadData(AnyipyreticsFiles);
            writeBar();


            string test_title;
            test_title = "有使用退燒藥";
            SelectIDSandPBD(IDData, ALLPBD, PBD_Anyipyretics, test_title);

            test_title = "沒有使用退燒藥";
            SelectIDSandPBDNotIn(IDData, ALLPBD, PBD_Anyipyretics, test_title);


            var AcetaminophenCriteria = new Criteria("[order]Aceteminopen-最早日期", "", PBD_Anyipyretics.indexTable);
            var IbuprofenCriteria = new Criteria("[order]Ibuprofen-最早日期", "", PBD_Anyipyretics.indexTable);
            var DiclofenicCriteria = new Criteria("[order]Diclofenic-最早日期", "", PBD_Anyipyretics.indexTable);

            test_title = "有使用過Aceteminopen";
            Console.WriteLine(test_title);
            var PBD_Acetaminophen = PBD_Anyipyretics.select(new List<Criteria>(), AcetaminophenCriteria);
            SelectIDSandPBD(IDData, ALLPBD, PBD_Acetaminophen, test_title);

            test_title = "有使用過Ibuprofen";
            var PBD_Ibuprofen = PBD_Anyipyretics.select(new List<Criteria>(), IbuprofenCriteria);
            SelectIDSandPBD(IDData, ALLPBD, PBD_Ibuprofen, test_title);

            test_title = "有使用過Diclofenic";
            var PBD_Diclofenic = PBD_Anyipyretics.select(new List<Criteria>(), DiclofenicCriteria);
            SelectIDSandPBD(IDData, ALLPBD, PBD_Diclofenic, test_title);

            test_title = "有使用過Aceteminopen+Ibuprofen";
            var PBD_Aceta_Ibu = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria });
            SelectIDSandPBD(IDData, ALLPBD, PBD_Aceta_Ibu, test_title);

            test_title = "有使用過Aceteminopen+Diclo";
            var PBD_Aceta_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, DiclofenicCriteria });
            SelectIDSandPBD(IDData, ALLPBD, PBD_Aceta_Diclo, test_title);

            test_title = "有使用過 Ibuprofen+Diclo";
            var PBD_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { DiclofenicCriteria, IbuprofenCriteria });
            SelectIDSandPBD(IDData, ALLPBD, PBD_Ibu_Diclo, test_title);

            test_title = "有使用過 Aceteminopen+Ibuprofen+Diclo";
            var PBD_Aceta_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria, DiclofenicCriteria });
            SelectIDSandPBD(IDData, ALLPBD, PBD_Aceta_Ibu_Diclo, test_title);
        }
        static void table2_atopicCount()
        {
            Console.WriteLine("table2_atopicCount");
            string test_title;

            test_title = "Allergic rhinitis過敏性鼻炎";
            var IDS_AR_final = IDData.selectIn(PBD_AR_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS_AR_final.ExportData(outputDir + $@"\{test_title}_IDS.txt");
            var PBD_AR_final = ALLPBD.selectIn(IDS_AR_final, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBD_AR_final.ExportData(outputDir + $@"\{test_title}_BPD.txt");

            test_title = "Atopic Dermatitis異位性皮膚炎";
            var IDS_AD_final = IDData.selectIn(PBD_AD_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS_AD_final.ExportData(outputDir + $@"\{test_title}_IDS.txt");
            var PBD_AD_final = ALLPBD.selectIn(IDS_AD_final, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBD_AD_final.ExportData(outputDir + $@"\{test_title}_BPD.txt");

            test_title = "Asthma氣喘";
            var IDS_Asthma_final = IDData.selectIn(PBD_Asthma_Joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS_Asthma_final.ExportData(outputDir + $@"\{test_title}_IDS.txt");
            var PBD_Asthma_final = ALLPBD.selectIn(PBD_Asthma_Joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBD_Asthma_final.ExportData(outputDir + $@"\{test_title}_BPD.txt");

            test_title = "Non-Atopic";
            var IDS_NonAtopic_final = IDData.selectNotIn(PBD_Asthma_Joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
                                            .selectNotIn(PBD_AD_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
                                            .selectNotIn(PBD_AR_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS_NonAtopic_final.ExportData(outputDir + $@"\{test_title}_IDS.txt");
            var PBD_NonAtopic_final = ALLPBD.selectNotIn(PBD_Asthma_Joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
                                            .selectNotIn(PBD_AD_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" })
                                            .selectNotIn(PBD_AR_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBD_NonAtopic_final.ExportData(outputDir + $@"\{test_title}_BPD.txt");


        }
        private static void SelectIDSandPBD(DataSet IDData, DataSet PBD_AllYoungerThan1Years, DataSet PBD_selectedGroup, string test_title)
        {
            Console.WriteLine(test_title);

            var IDS = IDData.selectIn(PBD_selectedGroup, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS.ExportData(outputDir + $@"\{test_title}IDS.txt");

            var PBDALL = PBD_AllYoungerThan1Years.selectIn(IDS, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBDALL.ExportData(outputDir + $@"\{test_title}PBDALL.txt");
            writeBar();
        }
        private static void SelectIDSandPBDNotIn(DataSet IDData, DataSet PBD_AllYoungerThan1Years, DataSet PBD_selectedGroup, string test_title)
        {
            Console.WriteLine(test_title);

            var IDS = IDData.selectNotIn(PBD_selectedGroup, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS.ExportData(outputDir + $@"\{test_title}IDS.txt");

            var PBDALL = PBD_AllYoungerThan1Years.selectIn(IDS, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBDALL.ExportData(outputDir + $@"\{test_title}PBDALL.txt");
            writeBar();
        }

        static void writeBar()
        {
            Console.WriteLine("".PadLeft(30, '='));
        }
    }

    class EventPatientYearSet
    {
        public EventPatientYearCount maleEventPatientYear = new EventPatientYearCount();
        public EventPatientYearCount femaleEventPatientYear = new EventPatientYearCount();
        public EventPatientYearCount totalEventPatientYear = new EventPatientYearCount();

        internal void printASI(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.Default))
            {
                var title = new StringBuilder();
                var toWrite = new StringBuilder();

                title.AppendLine("\tTotal\t\t\tMale\t\t\tFemale\t\t");
                title.AppendLine("age\tP-Y\tP-Y Disease Free\tEvent\tP-Y\tP-Y Disease Free\tEvent\tP-Y\tP-Y Disease Free\tEvent");


                for (int i = 0; i < 100; i++)
                {
                    toWrite.Append(i + "\t");
                    toWrite.Append(totalEventPatientYear.getPatientYearCount(i) + "\t");
                    toWrite.Append(totalEventPatientYear.getPatientYearCountDiseaseFree(i) + "\t");
                    toWrite.Append(totalEventPatientYear.getEventCount(i) + "\t");

                    toWrite.Append(maleEventPatientYear.getPatientYearCount(i) + "\t");
                    toWrite.Append(maleEventPatientYear.getPatientYearCountDiseaseFree(i) + "\t");
                    toWrite.Append(maleEventPatientYear.getEventCount(i) + "\t");

                    toWrite.Append(femaleEventPatientYear.getPatientYearCount(i) + "\t");
                    toWrite.Append(femaleEventPatientYear.getPatientYearCountDiseaseFree(i) + "\t");
                    toWrite.Append(femaleEventPatientYear.getEventCount(i));

                    toWrite.AppendLine();
                }
                sw.Write(title.ToString());
                sw.Write(toWrite.ToString());
            }
        }
    }

    class DistinctList<T> : List<T>
    {
        public void addDistinct(T toAdd)
        {
            int index = this.BinarySearch(toAdd);
            if (index < 0)
                this.Insert(~index, toAdd);
        }
    }

    class PatientData : IComparable
    {
        public string ID, Birthday, Gender;
        string IDBirhtday { get { return ID + Birthday; } }
        public double startAge, endAge, eventAge;
        public bool hasEvent;
        public PatientData(string ID, string birthday)
        {
            this.ID = ID; this.Birthday = birthday;
        }
        public int CompareTo(object obj)
        {
            PatientData that = obj as PatientData;
            return this.IDBirhtday.CompareTo(that.IDBirhtday);
        }

    }

}

