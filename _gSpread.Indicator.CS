using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSpread : IndicatorObject 
	{
        private _gSpreadFC m_spreadOpenFC;
        private _gSpreadFC m_spreadClosFC;

        private VariableSeries<Double> m_spreadOpen;
        private VariableSeries<Double> m_spreadClose;

        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
        private IPlotObject plot4;

		public _gSpread(object _ctx):base(_ctx)
		{
			leg1 = 1.0;
			leg2 = -0.333;
            leg3 = -0.5;
		}
		[Input]
		public double leg1 { get; set; }

		[Input]
		public double leg2 { get; set; }

        [Input]
        public double leg3 { get; set; }

        void assign(_gSpreadFC sprdFunc, ISeries<Double> inst1, 
            ISeries<Double> inst2, ISeries<Double> inst3)
        {
            sprdFunc.inst1 = inst1;
            sprdFunc.inst2 = inst2;
            sprdFunc.inst3 = inst3;
            sprdFunc.leg1 = this.leg1;
            sprdFunc.leg2 = this.leg2;
            sprdFunc.leg3 = this.leg3;
        }		
        
		protected override void Create() 
		{
            m_spreadOpenFC = new _gSpreadFC(this);
            m_spreadClosFC = new _gSpreadFC(this);

			m_spreadOpen = new VariableSeries<Double>(this);
            m_spreadClose = new VariableSeries<Double>(this);

            plot1 = AddPlot(new PlotAttributes("Open", EPlotShapes.LeftTick, Color.Red));
            plot2 = AddPlot(new PlotAttributes("High", EPlotShapes.BarHigh, Color.Red));
            plot3 = AddPlot(new PlotAttributes("Low", EPlotShapes.BarLow, Color.Red));
            plot4 = AddPlot(new PlotAttributes("Close", EPlotShapes.RightTick, Color.Red));
		}
		protected override void StartCalc() 
		{
            assign(m_spreadOpenFC, Bars.Open, BarsOfData(2).Open, (leg3 == 0) ? Bars.Open : BarsOfData(3).Open);
            assign(m_spreadClosFC, Bars.Close, BarsOfData(2).Close, (leg3 == 0) ? Bars.Close : BarsOfData(3).Close);
		}
		protected override void CalcBar()
		{
            //m_spreadOpen.Value = m_spreadOpenFC[0];
            //m_spreadClose.Value = m_spreadClosFC[0];
            var open = Math.Round(m_spreadOpenFC[0],2);
            var close = Math.Round(m_spreadClosFC[0],2);
            if (close >= open)
            {
                plot1.Set(open, Color.LawnGreen);
                plot2.Set(close, Color.LawnGreen);
                plot3.Set(open, Color.LawnGreen);
                plot4.Set(close, Color.LawnGreen);
            }
            else
            {
                plot1.Set(open, Color.Red);
                plot2.Set(open, Color.Red);
                plot3.Set(close, Color.Red);
                plot4.Set(close, Color.Red);
            }
		}
	}
}