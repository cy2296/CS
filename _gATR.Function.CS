using System;
using System.Drawing;
using System.Linq;

// the function takes up to three data series, do the spread, and compute the ATR for the spread
namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gATR : FunctionSeries<System.Double>
		{
            private VariableSeries<Double> m_atrSum;       // return result
            private _gTrueRange m_trueRangeFC;
            private SummationFC m_sumFC;

			public _gATR(CStudyControl _master) : base(_master) { }
			public _gATR(CStudyControl _master, int _ds) : base(_master, _ds) { }

            // Set up 
            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }
            public int atrlen { get; set; }	
            //
			protected override void Create()
			{
                m_atrSum = new VariableSeries<double>(this);
                m_trueRangeFC = new _gTrueRange(this);
                m_sumFC = new SummationFC(this);
			}

			protected override void StartCalc()
			{
                m_trueRangeFC.leg1 = leg1;
                m_trueRangeFC.leg2 = leg2;
                m_trueRangeFC.leg3 = leg3;
                m_sumFC.price = m_trueRangeFC;
                m_sumFC.length = atrlen;
			}

			protected override System.Double CalcBar()
			{
                //if (Bars.CurrentBar == 1)
                //{
                //    m_atrSum.Value = 0;
                //    for (var i = 0; i < atrlen; ++i)
                //        m_atrSum.Value += m_trueRangeFC[i];
                //}
                //m_atrSum.Value = m_atrSum[1] + m_trueRangeFC[0] - m_trueRangeFC[atrlen];
                //return m_atrSum.Value / atrlen;
                return m_sumFC[0] / atrlen;
               
			}
		}
	}
}