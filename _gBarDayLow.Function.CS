using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gBarDayLow : FunctionSeries<System.Double>
		{
			public _gBarDayLow(CStudyControl _master) : base(_master) { }
			public _gBarDayLow(CStudyControl _master, int _ds) : base(_master, _ds) { }

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
				// function logic
				return default(System.Double);
			}
		}
	}
}