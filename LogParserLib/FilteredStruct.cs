using AlgosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogParserLib
{
	public class FilteredStruct
    {
        //List<double> _unfilteredF;

        // для идентификации измерения
        public DateTime Point_TimeStamp;
        public int ChannelNum;

        public double F;
        public double ls;
        public List<double> GoodPnts;
        public List<double> BadPnts;
        public double reliability;
        public int PntNum;

        public FilteredStruct(List<double> _unfilteredF, DateTime Point_TimeStamp, int ChannelNum, int PntNum)
        {
            //this._unfilteredF = _unfilteredF;
            FilterPoint(_unfilteredF);
            this.Point_TimeStamp = Point_TimeStamp;
            this.ChannelNum = ChannelNum;
            this.PntNum = PntNum;
        }

        //public FilteredStruct(int PntNum, DateTime Point_TimeStamp, int ChannelNum, double F, double ls, double reliability, string sGoodPnts, string sBadPnts)
        public FilteredStruct(object PntNum, object Point_TimeStamp, object ChannelNum, object F, object ls, object sGoodPnts, object sBadPnts, object reliability)
        {
            this.PntNum = (int)(double)PntNum;
            this.Point_TimeStamp = DateTime.Parse((string)Point_TimeStamp);
            this.ChannelNum = (int)(double)ChannelNum;
            this.F = (double)F;
            this.ls = (double)ls;
            this.reliability = (double)reliability;

            this.GoodPnts = new List<double>();
            this.BadPnts = new List<double>();

            string[] arr;
            //TODO: обработка ошибок конвертации
            //string[] lines = theText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (sGoodPnts != null)
            {
                if (sGoodPnts is double)
                    GoodPnts.Add((double)sGoodPnts);
                else
                {
                    arr = ((string)sGoodPnts).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (var s in arr)
                    {
                        GoodPnts.Add(double.Parse(s));
                        //GoodPnts.Add(Convert.ToDouble(s));
                    }
                }
            }
            if (sBadPnts != null)
            {
                arr = ((string)sBadPnts).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var s in arr)
                {
                    BadPnts.Add(Convert.ToDouble(s));
                }
            }
            //this.GoodPnts = GoodPnts; //TODO
            //this.BadPnts = BadPnts; //TODO
        }

        void FilterPoint(List<double> row)
        {
            var fr = Статистика_Общее.Исключение_промахов_ряда(row, Статистика_Общее.EStudentCriticalP.a950);
            GoodPnts = fr.GoodPnts;
            BadPnts = fr.BadPnts;
            F = Статистика_Общее.Среднее_квадратическое(GoodPnts);
            if (double.IsNaN(F)) // патч для Excel (Excel распознаёт NaN как double.MaxValue)
                F = -1;
            ls = Статистика_Общее.Стандартное_отклонение(GoodPnts);
            if (row.Count() > 0)
                reliability = GoodPnts.Count() / row.Count();
            else
                reliability = 0; // для каналов, у которых нет измерений в текущей точке
            if (ls == 0)
                reliability /= 2;
            /*if (reliability > 0)
            {
                double fac3 = 1/(ls * 1e6 / F); // получили
                if (fac3 > 1)
                    fac3 = 1;
                reliability *= fac3; // TODO: сделать зависимость от стандартного отклонения
            }*/
        }
    }
}