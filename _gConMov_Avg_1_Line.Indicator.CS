using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
///////////////////////////////////////////////////////////////////////////////////////////
/// The volume indicates whether the price should be computed for simple moving average ///
/// Volume: 1 -> price should be counted, 0 -> price shouldn't be counted               ///
///////////////////////////////////////////////////////////////////////////////////////////
namespace PowerLanguage.Indicator{
	public class _gConMov_Avg_1_Line : IndicatorObject 
	{
		private _gConAvgFC m_conAvgFc1;
		private VariableSeries<Double> m_conAvg;
		private IPlotObject plot1;

		private ISeries<double> price { get; set; }

		public _gConMov_Avg_1_Line(object _ctx):base(_ctx)
		{
			length = 5;
		}

		[Input]
		public int length { get; set; }

		protected override void Create() 
		{
			m_conAvgFc1 = new _gConAvgFC(this);
			m_conAvg = new VariableSeries<Double>(this);
			plot1 = AddPlot(new PlotAttributes("conAvg", EPlotShapes.Line, 
				Color.Red, Color.Empty, 0, 0, true));
		}
		protected override void StartCalc() 
		{
			price = Bars.Close;
			m_conAvgFc1.price = price;
			m_conAvgFc1.length = length;
		}
		protected override void CalcBar()
		{
			m_conAvg.Value = m_conAvgFc1[0];
			plot1.Set(0, m_conAvg.Value);
		}
	}
}