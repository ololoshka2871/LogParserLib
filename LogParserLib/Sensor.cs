using AlgosLibrary;
using System.Collections.Generic;

namespace LogParserLib
{
	public class Sensor
    {
        public Sensor(eSensorType type, int[] channels)
        {
            this.type = type;
            this._channels = channels;
            this.coeffs = new SensorCalculatedCoeffs();
            this.coeffs.coeffT = new List<TSensorCoeffs>();
            this.coeffs.coeffTP = new List<PSensorCoeffs>();
            this.cutDirection = eSensorCutDirection.NotCutted;
        }

        public eSensorType type;
        public eSensorCutDirection cutDirection;
        public int[] _channels;
        public List<FilteredStruct> lsFt;
        public List<FilteredStruct> lsFp;
        public SensorCalculatedCoeffs coeffs;
    }
}