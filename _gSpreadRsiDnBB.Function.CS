using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gSpreadRsiDnBB : FunctionSeries<System.Double>
		{
			public _gSpreadRsiDnBB(CStudyControl _master) : base(_master) { }
			public _gSpreadRsiDnBB(CStudyControl _master, int _ds) : base(_master, _ds) { }

            /// for spread rsi function
            private _gSpreadRSI m_spreadRsiFC;

            public ISeries<Double> price1 { get; set; }
            public ISeries<Double> price2 { get; set; }
            public ISeries<Double> price3 { get; set; }
            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }
            public Int32 len_rsi { get; set; }

            /// for BBand 
            private AverageFC m_averagefc1;
            private VariableSeries<Double> m_spread_rsi;

            public int numdevsup { get; set; }
            public int numdevsdn { get; set; }
            public Int32 len_bb { get; set; }

			protected override void Create()
			{
                m_spreadRsiFC = new _gSpreadRSI(this);
                m_averagefc1 = new AverageFC(this);
                m_spread_rsi = new VariableSeries<double>(this);
			}

			protected override void StartCalc()
			{
                m_spreadRsiFC.price1 = price1;
                m_spreadRsiFC.price2 = price2;
                m_spreadRsiFC.price3 = price3;
                m_spreadRsiFC.leg1 = leg1;
                m_spreadRsiFC.leg2 = leg2;
                m_spreadRsiFC.leg3 = leg3;
                m_spreadRsiFC.length = len_rsi;

                m_averagefc1.price = m_spread_rsi;
                m_averagefc1.length = len_bb;
			}

			protected override System.Double CalcBar()
			{
                m_spread_rsi.Value = m_spreadRsiFC[0];
                var m_midband = m_averagefc1[0];
                var m_sdev = m_spread_rsi.StandardDeviationCustom(len_bb, 1);
                return m_midband - (numdevsdn * m_sdev);
			}
		}
	}
}