using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _GaryTest : IndicatorObject 
    {
        public _GaryTest(object _ctx) : base(_ctx) { }

		private IPlotObject plot1;
		//
		private IPlotObject plot2;
		private IPlotObject plot3;

		private ADX m_adx_indic;

		protected override void Create() 
        {
			plot1 = AddPlot();
			plot2 = AddPlot();
			plot3 = AddPlot();

			m_adx_indic = (ADX)AddIndicator("ADX");
			m_adx_indic.length = 20;
		}
		protected override void StartCalc() 
        {
		}
		protected override void CalcBar()
        {
			plot1.Set(m_adx_indic.Plots[0].Values[0]);
		}
	}
}