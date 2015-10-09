using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator{
	public class _gSpread : IndicatorObject 
	{
        private System.Text.StringBuilder csv;  // for debug only

        private _gUtility_MarketsOpened m_marketopened;
        private _gUtility_TimeToTrade m_todaysession;
        private bool m_prevBarEndedSession = false;

        private _gBarOpen m_open;
        private _gBarClose m_close;
        private int m_todaybarcount = 0;
        private double m_currSessionSum;
        private double m_currDeviationSum;

        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
        private IPlotObject plot4;

        private IPlotObject plot5;
        private IPlotObject plot6;
        private IPlotObject plot7;
        private IPlotObject plot8;
        private IPlotObject plot9;

		public _gSpread(object _ctx):base(_ctx)
		{
			leg1 = 1.0;
			leg2 = -0.333;
            leg3 = -1.5;
		}
		[Input]
		public double leg1 { get; set; }

		[Input]
		public double leg2 { get; set; }

        [Input]
        public double leg3 { get; set; }	
        
		protected override void Create() 
		{
            csv = new StringBuilder();

            m_marketopened = new _gUtility_MarketsOpened(this);
            m_todaysession = new _gUtility_TimeToTrade(this);

            m_open = new _gBarOpen(this);
            m_close = new _gBarClose(this);
            plot1 = AddPlot(new PlotAttributes("Open", EPlotShapes.LeftTick, Color.Red));
            plot2 = AddPlot(new PlotAttributes("High", EPlotShapes.BarHigh, Color.Red));
            plot3 = AddPlot(new PlotAttributes("Low", EPlotShapes.BarLow, Color.Red));
            plot4 = AddPlot(new PlotAttributes("Close", EPlotShapes.RightTick, Color.Red));
            plot5 = AddPlot(new PlotAttributes("Close", EPlotShapes.Line, Color.Red));
            plot6 = AddPlot(new PlotAttributes("Close", EPlotShapes.Line, Color.Red));
            plot7 = AddPlot(new PlotAttributes("Close", EPlotShapes.Line, Color.Red));
            plot8 = AddPlot(new PlotAttributes("Close", EPlotShapes.Line, Color.Red));
            plot9 = AddPlot(new PlotAttributes("Close", EPlotShapes.Line, Color.Red));
		}
		protected override void StartCalc() 
		{
            m_marketopened.leg1 = leg1;
            m_marketopened.leg2 = leg2;
            m_marketopened.leg3 = leg3;
            m_todaysession.StartTime = "09:30";
            m_todaysession.EndTime = "15:00";
            m_open.leg1 = leg1;
            m_open.leg2 = leg2;
            m_open.leg3 = leg3;
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
		}
		protected override void CalcBar()
		{
            var open = Math.Round(m_open[0], 2);
            var close = Math.Round(m_close[0], 2);
            if (close >= open)
            {
                plot1.Set(open, Color.LawnGreen);
                plot2.Set(close, Color.LawnGreen);
                plot3.Set(open, Color.LawnGreen);
                plot4.Set(close, Color.LawnGreen);
            }
            else
            {
                plot1.Set(open, Color.Red);
                plot2.Set(open, Color.Red);
                plot3.Set(close, Color.Red);
                plot4.Set(close, Color.Red);
            }
            if (m_prevBarEndedSession)
            {
                m_todaybarcount = 1;
                m_prevBarEndedSession = false;
                m_currSessionSum = m_close[0];
                m_currDeviationSum = 0;
            }
            else
            {
                ++m_todaybarcount;
                m_currSessionSum += m_close[0];
            }
            if (Bars.LastBarInSession)
            {
                m_prevBarEndedSession = true;
                m_currSessionSum = 0;
                m_currDeviationSum = 0;
            }


            double mean = m_currSessionSum / m_todaybarcount;
            m_currDeviationSum += Math.Pow((m_close[0] - mean), 2);
            double stdev = Math.Sqrt(m_currDeviationSum / m_todaybarcount);
            if (!Bars.LastBarInSession)
            {
                plot5.Set(mean, Color.Yellow);
                plot6.Set(mean + stdev, Color.White);
                plot7.Set(mean - stdev, Color.White);
                plot8.Set(mean - 2 * stdev, Color.White);
                plot9.Set(mean + 2 * stdev, Color.White);
            }
            //if (Bars.CurrentBar < 1000)
            //{
            //    var x = Bars.TimeValue.Date;
            //    var a = Bars.TimeValue.TimeOfDay;
            //    var b = Bars.CurrentBar;
            //    var c = m_todaybarcount;
            //    var d = m_close[0];
            //    var e = mean;
            //    var f = m_currSessionSum;
            //    var g = m_currDeviationSum;
            //    var h = stdev;

            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}{9}",
            //    x, a, b, c, d, e, f, g, h, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("C:/data/debug1.csv", csv.ToString());
            //}
		}
	}
}