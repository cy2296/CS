using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gSpread_Stairs : IndicatorObject 
    {
        private _gSpreadFC m_spreadOpenFC;
        private _gSpreadFC m_spreadClosFC;
        private VariableSeries<Double> m_averagePrice;

        private VariableSeries<int> m_pos;
        private _gTestFC1 m_stairway;

        private LinearRegValue m_linearregvalue1;
        private VariableSeries<Double> m_linreg;
        private XAverage m_linregEma;    

        // plot candlesticks
        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
        private IPlotObject plot4;
        // plot stairs
        private IPlotObject plot5;
        private IPlotObject plot6;
        // regression line
        private IPlotObject plot7;
        private IPlotObject plot8;

		public _gSpread_Stairs(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            reg_length = 20;
            displace = 0;
            emalength = 4;
        }
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int reg_length { get; set; }
        [Input]
        public int displace { get; set; }
        [Input]
        public int emalength { get; set; }


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
            m_averagePrice = new VariableSeries<Double>(this);

            m_pos = new VariableSeries<int>(this);
            m_stairway = new _gTestFC1(this);

            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<Double>(this);
            m_linregEma = new XAverage(this);

            plot1 = AddPlot(new PlotAttributes("Open", EPlotShapes.LeftTick, Color.Red));
            plot2 = AddPlot(new PlotAttributes("High", EPlotShapes.BarHigh, Color.Red));
            plot3 = AddPlot(new PlotAttributes("Low", EPlotShapes.BarLow, Color.Red));
            plot4 = AddPlot(new PlotAttributes("Close", EPlotShapes.RightTick, Color.Red));

            plot5 = AddPlot(new PlotAttributes("Low", EPlotShapes.LeftTick, Color.Red));
            plot6 = AddPlot(new PlotAttributes("Close", EPlotShapes.Line, Color.Red));

            plot7 = AddPlot(new PlotAttributes("LinReg", EPlotShapes.Line, Color.Yellow));
            plot8 = AddPlot(new PlotAttributes("LinReg", EPlotShapes.Line, Color.Purple));
		}
		protected override void StartCalc() 
        {
            assign(m_spreadOpenFC, Bars.Open, 
                (leg2 == 0) ? Bars.Open : BarsOfData(2).Open, 
                (leg3 == 0) ? Bars.Open : BarsOfData(3).Open);
            assign(m_spreadClosFC, Bars.Close, 
                (leg2 == 0) ? Bars.Close : BarsOfData(2).Close, 
                (leg3 == 0) ? Bars.Close : BarsOfData(3).Close);

            m_stairway.o = (leg2 == 0 && leg3 == 0) ? Bars.Open : m_spreadOpenFC;
            m_stairway.h = (leg2 == 0 && leg3 == 0) ? Bars.High : m_spreadOpenFC;
            m_stairway.l = (leg2 == 0 && leg3 == 0) ? Bars.Low : m_spreadClosFC;
            m_stairway.c = (leg2 == 0 && leg3 == 0) ? Bars.Close : m_spreadClosFC;
            m_stairway.calltype = "indic";

            m_linearregvalue1.price = (leg2 == 0 && leg3 == 0) ? Bars.Close : m_averagePrice; 
            m_linearregvalue1.length = reg_length;
            m_linearregvalue1.tgtbar = 0;

            m_linregEma.Price = m_linreg;
            m_linregEma.Length = emalength;
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            if (leg2 == 0 && leg3 == 0)
                m_averagePrice.Value = (Bars.High[0] + Bars.Low[0] + Bars.Close[0]) / 3;
            else
                m_averagePrice.Value = 0.5 * (m_spreadOpenFC[0] + m_spreadClosFC[0]);

            var open = Math.Round(m_spreadOpenFC[0], 4);
            var close = Math.Round(m_spreadClosFC[0], 4);

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
            // plot stairs
            //if (m_pos[0] == 0 && m_pos[1] == 0)
            //{
            //    plot5.Set(m_stairway[0], Color.Black);
            //    plot6.Set(m_stairway[0], Color.Black);
            //}
            //else
            //{
            //    plot5.Set(m_stairway[0], Color.Aqua);
            //    plot6.Set(m_stairway[0], Color.Aqua);
            //}

            m_linreg.Value = m_linearregvalue1[0];
            plot7.Set(displace, m_linreg.Value);
            plot8.Set(m_linregEma[0]);
		}
	}
}