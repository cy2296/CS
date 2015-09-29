using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy {
	public class _gTestStrat3 : SignalObject {

        public _gTestStrat3(object _ctx):base(_ctx){}

        private System.Text.StringBuilder csv;  // for debug only

        private VariableSeries<int> m_mp;
        private _gPnlFC m_pnlfc;
        private VariableSeries<double> m_pnl_1;
        private VariableSeries<double> m_pnl_2;
        private double m_initpnl2;

        private _gUtility_TimeToTrade m_timetotrade;
        private int m_count = 0;

        private _gBarClose m_close;
        private VariableSeries<int> m_mp1;
        private VariableSeries<int> m_mp2;
        //private _gPnlFC2 m_pnl_1;
        //private _gPnlFC2 m_pnl_2;


        private IOrderMarket m_test_LE1_Mkt;
        private IOrderMarket m_test_LE2_Mkt;
        private IOrderMarket m_test_LX1_Mkt;
        private IOrderMarket m_test_LX2_Mkt;

        private IOrderMarket m_test_SE_Mkt;
        private IOrderMarket m_test_SX_Mkt;
		
        protected override void Create()
        {
            csv = new StringBuilder();

            m_mp = new VariableSeries<int>(this);
            m_timetotrade = new _gUtility_TimeToTrade(this);
            m_close = new _gBarClose(this);
            m_mp1 = new VariableSeries<int>(this);
            m_mp2 = new VariableSeries<int>(this);

            m_pnl_1 = new VariableSeries<double>(this);
            m_pnl_2 = new VariableSeries<double>(this);
            m_pnlfc = new _gPnlFC(this);
            
            //m_pnl_1 = new _gPnlFC2(this);
            //m_pnl_2 = new _gPnlFC2(this);
            
            m_test_LE1_Mkt = OrderCreator.MarketThisBar
               (new SOrderParameters(Contracts.Default, "L1", EOrderAction.Buy));
            m_test_LE2_Mkt = OrderCreator.MarketThisBar
               (new SOrderParameters(Contracts.Default, "L2", EOrderAction.Buy));
            m_test_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_test_LX1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX1", EOrderAction.Sell, OrderExit.FromAll));
            m_test_LX2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX2", EOrderAction.Sell, OrderExit.FromAll));
            m_test_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            m_timetotrade.StartTime = "11:50";
            m_timetotrade.EndTime = "13:00";
            m_close.leg1 = 1;
            m_close.leg2 = -.333;
            m_close.leg3 = -.5;

            m_pnlfc.price = m_close;
            m_pnlfc.calltype = "strat";

            //m_pnl_1.price = m_close;
            //m_pnl_1.position = m_mp1;

            //m_pnl_2.price = m_close;
            //m_pnl_2.position = m_mp2;
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;
            
            m_pnl_1.Value = m_pnlfc[0];

            if (
                (m_mp[1] > 0 && m_mp[0] > 0 && Math.Abs(m_mp[0]) > Math.Abs(m_mp[1]))
                || 
                (m_mp[1] < 0 && m_mp[0] < 0 && Math.Abs(m_mp[0]) > Math.Abs(m_mp[1]))
                ) 
                m_initpnl2 = m_pnlfc[0];

            m_pnl_2.Value = m_pnl_1[0] - m_initpnl2;
            
            if (m_timetotrade[0])
            {
                ++m_count;
                if (m_count == 1)
                {
                    m_test_LE1_Mkt.Send();
                }

                if (m_count == 60)
                {
                    m_test_LE2_Mkt.Send();
                }
            }
            else
            {
                m_count = 0;
                m_test_LX1_Mkt.Send();
                m_test_LX2_Mkt.Send();
                m_initpnl2 = 0;
            }


            m_mp.Value = StrategyInfo.MarketPosition;
            if (Bars.CurrentBar < 2000)
            {
                var x = Bars.TimeValue.Date;
                var a = Bars.TimeValue.TimeOfDay;
                var b = Bars.CurrentBar;
                var c = m_mp1[0];
                var d = m_pnl_1[0];
                var e = m_mp2[0];
                var f = m_pnl_2[0];
                var g = m_mp[0];

                var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}{8}",
                x, a, b, c, d, e, f, g, System.Environment.NewLine);
                csv.Append(newLine);
                System.IO.File.WriteAllText("C:/data/debug1.csv", csv.ToString());
            }
		}
	}
}