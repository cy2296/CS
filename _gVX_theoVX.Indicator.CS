using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gVX_theoVX : IndicatorObject {
		public _gVX_theoVX(object _ctx):base(_ctx){}

        private VariableSeries<Double> m_spread;
        private IPlotObject plot1;
        private _gVX_fv m_theoVX_indic;
        private ISeries<double> vxFv { get; set; }
		
		protected override void Create() 
        {
            m_spread = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("theoVX_vx_spread", EPlotShapes.Histogram, Color.Red));

            m_theoVX_indic = (_gVX_fv)AddIndicator("_gVX_fv");
            m_theoVX_indic.spreadLookBack = 10;
        }
		protected override void StartCalc() 
        {
            vxFv = BarsOfData(4).Close;  
		}
		protected override void CalcBar()
        {
            m_spread.Value = vxFv[0] - m_theoVX_indic.Plots[0].Values[0];
            plot1.Set(0, m_spread.Value);
		}
	}
}