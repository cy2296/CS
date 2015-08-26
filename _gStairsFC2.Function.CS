using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gStairsFC2 : FunctionSeries<System.Double>
		{
            private double m_currHigh;
            private double m_currLow;
            private VariableSeries<int> m_pos;
            private VariableSeries<Double> m_stairs;
            private VariableSeries<Double> m_high;
            private VariableSeries<Double> m_low;

			public _gStairsFC2(CStudyControl _master) : base(_master) { }
			public _gStairsFC2(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> o { get; set; }    // open
            public ISeries<Double> h { get; set; }    // for spread : Max(open, close)
            public ISeries<Double> l { get; set; }    // for spread : Min(open, close)
            public ISeries<Double> c { get; set; }    // closes
            public String calltype { get; set; }   // "strat" or "indic"

			protected override void Create()
			{
                m_pos = new VariableSeries<int>(this);
                m_stairs = new VariableSeries<Double>(this);
                m_high = new VariableSeries<double>(this);
                m_low = new VariableSeries<double>(this);
			}

			protected override void StartCalc()
			{
			}

			protected override System.Double CalcBar()
			{
                m_pos.Value = StrategyInfo.MarketPosition;
                m_high.Value = Math.Max(h[0], l[0]);    // for spread
                m_low.Value = Math.Min(h[0], l[0]);     // for spread

                if (m_pos.Value == 0)
                {
                    if (calltype == "indic") // no pos or just close one
                    {
                        if (m_pos[0] == 0) // no pos
                            m_stairs.Value = c[0];
                        if (m_pos[1] > 0)  // close a long
                            m_stairs.Value = m_currLow;
                        if (m_pos[1] < 0)  // close a short
                            m_stairs.Value = m_currHigh;
                    }
                    if (calltype == "strat")
                        m_stairs.Value = c[0];
                }
                if (m_pos.Value > 0)
                {
                    if (m_pos[1] <= 0) // just open long position, or reverse short to long
                    {
                        if (calltype == "indic")
                        {
                            m_currHigh = Math.Max(m_high[1], m_high[0]);
                            m_currLow = Math.Min(m_low[1], m_low[0]);
                            m_stairs.Value = m_currLow;
                        }
                        if (calltype == "strat")
                        {
                            m_currHigh = Math.Max(m_high[1], m_high[2]);
                            m_currLow = Math.Min(m_low[1], m_low[2]);
                            if (c[0] <= m_currHigh)
                            {
                                m_currHigh = Math.Max(m_currHigh, m_high[0]);
                                m_stairs.Value = m_currLow;
                            }
                            else  // close[0] > currHigh, update stairs
                            {
                                m_currHigh = Math.Max(m_currHigh, m_high[0]);
                                double low = Math.Min(m_low[1], m_low[0]);
                                m_currLow = Math.Max(low, m_currLow);
                                m_stairs.Value = m_currLow;
                            }
                        }
                    }
                    else  // existing long position
                    {
                        if (c[0] <= m_currHigh)
                        {
                            m_currHigh = Math.Max(m_currHigh, m_high[0]);
                            m_stairs.Value = m_currLow;
                        }
                        else  // close[0] > currHigh, update stairs
                        {
                            m_currHigh = Math.Max(m_currHigh, m_high[0]);
                            double low = Math.Min(m_low[1], m_low[0]);
                            m_currLow = Math.Max(low, m_currLow);
                            m_stairs.Value = m_currLow;
                        }
                    }
                }
                if (m_pos.Value < 0)
                {
                    if (m_pos[1] >= 0) // just close short position, or reverse long to short
                    {
                        if (calltype == "indic")
                        {
                            m_currLow = Math.Min(m_low[1], m_low[0]);
                            m_currHigh = Math.Max(m_high[1], m_high[0]);
                            m_stairs.Value = m_currHigh;
                        }
                        if (calltype == "strat")
                        {
                            m_currLow = Math.Min(m_low[1], m_low[2]);
                            m_currHigh = Math.Max(m_high[1], m_high[2]);
                            if (c[0] >= m_currLow)
                            {
                                m_currLow = Math.Min(m_currLow, m_low[0]);
                                m_stairs.Value = m_currHigh;
                            }
                            else
                            {
                                m_currLow = Math.Min(m_currLow, m_low[0]);
                                double high = Math.Max(m_high[1], m_high[0]);
                                m_currHigh = Math.Min(high, m_currHigh);
                                m_stairs.Value = m_currHigh;
                            }
                        }
                    }
                    else  // existing short position
                    {
                        if (c[0] >= m_currLow)
                        {
                            m_currLow = Math.Min(m_currLow, m_low[0]);
                            m_stairs.Value = m_currHigh;
                        }
                        else
                        {
                            m_currLow = Math.Min(m_currLow, m_low[0]);
                            double high = Math.Max(m_high[1], m_high[0]);
                            m_currHigh = Math.Min(high, m_currHigh);
                            m_stairs.Value = m_currHigh;
                        }
                    }
                }
                return m_stairs.Value;
			}
		}
	}
}