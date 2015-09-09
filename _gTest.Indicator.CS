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

        private _gBarOpen m_open;
        private _gBarClose m_close;

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

        protected override void Create()
        {
            m_open = new _gBarOpen(this);
            m_close = new _gBarClose(this);
            plot1 = AddPlot(new PlotAttributes("highest1", EPlotShapes.Line, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("highest2", EPlotShapes.Line, Color.Aqua));
            plot3 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarHigh, Color.Yellow));
            plot4 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarLow, Color.Yellow));

            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
            m_open.leg1 = leg1;
            m_open.leg2 = leg2;
            m_open.leg3 = leg3;
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
		}
		protected override void CalcBar()
        {
            ////if (Bars.StatusLine.Time == BarsOfData(2).StatusLine.Time
            ////    && BarsOfData(2).StatusLine.Time == BarsOfData(3).StatusLine.Time)
            //if (Bars.TimeValue.Date == BarsOfData(2).TimeValue.Date
            //    && BarsOfData(2).TimeValue.Date == BarsOfData(3).TimeValue.Date)
            //{
            //    m_close.Value = Bars.Close[0] * leg1
            //         + BarsOfData(2).Close[0] * leg2
            //        + BarsOfData(3).Close[0] * leg3;
            //    plot1.Set(m_close[0]);
            //}

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