using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy 
{
	public class _gTestStrat2 : SignalObject 
    {

        private System.Text.StringBuilder csv;  // for debug only


        private _gUtility_MarketsOpened m_marketopen;
        private _gUtility_TimeToTrade m_timetotrade;
        private _gBarClose m_close;
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _gSpreadPnlFC m_pnl;

        private double m_lastprice;
        private int m_countpos;
        private int m_counttime;
        private VariableSeries<int> m_pos;

        private _MK_PrevDHigh m_prevDHigh;
        private _MK_PrevDLow m_prevDLow;


		public _gTestStrat2(object _ctx):base(_ctx)
        {
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;

            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
        }

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public double move { get; set; }
        [Input]
        public double stop { get; set; }
        [Input]
        public double target { get; set; }
        
        private IOrderMarket m_random_LE_Mkt;
        private IOrderMarket m_random_LX_Mkt;
        private IOrderMarket m_random_SE_Mkt;
        private IOrderMarket m_random_SX_Mkt;
		
        protected override void Create()
        {

            csv = new StringBuilder();
            m_marketopen = new _gUtility_MarketsOpened(this);
            m_timetotrade = new _gUtility_TimeToTrade(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_close = new _gBarClose(this);
            m_pnl = new _gSpreadPnlFC(this);

            m_pos = new VariableSeries<int>(this);

            m_prevDHigh = new _MK_PrevDHigh(this);
            m_prevDLow = new _MK_PrevDLow(this);

            m_random_LE_Mkt = OrderCreator.MarketThisBar
                 (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_random_LX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_random_SE_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_random_SX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            m_countpos = 0;
            m_counttime = 0;
            m_marketopen.leg1 = leg1;
            m_marketopen.leg2 = leg2;
            m_marketopen.leg3 = leg3;

            m_timetotrade.StartTime = "9:30";
            m_timetotrade.EndTime = "15:58";

            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
            m_low.leg1 = leg1;
            m_low.leg2 = leg2;
            m_low.leg3 = leg3;
            m_pnl.leg1 = leg1;
            m_pnl.leg2 = leg2;
            m_pnl.leg3 = leg3;
            m_pnl.calltype = "strat";

            m_prevDHigh.Data = m_high;
            m_prevDLow.Data = m_low;
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            if (m_marketopen[0])
            {
                var mid = 0.5 * (m_prevDHigh[0] + m_prevDLow[0]);
                var midHigh = 0.5 * (mid + m_prevDHigh[0]);
                var midLow = 0.5 * (mid + m_prevDLow[0]);

                ++m_counttime;
                if (m_pos[0] == 0
                    && m_countpos == 0
                    && m_timetotrade[0]
                    //&& m_counttime < 120
                    )
                {
                    //m_random_LE_Mkt.Send();   // original
                    if (//m_close[0] < m_lastprice 
                        (m_close[0] < midLow && m_close[0] > m_prevDLow[0])
                        || 
                        (m_close[0] > mid && m_close[0] < midHigh)
                        )
                        m_random_LE_Mkt.Send();
                    if (//m_close[0] > m_lastprice
                        (m_close[0] > midHigh && m_close[0] < m_prevDHigh[0])
                        || 
                        (m_close[0] < mid && m_close[0] > midLow)
                        )
                        m_random_SE_Mkt.Send();
                    m_countpos = 1;
                }

                //if ((m_pnl[0] < 0 && Math.Abs(m_pnl[0]) > Math.Abs(0.35))
                //    || (m_pnl[0] > 0 && Math.Abs(m_pnl[0]) > Math.Abs(0.5))
                //    )
                //{
                //    if (m_pos[0] > 0)
                //        m_random_LX_Mkt.Send();
                //    if (m_pos[0] < 0)
                //        m_random_SX_Mkt.Send();
                //}

                if (!m_timetotrade[0]
                    //|| m_counttime == 120
                    )
                {
                    if (m_pos[0] > 0)
                        m_random_LX_Mkt.Send();
                    if (m_pos[0] < 0)
                        m_random_SX_Mkt.Send();

                    m_lastprice = m_close[0];
                    m_countpos = 0;
                    //m_counttime = 0;
                }
                //if (Bars.CurrentBar < 1000)
                //{
                //    var a = Bars.TimeValue.Date;
                //    var b = Bars.TimeValue.TimeOfDay;
                //    var c = Bars.CurrentBar;
                //    var d = m_pos[0];
                //    var e = m_pnl[0];
                //    var f = m_prevDHigh[0];
                //    var g = m_prevDLow[0];
                //    //var h = m_sprdPnl[0];
                //    //var i = m_stopPos[0];

                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}{7}",
                //    a, b, c, d, e, f, g, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    System.IO.File.WriteAllText("H:/eSignalData/debug13.csv", csv.ToString());
                //}
            }
		}
	}
}