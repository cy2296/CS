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
		public sealed class _gRisk_StopLongCount : FunctionSeries<System.Int32>
		{

            private System.Text.StringBuilder csv;  // for debug only

            private _gPnlFC m_pnl;
            private _gUtility_TimeToTrade m_timeToTrade;
            private VariableSeries<int> m_pos;
            private VariableSeries<int> m_count;

            public _gRisk_StopLongCount(CStudyControl _master) : base(_master) { }
			public _gRisk_StopLongCount(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public string startTime { get; set; }
            public string endTime { get; set; }
            public ISeries<Double> price { get; set; }
            public string calltype { get; set; }
            public double stopCriteria { get; set; }

			protected override void Create()
			{
                csv = new StringBuilder();

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

			protected override System.Int32 CalcBar()
			{
                m_pos.Value = StrategyInfo.MarketPosition;
                if (!m_timeToTrade[0])
                    m_count.Value = 0;
                else 
                {
                    if (m_pos[1] > 0 && m_pos[0] <= 0)
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
                //if (Bars.CurrentBar < 2000)
                //{
                //    var a = Bars.TimeValue.Date;
                //    var b = Bars.TimeValue.TimeOfDay;
                //    var c = Bars.CurrentBar;
                //    var d = m_pos[0];
                //    var e = m_count[0];
                //    //var f = m_twoLineGap[0];
                //    //var g = m_mp[0];
                //    //var h = m_sprdPnl[0];
                //    //var i = m_stopPos[0];

                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}{5}",
                //    a, b, c, d, e, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    System.IO.File.WriteAllText("H:/eSignalData/debug11.csv", csv.ToString());
                //}
                return m_count.Value;
			}
		}
	}
}