using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class gSpread : IndicatorObject 
    {
		public gSpread(object _ctx):base(_ctx){}
		
        private IPlotObject plot1;
		
        protected override void Create() 
        {
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
		}
		protected override void StartCalc() 
        {

		}
		protected override void CalcBar()
        {
			plot1.Set(Bars.Close[0]);
		}
	}
}