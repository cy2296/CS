using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class PPB : IndicatorObject 
	{
		public PPB(object _ctx):base(_ctx){}
		private IPlotObject plot1;
		private IPlotObject plot2;
		private IPlotObject plot3;
		private IPlotObject plot4;
		
		protected override void Create() 
		{
			plot1 = AddPlot(new PlotAttributes("Open", EPlotShapes.LeftTick, Color.Red));
			plot2 = AddPlot(new PlotAttributes("High", EPlotShapes.BarHigh, Color.Red));
			plot3 = AddPlot(new PlotAttributes("Low", EPlotShapes.BarLow, Color.Red));
			plot4 = AddPlot(new PlotAttributes("Close", EPlotShapes.RightTick, Color.Red));
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar(){
	
			plot1.Set(Bars.Open[0]/2);
			plot2.Set(Bars.High[0]/2);
			plot3.Set(Bars.Low[0]/2);
			plot4.Set(Bars.Close[0]/2);
		}
	}
}