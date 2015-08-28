using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTest : IndicatorObject 
    {
        private HighestFC highest1;
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
            highest1 = new HighestFC(this);
       
            plot1 = AddPlot(new PlotAttributes("highest1", EPlotShapes.Line, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("highest2", EPlotShapes.Line, Color.Aqua));
            plot3 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarHigh, Color.Yellow));
            plot4 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarLow, Color.Yellow));

		}
		protected override void StartCalc() 
        {
            highest1.pricevalue = Bars.High;
            highest1.len = 5;
		}
		protected override void CalcBar()
        {
            plot1.Set(highest1[0]);

		}
	}
}