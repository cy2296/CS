using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator{
	public class _gTest2 : IndicatorObject 
    {
        private System.Text.StringBuilder csv;  // for debug only
        private VariableSeries<double> m_trend;
        private int m_upcount = 0;
        private int m_dncount = 0;

        private _gBarTipical m_average;
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _gBarClose m_close;
        private _gBarOpen m_open;

        private HighestFC m_highest;
        private LowestFC m_lowest;
        private VariableSeries<double> m_slope;
        private int lowestPoint = 0;
        private int highestPoint = 0;

		
        public _gTest2(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            lookback = 30;
        }
		private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int lookback { get; set; }

		protected override void Create() 
        {
            m_highest = new HighestFC(this);
            m_lowest = new LowestFC(this);
            m_slope = new VariableSeries<double>(this);

            m_trend = new VariableSeries<double>(this);



            m_average = new _gBarTipical(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_close = new _gBarClose(this);
            m_open = new _gBarOpen(this);
            plot1 = AddPlot(new PlotAttributes("trend", EPlotShapes.Histogram, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("slope", EPlotShapes.Histogram, Color.Aqua));
            plot3 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarHigh, Color.Yellow));
            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
            m_average.leg1 = leg1;
            m_average.leg2 = leg2;
            m_average.leg3 = leg3;
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
            m_low.leg1 = leg1;
            m_low.leg2 = leg2;
            m_low.leg3 = leg3;
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_open.leg1 = leg1;
            m_open.leg2 = leg2;
            m_open.leg3 = leg3;

            m_highest.pricevalue = m_high;
            m_highest.len = lookback;
            m_lowest.pricevalue = m_low;
            m_lowest.len = lookback;
		}
        protected override void CalcBar()
        {
           if (m_close[0] > m_open[0]       // up bar
                && m_close[0] > m_high[1]
                && m_close[0] > m_high[2]
                )
            {
                m_trend.Value = 1;      // first up trend
               // plot2.Set(ComputeSlope(m_trend, 1));
            }
            else if (m_close[0] < m_open[0]
                && m_close[0] < m_low[1]
                && m_close[0] < m_low[2]
                )
            {
                m_trend.Value = -1;
               // plot2.Set(ComputeSlope(m_trend, -1));
            }
            else
                m_trend.Value = m_trend[1];

            //if (m_trend[1] != m_trend[0])
            //    plot2.Set(ComputeSlope(m_trend, m_trend[1]));

            //plot1.Set(m_trend[0]);           


            if (m_close[0] == m_lowest[0])
                lowestPoint = 0;
            else 
                ++lowestPoint;
            if (m_close[0] == m_highest[0])
                highestPoint = 0;
            else 
                ++highestPoint;


            double dis1 = Math.Abs(m_close[0] - m_lowest[0]);
            double dis2 = Math.Abs(m_close[0] - m_highest[0]);
            
            double upslope = (lowestPoint != 0) ? (m_close[0] - m_lowest[0]) / lowestPoint : 0;
            double dnslope = (highestPoint != 0) ? (m_close[0] - m_highest[0]) / highestPoint : 0;

            if (dis1 <= dis2)
                m_slope.Value = dnslope;
            else
                m_slope.Value = upslope;

           // m_slope.Value = Math.Max(Math.Abs(upslope), Math.Abs(dnslope));
            plot2.Set(m_slope[0]);


            //if (Bars.CurrentBar < 2000)
            //{
            //    var a = Bars.TimeValue.Date;
            //    var b = Bars.TimeValue.TimeOfDay;
            //    var c = Bars.CurrentBar;
            //    var d = m_pos[0];
            //    var e = m_pnl[0];
            //    //var f = m_twoLineGap[0];
            //    //var g = m_mp[0];
            //    //var h = m_sprdPnl[0];
            //    //var i = m_stopPos[0];

            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}{5}",
            //    a, b, c, d, e, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("H:/eSignalData/debug13.csv", csv.ToString());
            //}
        }
        private double ComputeSlope(VariableSeries<double> trend, double value)
        {
            double count = 0;
            double slope = 0;
            //if (value == 1) // find up move
            //{
                double currhigh = m_high[1];
                double currlow = m_low[1];
                int x = 1;
                while (trend[x] == value)
                {
                    if (x > 1)
                    {
                        currhigh = Math.Max(currhigh, m_high[x]);
                        currlow = Math.Min(currlow, m_low[x]);
                    }
                    ++count;
                    ++x;
                }
                if (count > 1)
                    slope = (currhigh - currlow) / count;
            //}
            //if (value == -1)
            //{
                
            //}


            return slope;
        }
	}
}