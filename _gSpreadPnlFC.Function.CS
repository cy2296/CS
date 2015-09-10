using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
    namespace Function
    {
        public sealed class _gSpreadPnlFC : FunctionSeries<System.Double>
        {
            private _gSpreadFC m_spreadClosFC;
            private VariableSeries<int> m_pos;
            private VariableSeries<double> m_pnl;
            private VariableSeries<double> m_entryPrice;

            public _gSpreadPnlFC(CStudyControl _master) : base(_master) { }
            public _gSpreadPnlFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public string calltype { get; set; }

            // variable for _gSpreadFC
            public double leg1 { get; set; }      // 1
            public double leg2 { get; set; }      // -0.333
            public double leg3 { get; set; }      // -0.5  

            protected override void Create()
            {
                m_spreadClosFC = new _gSpreadFC(this);
                m_pos = new VariableSeries<int>(this);
                m_pnl = new VariableSeries<double>(this);
                m_entryPrice = new VariableSeries<double>(this);
            }

            protected override void StartCalc()
            {
                m_spreadClosFC.inst1 = Bars.Close;
                m_spreadClosFC.inst2 = (leg2 == 0) ? Bars.Close : BarsOfData(2).Close;
                m_spreadClosFC.inst3 = (leg3 == 0) ? Bars.Close : BarsOfData(3).Close;
                m_spreadClosFC.leg1 = leg1;
                m_spreadClosFC.leg2 = leg2;
                m_spreadClosFC.leg3 = leg3;
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
                                m_pnl.Value = m_pnl[1] + (m_spreadClosFC[0] - m_spreadClosFC[1]);
                            if (calltype == "strat")
                                m_pnl.Value = 0;
                        }
                        if (m_pos[1] < 0)   // just close short pos
                        {
                            if (calltype == "indic")
                                m_pnl.Value = m_pnl[1] + (m_spreadClosFC[1] - m_spreadClosFC[0]);
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
                            m_entryPrice.Value = m_spreadClosFC[0];
                        }
                        if (calltype == "strat")
                        {
                            if (m_pos.Value > 0)
                                m_pnl.Value = m_spreadClosFC[0] - m_spreadClosFC[1];
                            if (m_pos.Value < 0)
                                m_pnl.Value = m_spreadClosFC[1] - m_spreadClosFC[0];
                        }
                    }
                    if (m_pos.Value > 0 && m_pos[1] > 0)    // existing long
                    {
                        if (m_pos[2] < 0)    // just reverse from short
                            m_pnl.Value = m_spreadClosFC[0] - m_spreadClosFC[1];
                        else       
                            m_pnl.Value = m_pnl[1] + (m_spreadClosFC[0] - m_spreadClosFC[1]);
                    }
                    if (m_pos.Value < 0 && m_pos[1] < 0)    // existing short
                    {
                        if (m_pos[2] > 0)   // just reverse from long
                            m_pnl.Value = m_spreadClosFC[1] - m_spreadClosFC[0]; 
                        else 
                            m_pnl.Value = m_pnl[1] + (m_spreadClosFC[1] - m_spreadClosFC[0]);
                    }
                    if (m_pos.Value > 0 && m_pos[1] < 0)    // short reverse to long
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = m_pnl[1] + (m_spreadClosFC[1] - m_spreadClosFC[0]); // pnl for previous short position, pnl for current long position is 0
                        }
                        if (calltype == "strat")
                        {
                            m_pnl.Value = m_spreadClosFC[0] - m_spreadClosFC[1]; // only pnl for long position
                        }
                    }
                    if (m_pos.Value < 0 && m_pos[1] > 0)    // long reverse to short
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = m_pnl[1] + (m_spreadClosFC[0] - m_spreadClosFC[1]); // pnl for previous long position, pnl for current short position is 0
                        }
                        if (calltype == "strat")
                        {
                            m_pnl.Value = m_spreadClosFC[1] - m_spreadClosFC[0];  // pnl for long already closed, only pnl for short
                        }
                    }
                }
                return m_pnl.Value;
            }
        }
    }
}