using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	[SameAsSymbol(true), UpdateOnEveryTickAttribute(false)]
	public class _PrevDLow : IndicatorObject 
	{
		public _PrevDLow(object _ctx):base(_ctx){}
		
		private _MK_PrevDLow m_prevDLow;
		private ISeries<double> m_data;
		
		private IPlotObject plot1;
		
		protected override void Create() 
		{
			m_prevDLow = new _MK_PrevDLow(this);
			plot1 = AddPlot(new PlotAttributes("PDL", EPlotShapes.Cross, Color.Red));
		}
		
		protected override void StartCalc() 
		{
			m_data = new Lambda<double>(
                    delegate(int _idx)
                    {
                        return Bars.Low[_idx];
                    });
			m_prevDLow.Data = m_data;
		}
		
		protected override void CalcBar()
		{
			EResolution resolution = Bars.Info.Resolution.Type;

            if ( resolution < EResolution.Day || EResolution.Volume == resolution ||
                EResolution.Second == resolution || resolution > EResolution.Quarter )
            {
				double prevDLow = m_prevDLow[0];
				if (!PublicFunctions.DoubleEquals(prevDLow, 0))
				{
					plot1.Set(prevDLow);	
				}
			}
            else
			{
                PublicFunctions.RaiseRunTimeError("_PrevD_High requires intraday bars.");
            }
		}
	}
}