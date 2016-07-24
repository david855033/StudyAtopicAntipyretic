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
                table2_atopicCount();

                Console.WriteLine("End of Program.");
            }

        }

        static void writeBar()
        {
            Console.WriteLine("".PadLeft(30, '='));
        }

        static void table2_atopicCount()
        {
            Console.WriteLine("table2_atopicCount");

            string IDFileFolderPath = @"D:\NHIRD\STEP7 出生年2000-2013之ID標準化";
            var IDFiles = Directory.EnumerateFiles(IDFileFolderPath);
            var IDData = DataReader.LoadData(IDFiles);
            writeBar();

            string AtopicFolder = @"D:\NHIRD\STEP10 小於15歲 過敏診斷以及用氣喘藥的PBD";
            var AtopicFiles = Directory.EnumerateFiles(AtopicFolder, "*All*");
            var PBD_Atopic = DataReader.LoadData(AtopicFiles);
            writeBar();

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
    }
}

