using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTest2 : IndicatorObject 
    {
        private VariableSeries<int> m_pos;

		public _gTest2(object _ctx):base(_ctx){}
		private IPlotObject plot1;
		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
			plot1 = AddPlot(new PlotAttributes("Pos", EPlotShapes.Histogram, Color.Aqua));
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            plot1.Set(m_pos.Value);
		}
	}
}