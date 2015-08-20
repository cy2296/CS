using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gRSI : IndicatorObject 
    {
        private Function.RSI m_rsi;
        private VariableSeries<Double> m_spread;
        private double midline;

		public _gRSI(object _ctx):base(_ctx)
        {
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;
            rsi_len = 14;
            overbought = 70;
            oversold = 30;
        }
        private IPlotObject plot1;   // plot rsi
		private IPlotObject plot2;
        private IPlotObject plot3;
        private IPlotObject plot4;
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int rsi_len { get; set; }
        [Input]
        public double oversold { get; set; }
        [Input]
        public double overbought { get; set; }

		protected override void Create() {
            m_rsi = new Function.RSI(this);
            m_spread = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Green));
            plot3 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Green));
            plot4 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Gray));
		}
		protected override void StartCalc() 
        {
            midline = 0.5 * (oversold + overbought);
            m_rsi.price = m_spread;
            m_rsi.length = rsi_len;
        }
		protected override void CalcBar()
        {
            m_spread.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] : Bars.Close[0]) * leg2
                + ((leg3 != 0) ? BarsOfData(3).Close[0] : Bars.Close[0]) * leg3;
            plot1.Set(m_rsi[0]);
            plot2.Set(overbought);
            plot3.Set(oversold);
            plot4.Set(midline);
		}   
	}
}