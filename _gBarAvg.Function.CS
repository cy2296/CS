using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gBarAvg : FunctionSeries<System.Double>
		{
			public _gBarAvg(CStudyControl _master) : base(_master) { }
			public _gBarAvg(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }

			protected override void Create()
			{
				// create variable objects and function objects
			}

			protected override void StartCalc()
			{
				// assign inputs
			}

			protected override System.Double CalcBar()
			{
                double open = Bars.Open[0] * leg1
                    + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                    + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
                double close = Bars.Close[0] * leg1
                    + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                    + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);
                double high = (leg2 == 0 && leg3 == 0) ? Bars.High[0] * leg1 :
                    Math.Max(open, close);
                double low = (leg2 == 0 && leg3 == 0) ? Bars.Low[0] * leg1 :
                    Math.Min(open, close);
                return 0.25 * (open + high + low + close);
			}
		}
	}
}