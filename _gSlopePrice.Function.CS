using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gSlopePrice : FunctionSeries<System.Double>
		{
            private AverageFC m_slopeAvg;
            VariableSeries<double> m_slope;

			public _gSlopePrice(CStudyControl _master) : base(_master) { }
			public _gSlopePrice(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<double> price;
            public int lookback { get; set; }
            public int lengthbb { get; set; }
            

			protected override void Create()
			{
                m_slopeAvg = new AverageFC(this);
                m_slope = new VariableSeries<double>(this);
			}

			protected override void StartCalc()
			{
                m_slopeAvg.price = m_slope;
                m_slopeAvg.length = lengthbb;
			}

			protected override System.Double CalcBar()
			{
                m_slope.Value = (price[0] - price[lookback]) / lookback;
                //var upband = m_slopeAvg[0] + 2 * m_sdev;
                //var dnband = m_slopeAvg[0] - 2 * m_sdev;

				    // function logic
				    return default(System.Double);
			}
		}
	}
}