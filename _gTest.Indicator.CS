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
        private VariableSeries<Double> m_stairs;
		public _gTest(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
        }

        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
        private IPlotObject plot4;
       
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }

        protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_stairway = new _gTestFC1(this);
            m_spreadOpen = new VariableSeries<double>(this);
            m_spreadClos = new VariableSeries<double>(this);
            m_stairs = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.Line, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.RightTick, Color.Yellow));
            plot3 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarHigh, Color.Yellow));
            plot4 = AddPlot(new PlotAttributes("Stairs", EPlotShapes.BarLow, Color.Yellow));

		}
		protected override void StartCalc() 
        {
            m_stairway.o = (leg2 == 0 && leg3 == 0) ? Bars.Open : m_spreadOpen;
            m_stairway.h = (leg2 == 0 && leg3 == 0) ? Bars.High : m_spreadOpen;
            m_stairway.l = (leg2 == 0 && leg3 == 0) ? Bars.Low : m_spreadClos;
            m_stairway.c = (leg2 == 0 && leg3 == 0) ? Bars.Close : m_spreadClos;
            m_stairway.calltype = "indic";
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            //m_stairs.Value = m_stairway[0];     /// never make the mistake again !! 
            
            m_spreadOpen.Value = Bars.Open[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
            m_spreadClos.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);
            
            
            m_stairs.Value = m_stairway[0];
            if (m_pos[0] == 0 && m_pos[1] == 0)
            {
                plot1.Set(m_stairs[0], Color.Black);
                plot2.Set(m_stairs[0], Color.Black);
                //plot3.Set(m_stairway[0], Color.Black);
                //plot4.Set(m_stairway[0], Color.Black);
            }
            else
            {
                plot1.Set(m_stairs[0], Color.Aqua);
                plot2.Set(m_stairs[0], Color.Aqua);
                //plot3.Set(m_stairway[0], Color.Aqua);
                //plot4.Set(m_stairway[0], Color.Aqua);
            }
		}
	}
}