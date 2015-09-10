using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gRisk_StopForPos : FunctionSeries<System.Double>
		{
			public _gRisk_StopForPos(CStudyControl _master) : base(_master) { }
			public _gRisk_StopForPos(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> pnl { get; set; }
            public double stop { get; set; }

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
                if (pnl[0] <= -Math.Abs(stop))
                    return 1;
                else
                    return 0;
			}
		}
	}
}