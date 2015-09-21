using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gBarDayHigh : FunctionSeries<System.Double>
		{

			public _gBarDayHigh(CStudyControl _master) : base(_master) { }
			public _gBarDayHigh(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public bool intheday { get; set; }
            

			protected override void Create()
			{

			}

			protected override void StartCalc()
			{
			}

			protected override System.Double CalcBar()
			{
                return 0;
			}
		}
	}
}