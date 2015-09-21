using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	[SameAsSymbol(true), UpdateOnEveryTickAttribute(false)]
	public class _PrevDHigh : IndicatorObject 
	{
		public _PrevDHigh(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5; 
        }

        private _gBarHigh m_high;
		private _MK_PrevDHigh m_prevDHigh;
		//private ISeries<double> m_data;
		
		private IPlotObject plot1;

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }

        protected override void Create() 
		{
            m_high = new _gBarHigh(this);
			m_prevDHigh = new _MK_PrevDHigh(this);
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Cross, Color.Green));
		}
		
		protected override void StartCalc() 
		{
            //m_data = new Lambda<double>(
            //        delegate(int _idx)
            //        {
            //            return Bars.High[_idx];
            //        });
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
			//m_prevDHigh.Data = m_data;
            m_prevDHigh.Data = m_high;
		}
		
		protected override void CalcBar()
		{
			EResolution resolution = Bars.Info.Resolution.Type;

            if (resolution < EResolution.Day || EResolution.Volume == resolution ||
                EResolution.Second == resolution || resolution > EResolution.Quarter)
            {
				double prevDHigh = m_prevDHigh[0];
				if (!PublicFunctions.DoubleEquals(prevDHigh, 0))
				{
					plot1.Set(prevDHigh);	
				}
			}
            else
			{
                PublicFunctions.RaiseRunTimeError("_PrevD_High requires intraday bars.");
            }
		}
	}
}