using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataSetLibrary;

namespace StudyAtopicAntipyretic
{
    class Program
    {
        static string outputDir = @"D:\NHIRD";
        static void Main(string[] args)
        {
            using (var sw = new StreamWriter(outputDir + @"\Log.txt"))
            {
                Console.SetOut(sw);
                //table1_populationCount();
                //table2_atopicCount();

                Console.WriteLine("End of Program.");
            }

        }



        static void table3()
        {

        }
      



        static void table2_atopicCount()
        {
            Console.WriteLine("table2_atopicCount");

            string IDFileFolderPath = @"D:\NHIRD\STEP7 出生年2000-2013之ID標準化";
            var IDFiles = Directory.EnumerateFiles(IDFileFolderPath);
            var IDData = DataReader.LoadData(IDFiles);
            writeBar();

            string AllYoungerThan1YearFolderPath = @"D:\NHIRD\STEP8 小於一歲所有CD_DD之PBD";
            var AllYoungerThan1YearFiles = Directory.EnumerateFiles(AllYoungerThan1YearFolderPath, "*All*");
            var ALLPBD = DataReader.LoadData(AllYoungerThan1YearFiles);
            writeBar();

            string AtopicFolder = @"D:\NHIRD\STEP10 小於15歲 過敏診斷以及用氣喘藥的PBD";
            var AtopicFiles = Directory.EnumerateFiles(AtopicFolder, "*All*");
            var PBD_Atopic = DataReader.LoadData(AtopicFiles);
            writeBar();

            string test_title;

            var Criteria_AR_OPD_0or1 = new List<Criteria>();
            Criteria_AR_OPD_0or1.Add(new Criteria("Allergic Rhinitis-門診次數", "0", PBD_Atopic.indexTable));
            Criteria_AR_OPD_0or1.Add(new Criteria("Allergic Rhinitis-門診次數", "1", PBD_Atopic.indexTable));
            var Criteria_NoAR_Admission = new List<Criteria>();
            Criteria_NoAR_Admission.Add(new Criteria("Allergic Rhinitis-住院次數", "0", PBD_Atopic.indexTable));
            var PBD_AR_hasMoreThan1OPD = PBD_Atopic.select(new List<Criteria>(), Criteria_AR_OPD_0or1);
            var PBD_AR_hasAdmission = PBD_Atopic.select(new List<Criteria>(), Criteria_NoAR_Admission);
            var PBD_AR_joined = PBD_AR_hasMoreThan1OPD.joinData(PBD_AR_hasAdmission);

            test_title = "Allergic rhinitis過敏性鼻炎";
            var IDS_AR_final = IDData.selectIn(PBD_AR_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS_AR_final.ExportData(outputDir + $@"\{test_title}_IDS.txt");
            var PBD_AR_final = ALLPBD.selectIn(IDS_AR_final, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBD_AR_final.ExportData(outputDir + $@"\{test_title}_BPD.txt");

            var Criteria_AD_OPD_0or1 = new List<Criteria>();
            Criteria_AD_OPD_0or1.Add(new Criteria("Atopic Dermatitis-門診次數", "0", PBD_Atopic.indexTable));
            Criteria_AD_OPD_0or1.Add(new Criteria("Atopic Dermatitis-門診次數", "1", PBD_Atopic.indexTable));
            var Criteria_NoAD_Admission = new List<Criteria>();
            Criteria_NoAD_Admission.Add(new Criteria("Atopic Dermatitis-住院次數", "0", PBD_Atopic.indexTable));
            var PBD_AD_hasOPD = PBD_Atopic.select(new List<Criteria>(), Criteria_AD_OPD_0or1);
            var PBD_AD_hasAdmission = PBD_Atopic.select(new List<Criteria>(), Criteria_NoAD_Admission);
            var PBD_AD_joined = PBD_AD_hasOPD.joinData(PBD_AD_hasAdmission);

            test_title = "Atopic Dermatitis異位性皮膚炎";
            var IDS_AD_final = IDData.selectIn(PBD_AD_joined, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS_AD_final.ExportData(outputDir + $@"\{test_title}_IDS.txt");
            var PBD_AD_final = ALLPBD.selectIn(IDS_AD_final, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBD_AD_final.ExportData(outputDir + $@"\{test_title}_BPD.txt");

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

}

