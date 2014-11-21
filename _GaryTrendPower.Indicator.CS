using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _GaryTrendPower : IndicatorObject {
		public _GaryTrendPower(object _ctx):base(_ctx)
		{
			fastLength = 7;
			slowLength = 14;
			strongLevel = 0.9;
		}
		[Input]
		public int fastLength { get; set; }
		[Input]
		public int slowLength { get; set; }

		[Input]
		public double strongLevel { get; set; }
		[Input]
		public Color strongColor { get; set; }
		[Input]
		public Color weakColor { get; set; }
				private IPlotObject power_line;

		// compute the "avg median price" for the period
		private double AvgVal(int length)
		{
			double aval = 0.0;
			for (int i = 0; i < length; ++i)
				aval += Bars.AvgPrice(i);
			return aval / length;
		}

		protected override void Create() {
			// create variable objects, function objects, plot objects etc.
			power_line = AddPlot(new PlotAttributes("Power", EPlotShapes.Histogram, Color.Cyan));
		}

		protected override void StartCalc() {
			// assign inputs 
		}

		protected override void CalcBar(){
			double afast = AvgVal(fastLength);
			double aslow = AvgVal(slowLength);
			double power = Math.Abs(100 * (afast - aslow) / Bars.Close[0]);
			Color pl_col = weakColor;
			if (power >= strongLevel)
				pl_col = strongColor;
			power_line.Set(power, pl_col);
		}
	}
}