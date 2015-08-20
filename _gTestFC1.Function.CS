using System;
using System.Drawing;
using System.Linq;


using System.Text;
using System.IO;
using System.Collections.Generic;
namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gTestFC1 : FunctionSeries<System.Double>
		{
            private System.Text.StringBuilder csv;  // for debug only



            private VariableSeries<int> m_pos;
            private double m_currHigh;
            private double m_currLow;
            private VariableSeries<Double> m_stairway;

            private VariableSeries<Double> m_high;
            private VariableSeries<Double> m_low;

			public _gTestFC1(CStudyControl _master) : base(_master) { }
			public _gTestFC1(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> o { get; set; }    // open
            public ISeries<Double> h { get; set; }    // for spread : Max(open, close)
            public ISeries<Double> l { get; set; }    // for spread : Min(open, close)
            public ISeries<Double> c { get; set; }    // closes
            public String calltype { get; set; }   // "strat" or "Indic"

			protected override void Create()
			{
                m_pos = new VariableSeries<int>(this);
                m_stairway = new VariableSeries<Double>(this);
                m_high = new VariableSeries<double>(this);
                m_low = new VariableSeries<double>(this);
                csv = new StringBuilder();
			}
			protected override void StartCalc()
			{
			}
			protected override System.Double CalcBar()
			{
                m_pos.Value = StrategyInfo.MarketPosition;
                m_high.Value = Math.Max(h[0], l[0]);    // for spread
                m_low.Value = Math.Min(h[0], l[0]);     // for spread
                if (calltype == "indic")
                {
                    if (m_pos.Value == 0)
                    {
                        // no position or just close one.
                        if (m_pos[1] == 0)
                            m_stairway.Value = c[0];
                        else 
                            m_stairway.Value = m_stairway[1];
                    }
                    if (m_pos.Value > 0)
                    {
                        if (m_pos[1] == 0) // just open long position
                        {
                            m_currHigh = Math.Max(m_high[1], m_high[0]);
                            m_currLow = Math.Min(m_low[1], m_low[0]);
                            m_stairway.Value = m_currLow;
                        }
                        else // existing long
                        {
                            if (c[0] <= m_currHigh)
                            {
                                m_currHigh = Math.Max(m_currHigh, m_high[0]);
                                m_stairway.Value = m_stairway[1];
                            }
                            else  // update new m_stairway
                            {
                                m_currHigh = Math.Max(m_currHigh, m_high[0]);
                                double low = Math.Min(m_low[1], m_low[0]);
                                m_currLow = Math.Max(low, m_currLow);
                                m_stairway.Value = m_currLow;
                            }
                        }
                    }
                    if (m_pos.Value < 0)
                    {
                        if (m_pos[1] == 0) // just open long position
                        {
                            m_currHigh = Math.Max(m_high[1], m_high[0]);
                            m_currLow = Math.Min(m_low[1], m_low[0]);
                            m_stairway.Value = m_currHigh;
                        }
                        else // existing short
                        {
                            if (c[0] >= m_currLow)
                            {
                                m_currLow = Math.Min(m_currLow, m_low[0]);
                                m_stairway.Value = m_stairway[1];
                            }
                            else
                            {
                                m_currLow = Math.Min(m_currLow, m_low[0]);
                                double high = Math.Max(m_high[1], m_high[0]);
                                m_currHigh = Math.Min(high, m_currHigh);
                                m_stairway.Value = m_currHigh;
                            }
                        }
                    }
                }

                if (calltype == "strat")
                { }
                if (Bars.CurrentBar < 500)
                {
                    var x = Bars.TimeValue.Date;
                    var a = Bars.TimeValue.TimeOfDay;
                    var b = Bars.CurrentBar;
                    var z = m_high.Value;
                    var g = m_low.Value;
                    var p = c[0];
                    var d = m_pos[1];
                    var e = StrategyInfo.MarketPosition;
                    var f = m_currHigh;
                    var j = m_currLow;

                    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}{10}",
                    x, a, b, z, g, p, d, e, f, j, System.Environment.NewLine);
                    csv.Append(newLine);
                    System.IO.File.WriteAllText("H:/eSignalData/debug.csv", csv.ToString());
                }
                return m_stairway.Value;
                //return h[0];
                
            }
		}
	}
}