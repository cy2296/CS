using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gSpreadFC : FunctionSeries<System.Double>
		{
            private VariableSeries<Double> m_spread;

            public ISeries<Double> inst1 { get; set; }
            public ISeries<Double> inst2 { get; set; }
            public ISeries<Double> inst3 { get; set; }

            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }

			public _gSpreadFC(CStudyControl _master) : base(_master) { }
			public _gSpreadFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

			protected override void Create()
			{
                m_spread = new VariableSeries<double>(this);
			}

			protected override void StartCalc()
			{
			
			}

			protected override System.Double CalcBar()
			{
                m_spread.Value = leg1 * inst1[0] + leg2 * inst2[0] + leg3 * inst3[0];
                return m_spread.Value;
			}
		}
	}
}