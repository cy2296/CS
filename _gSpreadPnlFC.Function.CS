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
            private double spreadBigPtVal;

			public _gSpreadPnlFC(CStudyControl _master) : base(_master) { }
			public _gSpreadPnlFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public double bigPt1 { get; set; }
            public double bigPt2 { get; set; }
            public double bigPt3 { get; set; }
            public double slippage { get; set; }
            public string calltype { get; set; }

            // variable for _gSpreadFC
            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }
            // 
            // assign spreadFC
            void assign(_gSpreadFC sprdFunc, ISeries<Double> inst1,
                        ISeries<Double> inst2, ISeries<Double> inst3)
            {
                sprdFunc.inst1 = inst1;
                sprdFunc.inst2 = inst2;
                sprdFunc.inst3 = inst3;
                sprdFunc.leg1 = this.leg1;
                sprdFunc.leg2 = this.leg2;
                sprdFunc.leg3 = this.leg3;
            }	

			protected override void Create()
			{
                m_spreadClosFC = new _gSpreadFC(this);
                m_pos = new VariableSeries<int>(this);
                m_pnl = new VariableSeries<double>(this);
                m_entryPrice = new VariableSeries<double>(this);
			}

			protected override void StartCalc()
			{
                assign(m_spreadClosFC, Bars.Close, BarsOfData(2).Close,
                    (leg3 == 0) ? Bars.Close : BarsOfData(3).Close);
                spreadBigPtVal = bigPt1 * leg1 + bigPt2 * leg2 + bigPt3 * leg3;
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
                        var dollarRatio = Math.Abs(m_pos[1] / leg1) * spreadBigPtVal;
                        var posFactor = Math.Abs(m_pos[1] / leg1);
                        if (m_pos[1] > 0)
                        {
                            if (calltype == "indic")
                                m_pnl.Value = m_pnl[1]
                                + (m_spreadClosFC[0] - m_spreadClosFC[1]) * dollarRatio
                                - 2 * slippage * posFactor;
                            if (calltype == "strat")
                                m_pnl.Value = 0;

                        }
                        if (m_pos[1] < 0)   // just close short pos
                        {
                            if (calltype == "indic")
                                m_pnl.Value = m_pnl[1]
                                + (m_spreadClosFC[1] - m_spreadClosFC[0]) * dollarRatio
                                - 2 * slippage * posFactor;
                            if (calltype == "strat")
                                m_pnl.Value = 0;
                        }
                    }
                }
                else  // has position
                {
                    var dollarRatio = Math.Abs(m_pos.Value / leg1) * spreadBigPtVal;
                    var posFactor = Math.Abs(m_pos.Value / leg1);
                    if (m_pos[1] == 0)   // new open position
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = -2 * slippage * posFactor;
                            m_entryPrice.Value = m_spreadClosFC[0];
                        }
                        if (calltype == "strat")
                        {
                            if (m_pos.Value > 0)
                                m_pnl.Value = (m_spreadClosFC[0] - m_spreadClosFC[1]) * dollarRatio - 2 * slippage * posFactor;
                            if (m_pos.Value < 0)
                                m_pnl.Value = (m_spreadClosFC[1] - m_spreadClosFC[0]) * dollarRatio - 2 * slippage * posFactor;
                        }
                    }
                    if (m_pos.Value > 0 && m_pos[1] > 0)    // existing long
                    {
                        m_pnl.Value = m_pnl[1]
                                    + (m_spreadClosFC[0] - m_spreadClosFC[1]) * dollarRatio;
                    }
                    if (m_pos.Value < 0 && m_pos[1] < 0)    // existing short
                    {
                        m_pnl.Value = m_pnl[1]
                                    + (m_spreadClosFC[1] - m_spreadClosFC[0]) * dollarRatio;
                    }
                    if (m_pos.Value > 0 && m_pos[1] < 0)    // short reverse to long
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = m_pnl[1]
                                    + (m_spreadClosFC[1] - m_spreadClosFC[0]) * dollarRatio
                                    - 2 * slippage * posFactor;
                        }
                        if (calltype == "strat")
                        { 
                            m_pnl.Value = m_pnl[1]
                                        + (m_spreadClosFC[0] - m_spreadClosFC[1]) * dollarRatio
                                        - 2 * slippage * posFactor;    
                        }
                    }
                    if (m_pos.Value < 0 && m_pos[1] > 0)    // long reverse to short
                    {
                        if (calltype == "indic")
                        {
                            m_pnl.Value = m_pnl[1]
                                    + (m_spreadClosFC[0] - m_spreadClosFC[1]) * dollarRatio
                                    - 2 * slippage * posFactor;
                        }
                        if (calltype == "strat")
                        {
                            m_pnl.Value = m_pnl[1]
                                        + (m_spreadClosFC[1] - m_spreadClosFC[0]) * dollarRatio
                                        - 2 * slippage * posFactor;
                        }
                    }
                }
                return m_pnl.Value;
			}
		}
	}
}