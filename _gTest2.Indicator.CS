using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator{
	public class _gTest2 : IndicatorObject 
    {
        private VariableSeries<int> m_pos;

        private System.Text.StringBuilder csv;  // for debug only

		public _gTest2(object _ctx):base(_ctx)
        {
            spread_leg1 = 0.5;
            spread_leg2 = -1;
            spread_leg3 = 0.333;
        }
		private IPlotObject plot1;

        [Input]
        public double spread_leg1 { get; set; }
        [Input]
        public double spread_leg2 { get; set; }
        [Input]
        public double spread_leg3 { get; set; }

		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
			plot1 = AddPlot(new PlotAttributes("pos", EPlotShapes.Histogram, Color.Aqua));
            csv = new StringBuilder();
		}
		protected override void StartCalc() {

		}
        protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            plot1.Set(m_pos.Value);
        }
	}
}