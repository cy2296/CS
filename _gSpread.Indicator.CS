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


        //private ISeries<double> inst1 { get; set; }
        //private ISeries<double> inst2 { get; set; }
        //private ISeries<double> inst3 { get; set; }
		
        
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
            m_spreadOpenFC.inst1 = Bars.Open;
            m_spreadOpenFC.inst2 = BarsOfData(2).Open;
            m_spreadOpenFC.inst3 = (leg3 == 0) ? Bars.Open : BarsOfData(3).Open;
            m_spreadOpenFC.leg1 = leg1;
            m_spreadOpenFC.leg2 = leg2;
            m_spreadOpenFC.leg3 = leg3;

            m_spreadClosFC.inst1 = Bars.Close;
            m_spreadClosFC.inst2 = BarsOfData(2).Close;
            m_spreadClosFC.inst3 = (leg3 == 0) ? Bars.Close : BarsOfData(3).Close;
            m_spreadClosFC.leg1 = leg1;
            m_spreadClosFC.leg2 = leg2;
            m_spreadClosFC.leg3 = leg3;
		}
		protected override void CalcBar()
		{
            //m_spreadOpen.Value = m_spreadOpenFC[0];
            //m_spreadClose.Value = m_spreadClosFC[0];
            var open = m_spreadOpenFC[0];
            var close = m_spreadClosFC[0];
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