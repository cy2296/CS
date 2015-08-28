using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gBarClose : FunctionSeries<System.Double>
		{
			public _gBarClose(CStudyControl _master) : base(_master) { }
			public _gBarClose(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }

			protected override void Create()
			{
			}

			protected override void StartCalc()
			{
			}

			protected override System.Double CalcBar()
			{
                double close = Bars.Close[0] * leg1
                    + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                    + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);

                return close;
			}
		}
	}
}