 using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator
{
	public class _gTest3 : IndicatorObject 
    {
        private System.Text.StringBuilder csv;  // for debug only
        private _gBarClose m_close;
        private _gBarTipical m_average;

        private VariableSeries<double> m_slope;
        private AverageFC m_slopeAvg;

		public _gTest3(object _ctx):base(_ctx)
        {
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;
            lookback1 = 8;
            length = 390;
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
        public int lookback1 { get; set; }
        [Input]
        public int length { get; set; }
        


        protected override void Create() 
        {
            m_close = new _gBarClose(this);
            m_average = new _gBarTipical(this);

            m_slope = new VariableSeries<double>(this);
            m_slopeAvg = new AverageFC(this);


            plot1 = AddPlot(new PlotAttributes("slope", EPlotShapes.Histogram, Color.Aqua));
            plot2 = AddPlot(new PlotAttributes("up", EPlotShapes.Line, Color.Yellow));
            plot3 = AddPlot(new PlotAttributes("dn", EPlotShapes.Line, Color.Yellow));
            
            
            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_average.leg1 = leg1;
            m_average.leg2 = leg2;
            m_average.leg3 = leg3;
            m_slopeAvg.price = m_slope;
            m_slopeAvg.length = length;
            var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}{7}",
            "Date", "Time", "CurrBar", "Spread", "RSI", "BB_Up", "BB_Dn", System.Environment.NewLine);
            csv.Append(newLine);
            System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
		}
		protected override void CalcBar()
        {
            m_slope.Value = (m_close[0] - m_close[lookback1]) / lookback1;
            var m_sdev = m_slope.StandardDeviationCustom(length, 1);
            var upband = m_slopeAvg[0] + 2 * m_sdev;
            var dnband = m_slopeAvg[0] - 2 * m_sdev;

            plot1.Set(m_slope[0]);
            plot2.Set(upband);
            plot3.Set(dnband);


            //if (Bars.CurrentBar < 500)
            //{
            //    var x = Bars.TimeValue.Date;
            //    var a = Bars.TimeValue.TimeOfDay;
            //    var b = Bars.CurrentBar;
            //    var c = m_spread[0];
            //    var d = m_spreadRsiFC[0];
            //    var e = m_spreadRsiUpBbFC[0];
            //    var f = m_spreadRsiDnBbFC[0];
            
            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}{7}",
            //    x, a, b, c, d, e, f, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
            //}
            
		}
	}
}