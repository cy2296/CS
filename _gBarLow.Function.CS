﻿using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gBarLow : FunctionSeries<System.Double>
		{
			public _gBarLow(CStudyControl _master) : base(_master) { }
			public _gBarLow(CStudyControl _master, int _ds) : base(_master, _ds) { }

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
                double open = Bars.Open[0] * leg1
                    + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                    + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
                double close = Bars.Close[0] * leg1
                    + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                    + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);

                double low = (leg2 == 0 && leg3 == 0) ? Bars.Low[0] * leg1 :
                    Math.Min(open, close);

                return low;
			}
		}
	}
}