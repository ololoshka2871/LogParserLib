using System;
using System.Collections.Generic;

namespace LogParser
{
    public class oneAnalyzeStructure
    {
        public oneAnalyzeStructure(
            DateTime Point_TimeStamp, // Точка::штамп времени

            //int Channel, // канал

            UInt16 Device_SerNum,
            string Device_MBAddress,

            string Cell_Alias,
            // TODO: для булевых значений написать анализ по мажоритарности
            // TODO: всё кроме булевых представляем в System::Double
            double Cell_ValueSD, // СКО. Для булевых перевод обратно в дискретное значение [false, true]

            double Stat_BadCyclesPercent, // % отсеянных циклов 0..1

            List<double> Stat_RejectedValues, // отброшенные значения
            List<double> Stat_AcceptedValues // принятые значения
        )
        {
            this.Point_TimeStamp = Point_TimeStamp;

            this.Device_SerNum = Device_SerNum;
            this.Device_MBAddress = Device_MBAddress;

            this.Cell_Alias = Cell_Alias;
            this.Cell_ValueSD = Cell_ValueSD;

            this.Stat_BadCyclesPercent = Stat_BadCyclesPercent;

            this.Stat_RejectedValues = Stat_RejectedValues;
            this.Stat_AcceptedValues = Stat_AcceptedValues;
        }

        public DateTime Point_TimeStamp { get; private set; }

        public UInt16 Device_SerNum { get; private set; }
        public string Device_MBAddress { get; private set; }

        public string Cell_Alias { get; set; }
        public double Cell_ValueSD { get; set; }

        public double Stat_BadCyclesPercent { get; set; }
        public List<double> Stat_RejectedValues { get; set; }
        public List<double> Stat_AcceptedValues { get; set; }

        //public bool isProbablyBad { get; set; }
    }
}
