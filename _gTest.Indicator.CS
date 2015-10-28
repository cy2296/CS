using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator{
	public class _gTest : IndicatorObject 
    {
        private System.Text.StringBuilder csv;  // for debug only

        private _gUtility_MarketsOpened m_marketopen;

        private bool m_prevBarEndedSession = false;
        private _MK_CurrDHigh m_currDHigh;
        private _MK_CurrDLow m_CurrDLow;

        private _gBarOpen m_open;
        private _gBarClose m_close;
        private _gBarHigh m_high;
        private _gBarLow m_low;

		public _gTest(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
        }

        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
        private IPlotObject plot4;
       
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int interval { get; set; }

        protected override void Create()
        {
            csv = new StringBuilder();

            m_marketopen = new _gUtility_MarketsOpened(this);
            m_open = new _gBarOpen(this);
            m_close = new _gBarClose(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_currDHigh = new _MK_CurrDHigh(this);
            m_CurrDLow = new _MK_CurrDLow(this);

            plot1 = AddPlot(new PlotAttributes("Open", EPlotShapes.LeftTick, Color.Red));
            plot2 = AddPlot(new PlotAttributes("High", EPlotShapes.BarHigh, Color.Red));
            plot3 = AddPlot(new PlotAttributes("Low", EPlotShapes.BarLow, Color.Red));
            plot4 = AddPlot(new PlotAttributes("Close", EPlotShapes.RightTick, Color.Red));

            
		}
		protected override void StartCalc() 
        {
            m_marketopen.leg1 = leg1;
            m_marketopen.leg2 = leg2;
            m_marketopen.leg3 = leg3;
            m_open.leg1 = leg1;
            m_open.leg2 = leg2;
            m_open.leg3 = leg3;
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
            m_low.leg1 = leg1;
            m_low.leg2 = leg2;
            m_low.leg3 = leg3;

            m_currDHigh.Data = m_high;
            m_CurrDLow.Data = m_low;
		}
		protected override void CalcBar()
        {
            if (m_marketopen[0])
            {
                var open = Math.Round(m_open[0], 2);
                var close = Math.Round(m_close[0], 2);
                var high = Math.Round(m_high[0], 2);
                var low = Math.Round(m_low[0], 2);

                if (close >= open)
                {
                    plot1.Set(open, Color.LawnGreen);
                    plot2.Set(m_currDHigh[0], Color.Gray);
                    plot3.Set(m_CurrDLow[0], Color.Gray);
                    plot4.Set(close, Color.LawnGreen);
                }
                else
                {
                    plot1.Set(open, Color.Red);
                    plot2.Set(m_currDHigh[0], Color.Gray);
                    plot3.Set(m_CurrDLow[0], Color.Gray);
                    plot4.Set(close, Color.Red);
                }              
            }

            ////if (Bars.CurrentBar < 500)
            ////{
            ////    var x = Bars.TimeValue.Date;
            ////    var a = Bars.TimeValue.TimeOfDay;
            ////    var b = Bars.CurrentBar;
            ////    var c = Bars.Close[0] * leg1 + BarsOfData(2).Close[0] * leg2 + BarsOfData(3).Close[0] * leg3;
            ////    var y = StrategyInfo.MarketPosition;
            ////    var d = m_sprdPnl[0];
            ////    var e = m_atr.Value;
            ////    var f = (Math.Abs(m_sprdPnl[0]) >= m_atr.Value) ? 1 : 0;

            ////    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}{8}",
            ////    x, a, b, c, y, d, e, f, System.Environment.NewLine);
            ////    csv.Append(newLine);
            ////    System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
            ////}


		}
	}
}