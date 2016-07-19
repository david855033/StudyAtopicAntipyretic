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
                table1_populationCount();


                Console.WriteLine("End of Program.");
            }

        }

        static void writeBar()
        {
            Console.WriteLine("".PadLeft(30, '='));
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
            var PBD_AllYoungerThan1Years = DataReader.LoadData(AllYoungerThan1YearFiles);
            writeBar();

            string AnyipyreticsFolder = @"D:\NHIRD\STEP9 小於一歲 有用過退燒藥的 PBD";
            var AnyipyreticsFiles = Directory.EnumerateFiles(AnyipyreticsFolder, "*All*");
            var PBD_Anyipyretics = DataReader.LoadData(AnyipyreticsFiles);
            writeBar();

            string test_title;
            test_title = "沒有使用退燒藥";
            SelectIDSandPBD(IDData, PBD_AllYoungerThan1Years, PBD_Anyipyretics, test_title);

            var AcetaminophenCriteria = new Criteria("[order]Aceteminopen-最早日期", "", PBD_Anyipyretics.indexTable);
            var IbuprofenCriteria = new Criteria("[order]Ibuprofen-最早日期", "", PBD_Anyipyretics.indexTable);
            var DiclofenicCriteria = new Criteria("[order]Diclofenic-最早日期", "", PBD_Anyipyretics.indexTable);

            test_title = "有使用過Aceteminopen";
            Console.WriteLine(test_title);
            var PBD_Acetaminophen = PBD_Anyipyretics.select(new List<Criteria>(), AcetaminophenCriteria);
            var IDS_Acetaminophen = IDData.selectIn(PBD_Anyipyretics, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS_Acetaminophen.ExportData(outputDir + $@"\{test_title}IDS.txt");
            var PBDALL_Acetaminophen = PBD_AllYoungerThan1Years.selectIn(PBD_Anyipyretics, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBDALL_Acetaminophen.ExportData(outputDir + $@"\{test_title}PBDALL.txt");

            writeBar();
            Console.WriteLine("有使用過Ibuprofen");
            var PBD_Ibuprofen = PBD_Anyipyretics.select(new List<Criteria>(), IbuprofenCriteria);

            Console.WriteLine("有使用過Diclofenic");
            var PBD_Diclofenic = PBD_Anyipyretics.select(new List<Criteria>(), DiclofenicCriteria);

            Console.WriteLine("有使用過Aceteminopen+Ibuprofen");
            var PBD_Aceta_Ibu = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria });

            Console.WriteLine("有使用過Aceteminopen+Diclo");
            var PBD_Aceta_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, DiclofenicCriteria });

            Console.WriteLine("有使用過 Ibuprofen+Diclo");
            var PBD_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { DiclofenicCriteria, IbuprofenCriteria });

            Console.WriteLine("有使用過 Aceteminopen+Ibuprofen+Diclo");
            var PBD_Aceta_Ibu_Diclo = PBD_Anyipyretics.select(new List<Criteria>(), new List<Criteria>() { AcetaminophenCriteria, IbuprofenCriteria, DiclofenicCriteria });
        }

        private static void SelectIDSandPBD(DataSet IDData, DataSet PBD_AllYoungerThan1Years, DataSet PBD_Anyipyretics, string test_title)
        {
            Console.WriteLine(test_title);
            var IDS = IDData.selectNotIn(PBD_Anyipyretics, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            IDS.ExportData(outputDir + $@"\{test_title}IDS.txt");
            var PBDALL = PBD_AllYoungerThan1Years.selectIn(IDS, new string[] { "ID", "Birthday" }, new string[] { "ID", "Birthday" });
            PBDALL.ExportData(outputDir + $@"\{test_title}PBDALL.txt");
            writeBar();
        }
    }
}

