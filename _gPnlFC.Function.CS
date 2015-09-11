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
		public sealed class _gPnlFC : FunctionSeries<System.Double>
		{
		    private VariableSeries<int> m_pos;
            private VariableSeries<double> m_pnl;	
			
			public _gPnlFC(CStudyControl _master) : base(_master) { }
			public _gPnlFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> price { get; set; }
            public string calltype { get; set; }


            private System.Text.StringBuilder csv;  // for debug only

			protected override void Create()
			{
                m_pos = new VariableSeries<int>(this);
                m_pnl = new VariableSeries<double>(this);

                csv = new StringBuilder();
			}

			protected override void StartCalc()
			{
				// assign inputs
			}

			protected override System.Double CalcBar()
			{
                m_pos.Value = StrategyInfo.MarketPosition;
                if (m_pos.Value == 0)
                {
                    if (m_pos[1] == 0)  // no position
                        m_pnl.Value = 0;
                    else // just close a position
                    {
                        if (m_pos[1] > 0)   // just close long pos
                        {
                            if (calltype == "indic")
                                m_pnl.Value = m_pnl[1] + (price[0] - price[1]);
                            if (calltype == "strat")
                                m_pnl.Value = 0;
                        }
                        if (m_pos[1] < 0)   // just close short pos
                        {
                            if (calltype == "indic")
                                m_pnl.Value = m_pnl[1] + (price[1] - price[0]);
                            if (calltype == "strat")
                                m_pnl.Value = 0;
                        }
                    }
                }
                else  // has position
                {
                    if (m_pos[1] == 0)   // new open position
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = 0;
                        }
                        if (calltype == "strat")
                        {
                            if (m_pos.Value > 0)
                                m_pnl.Value = price[0] - price[1];
                            if (m_pos.Value < 0)
                                m_pnl.Value = price[1] - price[0];
                        }
                    }
                    if (m_pos.Value > 0 && m_pos[1] > 0)    // existing long
                    {
                        if (calltype == "indic")
                        {
                            if (m_pos[2] < 0)    // just reverse from short
                                m_pnl.Value = price[0] - price[1];
                            else
                                m_pnl.Value = m_pnl[1] + (price[0] - price[1]);
                        }
                        if (calltype == "strat")
                            m_pnl.Value = m_pnl[1] + (price[0] - price[1]);

                    }
                    if (m_pos.Value < 0 && m_pos[1] < 0)    // existing short
                    {
                        if (calltype == "indic")
                        {
                            if (m_pos[2] > 0)   // just reverse from long
                                m_pnl.Value = price[1] - price[0]; 
                            else 
                                m_pnl.Value = m_pnl[1] + (price[1] - price[0]);
                        }
                        if (calltype == "strat")
                            m_pnl.Value = m_pnl[1] + (price[1] - price[0]);
                    }
                    if (m_pos.Value > 0 && m_pos[1] < 0)    // short reverse to long
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = m_pnl[1] + (price[1] - price[0]); // pnl for previous short position, pnl for current long position is 0
                        }
                        if (calltype == "strat")
                        {
                            m_pnl.Value = price[0] - price[1]; // only pnl for long position
                        }
                    }
                    if (m_pos.Value < 0 && m_pos[1] > 0)    // long reverse to short
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = m_pnl[1] + (price[0] - price[1]); // pnl for previous long position, pnl for current short position is 0
                        }
                        if (calltype == "strat")
                        {
                            m_pnl.Value = price[1] - price[0];  // pnl for long already closed, only pnl for short
                        }
                    }
                }


                //if (Bars.CurrentBar < 2000)
                //{
                //    var a = Bars.TimeValue.Date;
                //    var b = Bars.TimeValue.TimeOfDay;
                //    var c = Bars.CurrentBar;
                //    var d = m_pos[0];
                //    var e = m_pnl[0];
                //    //var f = m_twoLineGap[0];
                //    //var g = m_mp[0];
                //    //var h = m_sprdPnl[0];
                //    //var i = m_stopPos[0];

                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}{5}",
                //    a, b, c, d, e, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    System.IO.File.WriteAllText("H:/eSignalData/debug12.csv", csv.ToString());
                //}




                return m_pnl.Value;
			}
		}
	}
}