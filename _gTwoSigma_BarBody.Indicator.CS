using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTwoSigma_BarBody : IndicatorObject 
    {
        private _gBarOpen m_open;
        private _gBarClose m_close;
        private AverageFC m_avgBody;
        private _gTwoSigma m_bigBodyLevel;
        private _gTwoSigma m_smallBodyLevel;
        private VariableSeries<Double> m_body;

        private IPlotObject plot1;   
        private IPlotObject plot2;
        private IPlotObject plot3;

		public _gTwoSigma_BarBody(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            length = 50;
            bigDev = 2;
            smallDev = -2;
            
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //length = 50;
            //bigDev = 2;
            //smallDev = -2;

            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //length = 50;
            //bigDev = 2;
            //smallDev = -2;
        }

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int length { get; set; }
        [Input]
        public double bigDev { get; set; }
        [Input]
        public double smallDev { get; set; }

        protected override void Create() 
        {
            m_open = new _gBarOpen(this);
            m_close = new _gBarClose(this);
            m_avgBody = new AverageFC(this);
            m_bigBodyLevel = new _gTwoSigma(this);
            m_smallBodyLevel = new _gTwoSigma(this);
            m_body = new VariableSeries<double>(this);

			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Histogram, Color.Gray));
            plot2 = AddPlot(new PlotAttributes("highBand", EPlotShapes.Line, Color.Aqua));
            plot3 = AddPlot(new PlotAttributes("lowBand", EPlotShapes.Line, Color.Aqua));
		}
		protected override void StartCalc() 
        {
            m_open.leg1 = leg1;
            m_open.leg2 = leg2;
            m_open.leg3 = leg3;
            
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            
            m_avgBody.price = m_body;
            m_avgBody.length = length;

            m_bigBodyLevel.price = m_body;
            m_bigBodyLevel.length = length;
            m_bigBodyLevel.sigma = bigDev;

            m_smallBodyLevel.price = m_body;
            m_smallBodyLevel.length = length;
            m_smallBodyLevel.sigma = smallDev;
		}
		protected override void CalcBar()
        {
            m_body.Value = Math.Abs(m_open[0] - m_close[0]);
            var dev = m_body.StandardDeviationCustom(length, 1);
			plot1.Set(m_body[0] - m_avgBody[0]);
            plot2.Set(bigDev * dev);
            plot3.Set(smallDev * dev);
		}
	}
}