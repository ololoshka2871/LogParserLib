using System.Collections.Generic;
using System.Linq;

namespace LogParser
{
    public class MainAction
    {
        public bool Do(List<oneStringStructure> loss, CSV_ImportExport csv_ie)
        {
            bool result = false;
            CSVParser csvp = new CSVParser();

            // разбиваем на точки
            List<List<oneStringStructure>> MeasurePointsCollection = CSVParser.ExtractMeasurePoints(loss);

            List<oneAnalyzeStructure> resultFull = new List<oneAnalyzeStructure>();
            // НЕ группируем результат по точкам
            foreach (var onePoint in MeasurePointsCollection)
            { // для каждой точки
                var query1 = (from oneStringStructure r in onePoint
                              group r by r.Device_MBAddress // или можно по серийнику
                ).ToList();
                foreach (var item in query1)
                {
                    resultFull.Add(csvp.FilterGroup(item.ToList()));
                }
            }

            /*
            // фильтрация по ячейкам (аномальные выпады в графике)
            var query2 = (from r in resultFull
                            group r by r.Cell_Alias
            ).ToList();
            foreach (var item in query2)
            {
                csvp.FilterResultDataOfChannel(item.ToList());
            }*/

            // экспорт в csv
            bool resFileIO = csv_ie.ExportResultToCSV(resultFull);

            if (resFileIO)
            {
                result = true;
            }
            return result;
        }
    }
}
