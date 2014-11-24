using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTurnMetric_Delta : IndicatorObject {
		public _gTurnMetric_Delta(object _ctx):base(_ctx)
		{
			length = 10;
			vxWeight = 0.6;
			upDnRatio = 0.5;
		}
		
		private IPlotObject upES;
		private IPlotObject dnES;
		private IPlotObject upVX;
		private IPlotObject dnVX;
		private IPlotObject upDel;
		private IPlotObject dnDel;
		private IPlotObject totDel;

		private ISeries<double> esDelta { get; set; }
		private ISeries<double> vxDelta { get; set; }

		[Input]
		public int length { get; set; }

		[Input]
		public double vxWeight { get; set; }

		[Input]
		public double upDnRatio { get; set; }

		protected override void Create() {
			// create variable objects, function objects, plot objects etc.
			upES = AddPlot(new PlotAttributes("upES", 0, Color.Red));
			dnES = AddPlot(new PlotAttributes("dnES", 0, Color.Red));
			upVX = AddPlot(new PlotAttributes("upVX", 0, Color.Red));
			dnVX = AddPlot(new PlotAttributes("dnVX", 0, Color.Red));
			upDel = AddPlot(new PlotAttributes("Up", 0, Color.Red));
			dnDel = AddPlot(new PlotAttributes("Down", 0, Color.Red));
			totDel = AddPlot(new PlotAttributes("Total", 0, Color.Red));
		}
		protected override void StartCalc() {
			esDelta = Bars.Close;
			vxDelta = BarsOfData(2).Close;



		}
		protected override void CalcBar(){ 


			upES.Set(0);
			dnES.Set(0);
			upVX.Set(0);
			dnVX.Set(0);
			upDel.Set(0);
			dnDel.Set(0);
			totDel.Set(0);
		}
	}
}