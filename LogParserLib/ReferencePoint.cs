namespace LogParserLib
{
	public class ReferencePoint
    {
        public double T;
        public double P;
        public int PntNum;

        //public ReferencePoint(DateTime Point_TimeStamp, double T, double P, int PntNum)
        public ReferencePoint(object T, object P, int PntNum)
        {
            this.T = (double)T;
            this.P = (double)P;
            this.PntNum = PntNum;
        }
    }
}