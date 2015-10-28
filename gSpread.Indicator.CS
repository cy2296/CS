using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class gSpread : IndicatorObject 
    {
        private _gUtility_MarketsOpened m_marketopen;
        private _gBarOpen m_open;
        private _gBarClose m_close;
        private _gBarHigh m_high;
        private _gBarLow m_low;

		public gSpread(object _ctx):base(_ctx){}

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
            m_marketopen = new _gUtility_MarketsOpened(this);
            m_open = new _gBarOpen(this);
            m_close = new _gBarClose(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);

            plot1 = AddPlot(new PlotAttributes("Open", EPlotShapes.LeftTick, Color.Red));
            plot2 = AddPlot(new PlotAttributes("High", EPlotShapes.BarHigh, Color.Red));
            plot3 = AddPlot(new PlotAttributes("Low", EPlotShapes.BarLow, Color.Red));
            plot4 = AddPlot(new PlotAttributes("Close", EPlotShapes.RightTick, Color.Red));
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
		}
		protected override void CalcBar()
        {
            if (m_marketopen[0])
            { 
                
            }
			plot1.Set(Bars.Close[0]);
		}

        //private bool AllMarketOpened()
        //{
        //    if (leg1 != 0 && leg2 != 0 && leg3 != 0 && leg4 != 0)
        //    { 
                            
        //    }
        //    else if (leg1 == 0 && leg2 != 0 && leg3 != 0 && leg4 != 0) 

        
        //}
	}
}