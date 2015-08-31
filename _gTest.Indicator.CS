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
        private _gBarHigh m_high;
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

        private System.Text.StringBuilder csv;  // for debug only
       
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }

        protected override void Create()
        {
            m_close = new _gBarClose(this);
            m_high = new _gBarHigh(this);
       
            plot1 = AddPlot(new PlotAttributes("highest1", EPlotShapes.Line, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("highest2", EPlotShapes.Line, Color.Aqua));
            plot3 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarHigh, Color.Yellow));
            plot4 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarLow, Color.Yellow));

            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
		}
		protected override void CalcBar()
        {
            plot1.Set(m_close[0]);
            plot2.Set(m_high[0]);

            if (Bars.CurrentBar < 500)
            {
                var x = Bars.TimeValue.Date;
                var y = Bars.TimeValue.TimeOfDay;
                var z = Bars.CurrentBar;

                var b = m_high[0];
                var d = m_close[0];

                var newLine = string.Format("{0}, {1}, {2}, {3}, {4}{5}",
                x, y, z, b, d, System.Environment.NewLine);
                csv.Append(newLine);
                System.IO.File.WriteAllText("H:/eSignalData/debug2.csv", csv.ToString());
            }
		}
	}
}