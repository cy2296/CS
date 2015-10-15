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

        private bool m_prevBarEndedSession = false;
        private int m_todaybarcount = 0;
        private double m_currSessionSum;
        private double m_currDeviationSum;

        private XAverage m_rsiXavg;

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

        private IPlotObject plot5;
        private IPlotObject plot6;
        private IPlotObject plot7;

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
            m_rsiXavg = new XAverage(this);
            m_spread = new VariableSeries<double>(this);

            plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Green));
            plot3 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Green));
            plot4 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Gray));
            plot5 = AddPlot(new PlotAttributes("meanRSI", EPlotShapes.Line, Color.Red));
            plot6 = AddPlot(new PlotAttributes("upperRSI", EPlotShapes.Line, Color.Red));
            plot7 = AddPlot(new PlotAttributes("lowerRSI", EPlotShapes.Line, Color.Red));
        }
		protected override void StartCalc() 
        {
            midline = 0.5 * (oversold + overbought);
            m_rsi.price = m_spread;
            m_rsi.length = rsi_len;
            m_rsiXavg.Price = m_rsi;
            m_rsiXavg.Length = 240;
        }
		protected override void CalcBar()
        {
            m_spread.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] : Bars.Close[0]) * leg2
                + ((leg3 != 0) ? BarsOfData(3).Close[0] : Bars.Close[0]) * leg3;

            double diff1 = m_rsiXavg[0] - 50;

            plot1.Set(m_rsi[0]);
            plot2.Set(overbought + diff1);
            plot3.Set(oversold + diff1);
            plot4.Set(midline);

            if (m_prevBarEndedSession)
            {
                m_todaybarcount = 1;
                m_prevBarEndedSession = false;
                m_currSessionSum = m_rsi[0];
                m_currDeviationSum = 0;
            }
            else
            {
                ++m_todaybarcount;
                m_currSessionSum += m_rsi[0];
            }
            if (Bars.LastBarInSession)
            {
                m_prevBarEndedSession = true;
                m_currSessionSum = 0;
                m_currDeviationSum = 0;
            }
            double mean = m_currSessionSum / m_todaybarcount;
            m_currDeviationSum += Math.Pow((m_rsi[0] - mean), 2);
            double stdev = Math.Sqrt(m_currDeviationSum / m_todaybarcount);
            if (!Bars.LastBarInSession)
            {
                //plot5.Set(mean, Color.SkyBlue);
                //plot6.Set(mean + 1.645 * stdev, Color.White);
                //plot7.Set(mean - 1.645 * stdev, Color.White);
            }
		}   
	}
}