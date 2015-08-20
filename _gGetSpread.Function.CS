using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gGetSpread : FunctionSimple<System.Double>
		{
			public _gGetSpread(CStudyControl _master) : base(_master) { }
			public _gGetSpread(CStudyControl _master, int _ds) : base(_master, _ds) { }

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
               return Bars.Close[0] * leg1
                    + ((leg2 != 0) ? BarsOfData(2).Close[0] : Bars.Close[0]) * leg2
                    + ((leg3 != 0) ? BarsOfData(3).Close[0] : Bars.Close[0]) * leg3;
			}
		}
	}
}