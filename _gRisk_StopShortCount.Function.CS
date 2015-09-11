using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gRisk_StopShortCount : FunctionSeries<System.Double>
		{
            private _gPnlFC m_pnl;
            private _gUtility_TimeToTrade m_timeToTrade;
            private VariableSeries<int> m_pos;
            private VariableSeries<int> m_count;

			public _gRisk_StopShortCount(CStudyControl _master) : base(_master) { }
			public _gRisk_StopShortCount(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public string startTime { get; set; }
            public string endTime { get; set; }
            public ISeries<Double> price { get; set; }
            public string calltype { get; set; }
            public double stopCriteria { get; set; }

			protected override void Create()
			{
                m_pnl = new _gPnlFC(this);
                m_timeToTrade = new _gUtility_TimeToTrade(this);

                m_pos = new VariableSeries<int>(this);
                m_count = new VariableSeries<int>(this);
			}

			protected override void StartCalc()
			{
                m_pnl.price = price;
                m_pnl.calltype = calltype;
                m_timeToTrade.StartTime = startTime;
                m_timeToTrade.EndTime = endTime;
			}

			protected override System.Double CalcBar()
			{
                m_pos.Value = StrategyInfo.MarketPosition;
                if (!m_timeToTrade[0])
                    m_count.Value = 0;
                else
                {
                    if (m_pos[1] < 0 && m_pos[0] >= 0)
                    {
                        if (calltype == "indic")
                        {
                            if (m_pnl[0] <= -Math.Abs(stopCriteria))
                                m_count.Value = m_count[1] + 1;
                            else
                                m_count.Value = m_count[1];
                        }
                        if (calltype == "strat")
                        {
                            if (m_pnl[1] <= -Math.Abs(stopCriteria))
                                m_count.Value = m_count[1] + 1;
                            else
                                m_count.Value = m_count[1];
                        }
                    }
                    else
                        m_count.Value = m_count[1];   
                }
                return m_count.Value;
            }
		}
	}
}