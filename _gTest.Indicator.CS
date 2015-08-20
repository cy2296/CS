using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTest : IndicatorObject 
    {
        private VariableSeries<int> m_pos;
        private _gTestFC1 m_stairway;
        private VariableSeries<Double> m_spreadOpen;
        private VariableSeries<Double> m_spreadClos;
		public _gTest(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            showValue = -21.5;
        }

        private IPlotObject plot1;
        private IPlotObject plot2;

       
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public double showValue { get; set; }
  	

        protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_stairway = new _gTestFC1(this);
            m_spreadOpen = new VariableSeries<double>(this);
            m_spreadClos = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("FastAvg", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            plot2 = AddPlot(new PlotAttributes("FastAvg", 0, Color.Aqua,
                               Color.Empty, 0, 0, true));
		}
		protected override void StartCalc() 
        {
            m_stairway.o = m_spreadOpen;
            m_stairway.h = m_spreadOpen;
            m_stairway.l = m_spreadClos;
            m_stairway.c = m_spreadClos;
            m_stairway.calltype = "indic";
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            m_spreadOpen.Value = Bars.Open[0] * leg1
            + ((leg2 != 0) ? BarsOfData(2).Open[0] : Bars.Open[0]) * leg2
            + ((leg3 != 0) ? BarsOfData(3).Open[0] : Bars.Open[0]) * leg3;
            m_spreadClos.Value = Bars.Close[0] * leg1
            + ((leg2 != 0) ? BarsOfData(2).Close[0] : Bars.Close[0]) * leg2
            + ((leg3 != 0) ? BarsOfData(3).Close[0] : Bars.Close[0]) * leg3;

            if (m_pos[0] == 0 && m_pos[1] == 0)
                plot1.Set(m_stairway[0], Color.Black);
            else
                plot1.Set(m_stairway[0], Color.Aqua);
		}
	}
}