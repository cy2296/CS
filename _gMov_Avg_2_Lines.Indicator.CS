using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gMov_Avg_2_Lines : IndicatorObject 
    {
        private XAverage m_averagefc1;
        private XAverage m_averagefc2;
        private IPlotObject Plot1;
        private IPlotObject Plot2;
        private VariableSeries<Double> m_spread;
		
        public _gMov_Avg_2_Lines(object _ctx):base(_ctx)
        {
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;
            slowlength = 18;
            fastlength = 9;        
        }

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int fastlength { get; set; }
        [Input]
        public int slowlength { get; set; }

		protected override void Create() 
        {
            m_averagefc1 = new XAverage(this);
            m_averagefc2 = new XAverage(this);
            Plot1 =
                AddPlot(new PlotAttributes("FastAvg", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("SlowAvg", 0, Color.Magenta,
                                           Color.Empty, 0, 0, true));
            m_spread = new VariableSeries<double>(this);
		}
		protected override void StartCalc() 
        {
            m_averagefc1.Price = m_spread;
            m_averagefc1.Length = fastlength;
            m_averagefc2.Price = m_spread;
            m_averagefc2.Length = slowlength;
		}
		protected override void CalcBar()
        {
            m_spread.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] : Bars.Close[0]) * leg2
                + ((leg3 != 0) ? BarsOfData(3).Close[0] : Bars.Close[0]) * leg3;
            Plot1.Set(m_averagefc1[0]);
            Plot2.Set(m_averagefc2[0]);
		}
	}
}