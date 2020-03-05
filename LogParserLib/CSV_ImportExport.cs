using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LogParser
{
    public class CSV_ImportExport
    {
        public string path2File { get; set; }

        public static bool areFilesRaw(string[] astr)
        {
            return (astr[0].ToUpper().Contains(".CSV"));
        }

        public static string getOutputFileFullPath(string path2File)
        {
            string result = "";
            if (path2File != "")
            {
                var fDir = Path.GetDirectoryName(path2File);
                var fName = Path.GetFileNameWithoutExtension(path2File);
                var fExt = Path.GetExtension(path2File);
                var fFull = Path.Combine(fDir, fName + ".output" + ".xls");
                //var fFull = System.IO.Path.Combine(fDir, fName + ".xls");
                result = fFull;
            }
            return result;
        }

        /// <summary>
        /// возвращает полный путь выходного файла
        /// </summary>
        /// <returns></returns>
        public string getOutputFileFullPath()
        {
            return getOutputFileFullPath(path2File);
        }

        /// <summary>
        /// экспортирует данные в CSV
        /// </summary>
        /// <param name="toSaveData"></param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="toSaveData"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.OI.Exception"></exception>
        /// <returns>bool result</returns>
        public bool ExportResultToCSV(List<oneAnalyzeStructure> toSaveData)
        {
            return ExportResultToCSV(getOutputFileFullPath(), toSaveData);
        }

        /// <summary>
        /// парсит одну строку из CSV файла
        /// </summary>
        /// <param name="S">входная строка</param>
        /// <returns>распознанную структуру или null если строка <paramref name="S"/> не валидна </returns>
        protected static oneStringStructure ParseOneString(string S)
        {
            oneStringStructure result = null;

            var elems = S.Split(';');
            //if (elems.Length != 10) throw new MissingMemberException();

            // пробуем распарсить строку
            try
            {
                DateTime _dt;
                bool res = DateTime.TryParse(elems[0], out _dt);
                if (!res)
                    elems[0] = elems[0].Substring(0, elems[0].LastIndexOf(',')) + '.' + elems[0].Substring(elems[0].LastIndexOf(',') + 1, elems[0].Length - 1 - elems[0].LastIndexOf(','));
                /*
                try
                {
                    _dt = DateTime.Parse(elems[0]);
                }
                catch (FormatException) { }
                if (_dt == null)
                {
                    try
                    {
                        elems[0] = elems[0].Substring(0, elems[0].LastIndexOf(',')) + '.' + elems[0].Substring(elems[0].LastIndexOf(',') + 1, elems[0].Length - 1 - elems[0].LastIndexOf(','));
                    }
                    catch (Exception) { }
                }
                 */
                if ((elems[4].IndexOf('[') != -1) && (elems[4].IndexOf(']') != -1))
                    result = new oneStringStructure(
                        DateTime.Parse(elems[0]),
                        UInt16.Parse(elems[1]),
                        UInt16.Parse(elems[2]),
                        elems[3].ToString(),
                        elems[4],
                        double.Parse(elems[5]),
                        int.Parse(elems[4].Substring(elems[4].IndexOf('[') + 1, elems[4].IndexOf(']') - elems[4].IndexOf('[') - 1))
                        );
                else
                    result = new oneStringStructure(
                        DateTime.Parse(elems[0]),
                        UInt16.Parse(elems[1]),
                        UInt16.Parse(elems[2]),
                        elems[3].ToString(),
                        elems[4],
                        double.Parse(elems[5]),
                        -1
                        );
            }
            catch (ArgumentNullException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            catch (IndexOutOfRangeException) { }
            catch (ArgumentOutOfRangeException) { }

            return result;
        }

        //public List<oneStringStructure> parseCSV(string path)
        public static List<oneStringStructure> ImportRawFromCSV(string path)
        {
            List<oneStringStructure> result = null;
            try
            {
                using (StreamReader readFile = new StreamReader(path: path, encoding: Encoding.GetEncoding("UTF-8")))
                {
                    string line;

                    line = readFile.ReadLine(); // первая строка - расшифровка полей, пропускаем
                    result = new List<oneStringStructure>();

                    //while ((line = readFile.ReadLine()) != null)
                    int strCntr = 2;
                    while (!string.IsNullOrEmpty(line = readFile.ReadLine()))
                    {
                        var resf = ParseOneString(line);
                        if (resf != null)
                        {
                            result.Add(resf);
                        }
                        else
                        {
//                            string msg = string.Format("Ошибка парсинга строки № {0} в файле {1}", strCntr, System.IO.Path.GetFileNameWithoutExtension(path));
//                            MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                            return null;
                            throw new Import.RawCSVStringParseException(line, strCntr);
                        }
                        strCntr++;
                    }
                }
            }
            catch (System.IO.IOException)
            {
                throw; // передаём вверх
            }
            return result;
        }

        // format specific code

        protected bool ExportResultToCSV(string path, List<oneAnalyzeStructure> toSaveData)
        {
            if (null == toSaveData)
                return false;

            bool result = true;

            StringBuilder sb = new StringBuilder();
            /*
                        CultureInfo ci = new CultureInfo("ru-RU");
                        ci.NumberFormat.NumberDecimalSeparator = ","; // TODO: разобраться с импортом (разделитель)
            */
            sb.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                "Точка::штамп времени",
                "Устройство::сер.№", 
                "Устройство::адрес",
                "Ячейка::алиас",
                "Ячейка::значение",
                "Статистика::% отсеянных",
                "Статистика::список отброшенных",
                "Статистика::список принятых"
            ));

            foreach (oneAnalyzeStructure oas in toSaveData)
            {
                char quoteSymb = '"'; // TODO: не помню как, и инета под рукой нет
                StringBuilder sbR = new StringBuilder();
                StringBuilder sbA = new StringBuilder();
                sbA.Append(quoteSymb);
                sbR.Append(quoteSymb);
                foreach (var value in oas.Stat_RejectedValues)
                {
                    sbR.Append(value);
                    sbR.Append("\n");
                }
                foreach (var value in oas.Stat_AcceptedValues)
                {
                    sbA.Append(value);
                    sbA.Append("\n");
                }
                sbA.Remove(sbA.Length - 1, 1);
                sbR.Remove(sbR.Length - 1, 1);

                sbA.Append(quoteSymb);
                sbR.Append(quoteSymb);

                if (sbA.Length < 3)
                    sbA = new StringBuilder();
                if (sbR.Length < 3)
                    sbR = new StringBuilder();

                sb.AppendLine(string.Format(
                    "{0};{1};{2};{3};{4};{5};{6};{7}",
                    oas.Point_TimeStamp.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                    
                    oas.Device_SerNum.ToString(),
                    oas.Device_MBAddress.ToString(),

                    oas.Cell_Alias,
                    oas.Cell_ValueSD,

                    oas.Stat_BadCyclesPercent.ToString(),
                    
                    sbR,
                    sbA
                    ));
            }

            //			string FullFileName = string.Format("{0}{1:D3}.csv", path, NomerUchastka);
            try
            {
                using (TextWriter tw = new StreamWriter(path: path, append: false, encoding: Encoding.GetEncoding("UTF-8")))
                {
                    tw.Write(sb);
                }
            }
            catch (System.IO.IOException)
            {
                result = false;
            }
            return result;
        }
    }
}
