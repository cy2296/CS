using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _GaryAvgTwo : IndicatorObject 
    {
        private VariableSeries<Double> m_avgBar12;

        public _GaryAvgTwo(object _ctx) : base(_ctx) 
        {
            ratioBar1 = 0.5;
        }
        private IPlotObject plot3;
		
		protected override void Create() 
        {
			m_avgBar12 = new VariableSeries<Double>(this);
            plot3 = AddPlot(new PlotAttributes("Avg", 0, Color.Yellow, Color.Empty, 0, 0, true));
		}

        private ISeries<double> price1 { get; set; }
        private ISeries<double> price2 { get; set; }
		
        [Input]
        public double ratioBar1 { get; set; }

		protected override void StartCalc() 
        {
            price1 = Bars.Close;
            price2 = BarsOfData(2).Close;
		}
		protected override void CalcBar()
        {
			m_avgBar12.Value = ratioBar1 *  Bars.Close[0] + (1 - ratioBar1) * BarsOfData(2).Close[0];
            plot3.Set(0, m_avgBar12.Value);
		}
	}
}