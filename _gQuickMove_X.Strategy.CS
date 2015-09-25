using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy {
	public class _gQuickMove_X : SignalObject 
    {
        private System.Text.StringBuilder csv;  // for debug only
        
        private VariableSeries<int> m_pos;
        private _gBarClose m_close;
        private VariableSeries<double> m_slope;
        private VariableSeries<double> m_slopeslope;
        private AverageFC m_slopeAvg;
        private _gPnlFC m_pnl;

		public _gQuickMove_X(object _ctx):base(_ctx)
        {
            //// XLE
            //leg1 = 1;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //lookback = 8;
            //length = 390;

            //// SPY
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //lookback = 8;
            //length = 390;

            // CL
            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            lookback = 8;
            length = 390;
        }
        
        private IOrderMarket m_QuickMove_LX_Mkt;
        private IOrderMarket m_QuickMove_SX_Mkt;

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int lookback { get; set; }
        [Input]
        public int length { get; set; }

		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_close = new _gBarClose(this);
            m_slope = new VariableSeries<double>(this);
            m_slopeslope = new VariableSeries<double>(this);
            m_slopeAvg = new AverageFC(this);
            m_pnl = new _gPnlFC(this);
            m_QuickMove_LX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "QLX", EOrderAction.Sell));
            m_QuickMove_SX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "QSX", EOrderAction.BuyToCover));

            csv = new StringBuilder();

		}
		protected override void StartCalc() 
        {
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            //m_slopeAvg.price = m_slope;
            m_slopeAvg.price = m_slopeslope;
            m_slopeAvg.length = length;
            m_pnl.price = m_close;
            m_pnl.calltype = "strat";
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            m_slope.Value = (m_close[0] - m_close[lookback]) / lookback;
            m_slopeslope.Value = (m_slope[0] - m_slope[lookback]) / lookback;

            var m_sdev = m_slope.StandardDeviationCustom(length, 1);
            var upband = m_slopeAvg[0] + 2 * m_sdev;
            var dnband = m_slopeAvg[0] - 2 * m_sdev;
            //var m_sdev1 = m_slopeslope.StandardDeviationCustom(length, 1);
            //var upband = m_slopeAvg[0] + 2 * m_sdev1;
            //var dnband = m_slopeAvg[0] - 2 * m_sdev1;
            
            if (m_pnl[0] > 0)
            {
                if (m_pos[0] > 0 && m_slope[0] > upband)
                    m_QuickMove_LX_Mkt.Send();
                if (m_pos[0] < 0 && m_slope[0] < dnband)
                    m_QuickMove_SX_Mkt.Send();
            }

            //if (Bars.CurrentBar < 2000)
            //{
            //    var x = Bars.TimeValue.Date;
            //    var a = Bars.TimeValue.TimeOfDay;
            //    var b = Bars.CurrentBar;
            //    var c = m_pnl[0];
            //    var d = m_slope[0];
            //    var e = upband;
            //    var f = dnband;
            //    var g = m_pos[0];

            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}{8}",
            //    x, a, b, c, d, e, f, g, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
            //}
		}
	}
}