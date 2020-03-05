using AlgosLibrary;
using System.Collections.Generic;

namespace LogParserLib
{
	public class SensorCalculatedCoeffs
    {
        // список - для интервального расчёта (склейка из нескольких полиномов)
        //public TSensorCoeffs coeffT2;
        public List<TSensorCoeffs> coeffT;
        public List<PSensorCoeffs> coeffTP;
    }
}