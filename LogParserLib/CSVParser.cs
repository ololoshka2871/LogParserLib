using System;
using System.Linq;
using System.Collections.Generic;

namespace LogParser
{
	/// <summary>
	/// Description of CSVParser.
	/// </summary>
	
	public class CSVParser
	{
		// выделяем полный цикл измерений в один блок (итерации * ячейки)
		public static List<List<oneStringStructure>> ExtractMeasurePoints(List<oneStringStructure> inData)
		{
            // берём только одно устройство
            var dev = inData[0].Device_MBAddress;
            var query = (from oneStringStructure r in inData
                         /*orderby r.Point_TimeStamp
                         orderby r.Device_MBAddress
                         orderby r.Cell_Alias*/
                            where r.Device_MBAddress == dev // обрабатываем только одно устройство
                             //orderby r.Device_MBAddress
                            orderby r.Point_TimeStamp

                         orderby r.Iteration
                         orderby r.ChannelNum
                         //group r by r.Point_TimeStamp into strG
                         //group r by r.Point_TimeStamp
                         //group r by new { r.Point_TimeStamp, r.Device_SerNum, r.Device_MBAddress, r.Cell_Alias }
                         group r by new { r.Point_TimeStamp, r.ChannelNum }
                //select strG).ToList();
                ).ToList();

            List<List<oneStringStructure>> result = new List<List<oneStringStructure>>();
            foreach (var item in query)
            {
                result.Add(item.ToList());
            }
            return result;
		}

        /*
        // фильтрация по каналам (аномальные точки в графике)
		public void FilterResultDataOfChannel(List<oneAnalyzeStructure> inData)
		{
			int badRecsCount = 1;
			int iter = 0;
			while (badRecsCount != 0) {
				// отсеиваем по частоте битые измерения
				iter++;
                badRecsCount = FilterCollectionOfReading(0, ref inData, 99);
			}
			// теперь фильтруем выборку из 10 показаний
			badRecsCount = 0;
			int sizeOfFilterInData = 10;
			
			var query = (from id in inData
			    where id.isProbablyBad == false
			    select id).ToList();
			
			for (int i = 0; i < query.Count; i++) {
				query = (from id in inData
			        where id.isProbablyBad == false
			        select id).ToList();
				if (i + sizeOfFilterInData < query.Count)
				{
                    List<oneAnalyzeStructure> aData = query.GetRange(i, sizeOfFilterInData);
                    badRecsCount = FilterCollectionOfReading(badRecsCount, ref aData, 999);
				}
			}
		}

        private static int FilterCollectionOfReading(int badRecsCount, ref List<oneAnalyzeStructure> data, int R)
        {
            var probGoodData = (from id in data
                where id.isProbablyBad == false
                select id).ToList();

            PointsTester epa = new PointsTester();
            foreach (var oas in probGoodData)
            {
                epa.AddPoint(oas.Cell_Value);
            }
            foreach (var oas in probGoodData)
            {
                if (!epa.isTestPointGood(oas.Cell_Value, R))
                {
                    badRecsCount++;
                    oas.isProbablyBad = true;
                }
            }
            return badRecsCount;
        }
        */

        /// <summary>
        /// фильтрует неверные измерения внутри одной группы однородных измерений
        /// </summary>
        /// <param name="inData">входные данные - 1 точка, 1 устройство, 1 ячейка, > 3 измерений</param>
        /// <returns>результирующую структуру</returns>
		public oneAnalyzeStructure FilterGroup(List<oneStringStructure> inData)
		{
			List<oneStringStructure> _inData = new List<oneStringStructure>(inData);
			int badRecsCount = 0;
			oneAnalyzeStructure result = new oneAnalyzeStructure(
                _inData[0].Point_TimeStamp, //inData[0].Point_TimeStamp.AddTicks((_inData[_inData.Count-1].Point_TimeStamp.Subtract(inData[0].Point_TimeStamp)).Ticks / 2),
                _inData[0].Device_SerNum,
                _inData[0].Device_MBAddress,
                _inData[0].Cell_Alias,
                double.NaN,
                double.NaN,
                new List<double>(),
                new List<double>()
				);
			
			// отсеиваем по частоте битые измерения
            PointsTester epa = new PointsTester();
            // забиваем все полученные значения измерения
			foreach (var item in _inData) {
                if (!double.IsNaN(item.Cell_Value) && !double.IsInfinity(item.Cell_Value))
                {
                    epa.AddPoint(item.Cell_Value);
                    result.Stat_AcceptedValues.Add(item.Cell_Value);
                }
			}
            // проверяем по одному значению, соответствует ли оно гипотезе            
            bool badRecsFound = true;
            while (badRecsFound)
            {
                badRecsFound = false;
                for (int i = _inData.Count - 1; i >= 0; i--)
                {
                    if (!epa.isTestPointGood(_inData[i].Cell_Value, 99))
                    {
                        badRecsCount++;
                        badRecsFound = true;
                        result.Stat_AcceptedValues.Remove(_inData[i].Cell_Value); // удаляем из списка принятых значений
                        result.Stat_RejectedValues.Add(_inData[i].Cell_Value); // помещаем в список отсеянных значений
                        _inData.Remove(_inData[i]);
                    }
                }
			}

			// по остальным находим средние квадратичные значения
            double skvCV = 0;
			
			foreach (var oss in _inData) 
            {
                skvCV += Math.Pow(oss.Cell_Value, 2);
			}
            skvCV = Math.Sqrt(skvCV / _inData.Count);
            result.Cell_ValueSD = skvCV;
			result.Stat_BadCyclesPercent = 1 - (_inData.Count / (double)(_inData.Count + badRecsCount));
            result.Stat_BadCyclesPercent *= 100;
			return result;
		}
	}
}
