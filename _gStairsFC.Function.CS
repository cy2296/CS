using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gStairsFC : FunctionSeries<System.Double>
		{
            private VariableSeries<int> m_pos;
            private double m_currHigh;
            private double m_currLow;
            private double m_initDiff;
            private double m_initStair;
            private VariableSeries<Double> m_stairs;

            private VariableSeries<Double> m_high;
            private VariableSeries<Double> m_low;

			public _gStairsFC(CStudyControl _master) : base(_master) { }
			public _gStairsFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> o { get; set; }    // open
            public ISeries<Double> h { get; set; }    // for spread : Max(open, close)
            public ISeries<Double> l { get; set; }    // for spread : Min(open, close)
            public ISeries<Double> c { get; set; }    // closes
            public Double initStop { get; set; }   // 
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
				// assign inputs
			}

			protected override System.Double CalcBar()
			{
                m_pos.Value = StrategyInfo.MarketPosition;
                m_high.Value = Math.Max(h[0], l[0]);    // for spread
                m_low.Value = Math.Min(h[0], l[0]);     // for spread
         
                if (m_pos.Value == 0)
                {
                    if (calltype == "indic")
                    {
                        // no position or just close one.
                        if (m_pos[1] == 0)
                            m_stairs.Value = c[0];
                        if (m_pos[1] > 0)
                            m_stairs.Value = m_currLow;
                        if (m_pos[1] < 0)
                            m_stairs.Value = m_currHigh;
                    }
                    if (calltype == "strat")
                    {
                        m_stairs.Value = c[0];
                    }
                }
                if (m_pos.Value > 0)
                {
                    if (m_pos[1] <= 0) // just open long position, or reverse short to long
                    {
                        if (calltype == "indic")
                        {
                            m_currHigh = Math.Max(m_high[1], m_high[0]);
                            m_initStair = Math.Min(m_low[1], m_low[0]);
                            var initStopPrice = c[0] - initStop;
                            m_currLow = Math.Min(m_initStair, initStopPrice);
                            m_initDiff = (initStopPrice < m_initStair) ? (m_initStair - initStopPrice) : 0;
                            m_stairs.Value = m_currLow;
                        }
                        if (calltype == "strat")
                        {
                            m_currHigh = Math.Max(m_high[1], m_high[2]);
                            m_initStair = Math.Min(m_low[1], m_low[2]);
                            var initStopPrice = c[1] - initStop;
                            m_currLow = Math.Min(m_initStair, initStopPrice);
                            m_initDiff = (initStopPrice < m_initStair) ? (m_initStair - initStopPrice) : 0;

                            if (c[0] <= m_currHigh)
                            {
                                m_currHigh = Math.Max(m_currHigh, m_high[0]);
                                m_stairs.Value = m_currLow;
                            }
                            else  // close[0] > currHigh, update stairs
                            {
                                m_currHigh = Math.Max(m_currHigh, m_high[0]);
                                double low = Math.Min(m_low[1], m_low[0]);
                                if (low > m_currLow)
                                {
                                    if (Math.Abs(low - m_initStair) >= m_initDiff)
                                        m_currLow = low;   // don't consider initStop gap
                                    else
                                        m_currLow = low - m_initDiff;
                                }
                                m_stairs.Value = m_currLow;
                            }                              
                        }
                    }
                    else // existing long
                    {
                        if (c[0] <= m_currHigh)
                        {
                            m_currHigh = Math.Max(m_currHigh, m_high[0]);
                            m_stairs.Value = m_currLow;
                        }
                        else  // update new m_stairway
                        {
                            m_currHigh = Math.Max(m_currHigh, m_high[0]);
                            double low = Math.Min(m_low[1], m_low[0]);
                            if (low > m_currLow)
                            {
                                if (Math.Abs(low - m_initStair) >= m_initDiff)
                                    m_currLow = low;   // don't consider initStop gap
                                else
                                    m_currLow = low - m_initDiff;
                            }
                            m_stairs.Value = m_currLow;
                        }
                    }
                }
                if (m_pos.Value < 0)
                {
                    if (m_pos[1] >= 0) // just open short position, or reversse long to short 
                    {
                        if (calltype == "indic")
                        {
                            m_currLow = Math.Min(m_low[1], m_low[0]);
                            m_initStair = Math.Max(m_high[1], m_high[0]);
                            var initStopPrice = c[0] + initStop;
                            m_currHigh = Math.Max(m_initStair, initStopPrice);
                            m_initDiff = (initStopPrice > m_initStair) ? (initStopPrice - m_initStair) : 0;
                            m_stairs.Value = m_currHigh;
                        }
                        if (calltype == "strat")
                        {
                            m_currLow = Math.Min(m_low[1], m_low[2]);
                            m_initStair = Math.Max(m_high[1], m_high[2]);
                            var initStopPrice = c[1] + initStop;
                            m_currHigh = Math.Max(m_initStair, initStopPrice);
                            m_initDiff = (initStopPrice > m_initStair) ? (initStopPrice - m_initStair) : 0;

                            if (c[0] >= m_currLow)
                            {
                                m_currLow = Math.Min(m_currLow, m_low[0]);
                                m_stairs.Value = m_currHigh;
                            }
                            else
                            {
                                m_currLow = Math.Min(m_currLow, m_low[0]);
                                double high = Math.Max(m_high[1], m_high[0]);
                                if (high < m_currHigh)
                                {
                                    if (Math.Abs(high - m_initStair) >= m_initDiff)
                                        m_currHigh = high;
                                    else
                                        m_currHigh = high - m_initDiff;
                                }
                                m_stairs.Value = m_currHigh;
                            }
                        }
                    }
                    else // existing short
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
                            if (high < m_currHigh)
                            {
                                if (Math.Abs(high - m_initStair) >= m_initDiff)
                                    m_currHigh = high;
                                else
                                    m_currHigh = high - m_initDiff;
                            }
                            m_stairs.Value = m_currHigh;
                        }
                    }
                }
                return m_stairs.Value;
			}
		}
	}
}