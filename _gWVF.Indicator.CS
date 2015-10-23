using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gWVF : IndicatorObject 
    {
        private HighestFC m_highest;
        private VariableSeries<double> m_wvf;
        private XAverage m_wvfAvg;

		public _gWVF(object _ctx):base(_ctx)
        {
            length = 22;
        }
		
        private IPlotObject plot1;
        private IPlotObject plot2;

        [Input]
        public int length { get; set; }

        protected override void Create() 
        {
            m_highest = new HighestFC(this);
            m_wvf = new VariableSeries<double>(this);
            m_wvfAvg = new XAverage(this);
			plot1 = AddPlot(new PlotAttributes("WVF", EPlotShapes.Line, Color.Aqua));
            plot2 = AddPlot(new PlotAttributes("WVF_Avg", EPlotShapes.Line, Color.Yellow));
		}
		protected override void StartCalc() 
        {
            m_highest.pricevalue = Bars.Close;
            m_highest.len = length;
            m_wvfAvg.Price = m_wvf;
            m_wvfAvg.Length = 5;
		}
		protected override void CalcBar()
        {
            m_wvf.Value = (m_highest[0] - Bars.Low[0]) / m_highest[0] * 100;
			plot1.Set(m_wvf[0]);
            plot2.Set(m_wvfAvg[0]);
		}
	}
}