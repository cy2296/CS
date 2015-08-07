using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSpreadATR : IndicatorObject 
    {
		public _gSpreadATR(object _ctx):base(_ctx){}

		private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;

		protected override void Create() {
			// create variable objects, function objects, plot objects etc.
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
            plot2 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
		    plot3 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar(){
			// indicator logic 
			plot1.Set(Bars.Close[0]);
            plot2.Set(Bars.Close[0]);
            plot3.Set(Bars.Close[0]);
		}
	}
}