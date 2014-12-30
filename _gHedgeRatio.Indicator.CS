using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gHedgeRatio : IndicatorObject {
		public _gHedgeRatio(object _ctx):base(_ctx)
		{
		    delta_length = 100;
            delta_vxWeight = 0.6;
            delta_upRatio = 0.5;
            sizeFactor = 100; 
		}
		
		private VariableSeries<Double> m_esSize;
		private VariableSeries<Double> m_vxSize;
		
		private IPlotObject plot1;
		private IPlotObject plot2;
			
		private _gTurnMetric_Delta m_delta_indic;
		[Input]
        public int delta_length { get; set; }
        [Input]
        public double delta_vxWeight { get; set; }
        [Input]
        public double delta_upRatio { get; set; }
        [Input]
        public int sizeFactor { get; set; }
		
		protected override void Create() 
		{
            m_esSize = new VariableSeries<double>(this);
            m_vxSize = new VariableSeries<double>(this);

			plot1 = AddPlot(new PlotAttributes("esSize", EPlotShapes.Line, Color.Red));
			plot2 = AddPlot(new PlotAttributes("vxSize", EPlotShapes.Line, Color.Yellow));
			
			m_delta_indic = (_gTurnMetric_Delta)AddIndicator("_gTurnMetric_Delta");
			m_delta_indic.length = delta_length;
            m_delta_indic.vxWeight = delta_vxWeight;
            m_delta_indic.upRatio = delta_upRatio;
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar(){
			if (1 == Bars.CurrentBar)
			{
				m_esSize.Value = sizeFactor;
				m_vxSize.Value = sizeFactor;
			}
			var delta = m_delta_indic.Plots[6].Values[1];
			m_esSize.Value = 2 * sizeFactor * delta / (1 + delta);
			m_vxSize.Value = 2 * sizeFactor - m_esSize.Value;

			plot1.Set(0,m_esSize.Value);
			plot2.Set(0,m_vxSize.Value);
		}
	}
}