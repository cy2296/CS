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
		public sealed class _gPnlFC2 : FunctionSeries<System.Double>
		{
            private VariableSeries<double> m_pnl;	

			public _gPnlFC2(CStudyControl _master) : base(_master) { }
			public _gPnlFC2(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> price { get; set; }
            public ISeries<int> position { get; set; }
            public string calltype { get; set; }

            private System.Text.StringBuilder csv;  // for debug only

			protected override void Create()
			{
                m_pnl = new VariableSeries<double>(this);

                csv = new StringBuilder();
			}

			protected override void StartCalc()
			{
			}

			protected override System.Double CalcBar()
			{
                if (position[0] == 0)
                {
                    if (position[1] == 0)  // no position
                        m_pnl.Value = 0;
                    else // just close a position
                    {
                        if (position[1] > 0)   // just close long pos
                        {
                            if (calltype == "indic")
                                m_pnl.Value = m_pnl[1] + (price[0] - price[1]);
                            if (calltype == "strat")
                                m_pnl.Value = 0;
                        }
                        if (position[1] < 0)   // just close short pos
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
                    if (position[1] == 0)   // new open position
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = 0;
                        }
                        if (calltype == "strat")
                        {
                            if (position[0] > 0)
                                m_pnl.Value = price[0] - price[1];
                            if (position[0] < 0)
                                m_pnl.Value = price[1] - price[0];
                        }
                    }
                    if (position[0] > 0 && position[1] > 0)    // existing long
                    {
                        if (calltype == "indic")
                        {
                            if (position[2] < 0)    // just reverse from short
                                m_pnl.Value = price[0] - price[1];
                            else
                                m_pnl.Value = m_pnl[1] + (price[0] - price[1]);
                        }
                        if (calltype == "strat")
                            m_pnl.Value = m_pnl[1] + (price[0] - price[1]);

                    }
                    if (position[0] < 0 && position[1] < 0)    // existing short
                    {
                        if (calltype == "indic")
                        {
                            if (position[2] > 0)   // just reverse from long
                                m_pnl.Value = price[1] - price[0];
                            else
                                m_pnl.Value = m_pnl[1] + (price[1] - price[0]);
                        }
                        if (calltype == "strat")
                            m_pnl.Value = m_pnl[1] + (price[1] - price[0]);
                    }
                    if (position[0] > 0 && position[1] < 0)    // short reverse to long
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
                    if (position[0] < 0 && position[1] > 0)    // long reverse to short
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
                return m_pnl.Value;
			}
		}
	}
}