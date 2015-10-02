using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;


using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy {
	public class _gQuickMove2_X : SignalObject 
    {
        private System.Text.StringBuilder csv;  // for debug only

        private _gPnlFC m_firstpnl;
        private _gBarClose m_close;

        private VariableSeries<int> m_pos;
        private VariableSeries<int> m_timecount;
        private VariableSeries<double> m_secondpnl;
        private bool m_secondpos = false;

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int postime { get; set; }
        [Input]
        public double profittar { get; set; }

		public _gQuickMove2_X(object _ctx):base(_ctx)
        {
            //// XLE
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;
            postime = 20;
            profittar = 0.2;

            //// SPY
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //postime = 20;
            //profittar = 0.2;

            //// CL
            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //postime = 20;
            //profittar = 0.2;
        }
        private IOrderMarket m_reverse_LE1_Mkt;
        private IOrderMarket m_reverse_SE1_Mkt;

        private IOrderMarket m_QuickMove2_LX_Mkt;
        private IOrderMarket m_QuickMove2_SX_Mkt;

		protected override void Create() 
        {
            m_firstpnl = new _gPnlFC(this);
            m_close = new _gBarClose(this);

            m_pos = new VariableSeries<int>(this);
            m_timecount = new VariableSeries<int>(this);

            m_secondpnl = new VariableSeries<double>(this);

            m_reverse_LE1_Mkt = OrderCreator.MarketThisBar
        (new SOrderParameters(Contracts.Default, "RevL1", EOrderAction.Buy));
            m_reverse_SE1_Mkt = OrderCreator.MarketThisBar
        (new SOrderParameters(Contracts.Default, "RevS1", EOrderAction.SellShort));

            m_QuickMove2_LX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "QLX", EOrderAction.Sell, OrderExit.FromAll));
            m_QuickMove2_SX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "QSX", EOrderAction.BuyToCover, OrderExit.FromAll));

            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
            m_firstpnl.price = m_close;
            m_firstpnl.calltype = "strat";

            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            if (m_pos[0] == 0
               || (m_pos[0] > 0 && m_pos[1] > 0 && Math.Abs(m_pos[0]) > Math.Abs(m_pos[1]))  // add long
               || (m_pos[0] < 0 && m_pos[1] < 0 && Math.Abs(m_pos[0]) > Math.Abs(m_pos[1]))  // add short
               || (m_pos[0] > 0 && m_pos[1] < 0)  // reverse short to long
               || (m_pos[0] < 0 && m_pos[1] > 0)  // reverse long to short 
               )
            {
                m_timecount.Value = 0;
            }
            else
            {
                m_timecount.Value = m_timecount[1] + 1;
            }

            if (m_pos[0] > 0 && m_pos[1] > 0)    
            {
                if (Math.Abs(m_pos[0]) > Math.Abs(m_pos[1]))  // add long
                {
                    m_secondpnl.Value = m_close[0] - m_close[1];
                    m_secondpos = true;
                }
                else
                {
                    if (m_secondpos)
                        m_secondpnl.Value = m_secondpnl[1] + (m_close[0] - m_close[1]);
                    else
                        m_secondpnl.Value = 0;
                }
            }
            else if (m_pos[0] < 0 && m_pos[1] < 0)
            {
                if (Math.Abs(m_pos[0]) > Math.Abs(m_pos[1]))  // add short
                {
                    m_secondpnl.Value = m_close[1] - m_close[0];
                    m_secondpos = true;
                }
                else
                {
                    if (m_secondpos)
                        m_secondpnl.Value = m_secondpnl[1] + (m_close[1] - m_close[0]);
                    else
                        m_secondpnl.Value = 0;
                }
            }
            else
            {
                m_secondpnl.Value = 0;
                m_secondpos = false;
            }

            if ( (m_timecount[0] <= 50 && m_firstpnl[0] >= 0.35)
                 //(m_timecount[0] <= 50 && m_firstpnl[0] <= -Math.Abs(0.35))
               ||(m_timecount[0] <= 50 && m_secondpnl[0] >= 0.50)
               //||(m_timecount[0] > 50 && m_secondpnl[0] < 0)
                )
            //if (m_timecount[0] <= postime
            //    && (m_firstpnl[0] >= profittar || m_secondpnl[0] >= profittar)
            //    )
            {
                if (m_pos[0] > 0)
                {
                    m_QuickMove2_LX_Mkt.Send();
                    //m_reverse_SE1_Mkt.Send();
                }
                if (m_pos[0] < 0)
                {
                    m_QuickMove2_SX_Mkt.Send();
                    //m_reverse_LE1_Mkt.Send();
                }
            }

            //if (Bars.CurrentBar > 0
            //    && Bars.CurrentBar < 3000)
            //{
            //    var x = Bars.TimeValue.Date;
            //    var a = Bars.TimeValue.TimeOfDay;
            //    var b = Bars.CurrentBar;
            //    var c = m_pos[0];
            //    var d = m_firstpnl[0];
            //    var e = m_secondpnl[0];
            //    var f = m_timecount[0];
            //    var g = m_close[0];

            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}{8}",
            //    x, a, b, c, d, e, f, g, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
            //}
		}
	}
}