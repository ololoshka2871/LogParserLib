using System;
using System.Collections.Generic;

namespace LogParser
{
    public class PointsTester
    {
        List<double> pnts = new List<double>();
        double S;
        double mid;
        int n; // n - количество точек

        /// <summary>
        /// добавление точки в набор для анализа
        /// </summary>
        /// <param name="pnt"></param>
        public void AddPoint(double pnt)
        {
            if (!double.IsNaN(pnt))
                pnts.Add(pnt);
        }

        protected double Get_t(int n, int R)
        {
            // n - число приемлемых результатов (не считая тестируемого)
            // R - надёжность вывода
            //Dictionary<int, double> R095 = { 5: 3.04, 6: 2.78, 7: 2.62, 8: 2.51, 9: 2.43, 10: 2.37 };
            // = new Dictionary<int, double>();
            Dictionary<int, double> R_current;
            Dictionary<int, double> R099 = new Dictionary<int, double>()
			{
				{ 4, 7.40}, 
				{ 5, 5.04},
				{ 6, 4.36},
				{ 7, 3.96},
				{ 8, 3.71},
				{ 9, 3.54},
				{10, 3.41},
				{11, 3.31},
				{12, 3.23},
				{13, 3.17},
				{14, 3.12},
				{15, 3.08},
				{16, 3.04},
				{17, 3.01},
				{18, 2.98},
				{20, 2.932},
				{25, 2.852},
				{30, 2.802},
				{35, 2.768},
				{40, 2.742},
				{45, 2.722},
				{50, 2.707},
				{60, 2.683},
				{70, 2.667},
				{80, 2.655},
				{90, 2.646},
				{100, 2.639},
				{int.MaxValue, 2.576}
			};
            Dictionary<int, double> R0999 = new Dictionary<int, double>()
			{
				{ 4, 13.85}, // так-то слишком мало для такого вида анализа
				{ 5, 9.43},
				{ 6, 7.41},
				{ 7, 6.37},
				{ 8, 5.73},
				{ 9, 5.31},
				{10, 5.01},
				{11, 4.79},
				{12, 4.62},
				{13, 4.48},
				{14, 4.37},
				{15, 4.28},
				{16, 4.20},
				{17, 4.13},
				{18, 4.07},
				{20, 3.979},
				{25, 3.819},
				{30, 3.719},
				{35, 3.652},
				{40, 3.602},
				{45, 3.565},
				{50, 3.532},
				{60, 3.492},
				{70, 3.462},
				{80, 3.439},
				{90, 3.423},
				{100, 3.409},
				{int.MaxValue, 3.291}
			};
            //
            switch (R)
            {
                case 99:
					R_current = R099;
					break;
                case 999:
					R_current = R0999;
					break;
                default: throw new NotImplementedException();
            }

            //
            int nl = int.MinValue;
            int ng = int.MaxValue;
            foreach (var key in R_current.Keys) // ищем диапазон
            {
                if (key == n) // нашли точное значение
                    return R_current[key];

                if ((key < n) && (key > nl))
                    nl = key;
                if ((key > n) && (key < ng))
                    ng = key;
            }
            // интерполируем
            double y;
            if (nl == int.MinValue)
            {
                return int.MaxValue; // слишком мало значений для определения, поэтому не бракуем
            }
            if (ng != int.MaxValue)
            {
                y = R_current[nl] + ((R_current[ng] - R_current[nl]) / (ng - nl) * (n - nl));
            }
            else
            {
                y = R_current[ng] + ((R_current[nl] - R_current[ng]) / n * 100);
            }
            return y;
        }

        private void calcS(double tstpnt)
        {
            n = pnts.Count - 1; // вынимаем тестируемое значение
            // mid - среднее
            mid = 0;
            foreach (var pnt in pnts)
            {
                mid += pnt;
            }
            mid -= tstpnt; // вынимаем тестируемое значение
            mid /= n;
            double sum = 0;
            foreach (var pnt in pnts)
            {
                sum += Math.Pow(pnt - mid, 2);
            }
            sum -= Math.Pow(tstpnt - mid, 2); // вынимаем тестируемое значение
            // "S"
            S = Math.Sqrt(1 / (double)(n - 1) * sum);
            // ещё одна заплатка. При всех абсолютно равных значениях S == 0, и дальнейшие расчёты срываются.
            if (S == 0)
            {
                S = double.Epsilon;
            }
        }

        private double getMedian()
        {
            double mid = 0;
            foreach (var pnt in pnts)
            {
                mid += pnt;
            }
            mid /= pnts.Count;
            return mid;
        }

        /// <summary>
        /// проверка гипотезы о соответствии числа критерию
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public bool isTestPointGood(double pnt, int R)
        {
            calcS(pnt);
            double diff = Math.Abs(pnt - mid);
            // если разница меньше, чем погрешность типа данных Double
            // иначе говоря: если разница из-за погрешности хранения в памяти
            // то обнуляем разницу.
            if (diff < 1e-15 * Math.Pow(10, Math.Ceiling(Math.Log10(pnt)))) // сдвигаем минимальное значащее значение на количество знаков до запятой
            {
                diff = 0;
            }
            //double t = Math.Abs(pnt - mid) / S;
            double t = diff / S;

            bool в_допустимом_диапазоне = true;
            bool гипотеза_принята = true;
            bool отклонение_минимально = true;
//            в_допустимом_диапазоне = (pnt > 100) && (pnt < 1000);
            
            в_допустимом_диапазоне = (pnt >= 0); // 1-я проверка на валидный диапазон значений
            гипотеза_принята = (t < Get_t(n, R)); // 2-я проверка по критерию Стьюдента

            double relativeError = diff / getMedian();
            отклонение_минимально = relativeError < 0.000001;
            // 3-я проверка на исключение ложных срабатываний при едва различимых расхождениях

            return (
                в_допустимом_диапазоне && 
                (
                    гипотеза_принята ||
                    (!гипотеза_принята && отклонение_минимально)
                )
                );
        }
    }
}
