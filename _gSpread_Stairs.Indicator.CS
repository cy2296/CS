using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gSpread_Stairs : IndicatorObject 
    {
        private _gUtility_MarketsOpened m_marketopen;
       // private _gUtility_TimeToTrade m_timetotrade;
        private _gBarOpen m_open;
        private _gBarClose m_close;
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _gBarTipical m_average;
        

        private VariableSeries<int> m_pos;
       // private _gTestFC1 m_stairway;

        private LinearRegValue m_linearregvalue1;
        private VariableSeries<Double> m_linreg;
        private XAverage m_linregEma;

        private _MK_PrevDHigh m_prevDHigh;
        private _MK_PrevDLow m_prevDLow;
        


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

        private IPlotObject plot9;
        private IPlotObject plot10;
        private IPlotObject plot11;

        private IPlotObject plot12;
        private IPlotObject plot13;

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

		protected override void Create() 
        {
            m_marketopen = new _gUtility_MarketsOpened(this);
            m_open = new _gBarOpen(this);
            m_close = new _gBarClose(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_average = new _gBarTipical(this);
            m_prevDHigh = new _MK_PrevDHigh(this);
            m_prevDLow = new _MK_PrevDLow(this);
            m_pos = new VariableSeries<int>(this);
            //m_stairway = new _gTestFC1(this);

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
            plot8 = AddPlot(new PlotAttributes("LinRegEMA", EPlotShapes.Line, Color.Aqua));

            plot9 = AddPlot(new PlotAttributes("pDayHigh", EPlotShapes.Line, Color.Yellow));
            plot10 = AddPlot(new PlotAttributes("pDayLow", EPlotShapes.Line, Color.Yellow));
            plot11 = AddPlot(new PlotAttributes("pDayMid", EPlotShapes.Line, Color.Yellow));

            plot12 = AddPlot(new PlotAttributes("PH", EPlotShapes.Line, Color.White));
            plot13 = AddPlot(new PlotAttributes("PL", EPlotShapes.Line, Color.White));
		}
        protected override void StartCalc()
        {
            m_marketopen.leg1 = leg1;
            m_marketopen.leg2 = leg2;
            m_marketopen.leg3 = leg3;
            m_open.leg1 = leg1;
            m_open.leg2 = leg2;
            m_open.leg3 = leg3;
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
            m_low.leg1 = leg1;
            m_low.leg2 = leg2;
            m_low.leg3 = leg3;
            m_average.leg1 = leg1;
            m_average.leg2 = leg2;
            m_average.leg3 = leg3;

            //m_stairway.o = (leg2 == 0 && leg3 == 0) ? Bars.Open : m_spreadOpenFC;
            //m_stairway.h = (leg2 == 0 && leg3 == 0) ? Bars.High : m_spreadOpenFC;
            //m_stairway.l = (leg2 == 0 && leg3 == 0) ? Bars.Low : m_spreadClosFC;
            //m_stairway.c = (leg2 == 0 && leg3 == 0) ? Bars.Close : m_spreadClosFC;
            //m_stairway.calltype = "indic";

            m_linearregvalue1.price = m_average;
            m_linearregvalue1.length = reg_length;
            m_linearregvalue1.tgtbar = 0;

            m_linregEma.Price = m_linreg;
            m_linregEma.Length = emalength;

            m_prevDHigh.Data = m_high;
            m_prevDLow.Data = m_low;
        }
		protected override void CalcBar()
        {
            if (m_marketopen[0])
            {
                m_pos.Value = StrategyInfo.MarketPosition;

                var open = Math.Round(m_open[0], 4);
                var close = Math.Round(m_close[0], 4);

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
                //plot7.Set(displace, m_linreg.Value);
                //plot8.Set(m_linregEma[0]);

                //plot9.Set(m_dayhigh[1]);

                EResolution resolution = Bars.Info.Resolution.Type;

                if (resolution < EResolution.Day || EResolution.Volume == resolution ||
                    EResolution.Second == resolution || resolution > EResolution.Quarter)
                {
                    double prevDHigh = m_prevDHigh[0];
                    if (!PublicFunctions.DoubleEquals(prevDHigh, 0))
                    {
                        //plot9.Set(prevDHigh);
                    }
                    double prevDLow = m_prevDLow[0];
                    if (!PublicFunctions.DoubleEquals(prevDLow, 0))
                    {
                        //plot10.Set(prevDLow);
                    }
                    double prevDMid = 0.5 * (prevDHigh + prevDLow);
                    double prevDPH = prevDLow + 1.382 * (prevDHigh - prevDLow);
                    double prevDPL = prevDHigh - 1.382 * (prevDHigh - prevDLow);
                    //plot11.Set(prevDMid);


                    //plot12.Set(prevDPH);
                    //plot13.Set(prevDPL);
                    
                }
                else
                {
                    PublicFunctions.RaiseRunTimeError("_PrevD_High requires intraday bars.");
                }
            }
		}




	}


}