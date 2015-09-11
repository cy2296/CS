using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
        public sealed class _gUtility_MarketsOpened : FunctionSeries<System.Boolean>
		{
			public _gUtility_MarketsOpened(CStudyControl _master) : base(_master) { }
			public _gUtility_MarketsOpened(CStudyControl _master, int _ds) : base(_master, _ds) { }

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

			protected override System.Boolean CalcBar()
			{
                if ((leg2 == 0 && leg3 == 0) ||
                    (leg2 != 0 && leg3 == 0
                     && Bars.TimeValue.Date == BarsOfData(2).TimeValue.Date
                     && Bars.TimeValue.TimeOfDay == BarsOfData(2).TimeValue.TimeOfDay) ||
                    (leg2 != 0 && leg3 != 0
                     && Bars.TimeValue.Date == BarsOfData(2).TimeValue.Date
                     && Bars.TimeValue.Date == BarsOfData(3).TimeValue.Date
                     && Bars.TimeValue.TimeOfDay == BarsOfData(2).TimeValue.TimeOfDay
                     && Bars.TimeValue.TimeOfDay == BarsOfData(3).TimeValue.TimeOfDay))
                {
                    return true;
                }
                else
                    return false;
			}
		}
	}
}