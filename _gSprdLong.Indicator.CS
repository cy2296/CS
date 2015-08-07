using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSprdLong : IndicatorObject 
    {
        private VariableSeries<Double> m_spreadLong;
        private VariableSeries<Double> m_spreadShort;
		public _gSprdLong(object _ctx):base(_ctx)
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

		private IPlotObject plot1;
        private IPlotObject plot2;

        protected override void Create() {
            m_spreadLong = new VariableSeries<Double>(this);
            m_spreadShort = new VariableSeries<Double>(this);
			plot1 = AddPlot(new PlotAttributes("forlong", EPlotShapes.Line, Color.Aqua));
            plot2 = AddPlot(new PlotAttributes("forshort", EPlotShapes.Line, Color.Beige));
		}
		protected override void StartCalc() 
        {

		}
		protected override void CalcBar()
        {
            double leg1forlong = (leg1 >= 0) ? Bars.StatusLine.Ask : Bars.StatusLine.Bid;
            double leg1forshort = (leg1 >= 0) ? Bars.StatusLine.Bid : Bars.StatusLine.Ask;

            double leg2forlong = (leg2 >= 0) ? BarsOfData(2).StatusLine.Ask : BarsOfData(2).StatusLine.Bid;
            double leg2forshort = (leg2 >= 0) ? BarsOfData(2).StatusLine.Bid : BarsOfData(2).StatusLine.Ask;

            double leg3forlong;
            double leg3forshort;
            if (leg3 == 0)
            {
                leg3forlong = 0;  // no Data3
                leg3forshort = 0;
            }
            else if (leg3 > 0)
            {
                leg3forlong = BarsOfData(3).StatusLine.Ask;
                leg3forshort = BarsOfData(3).StatusLine.Bid;
            }
            else
            {
                leg3forlong = BarsOfData(3).StatusLine.Bid;
                leg3forshort = BarsOfData(3).StatusLine.Ask;
            }
            m_spreadLong.Value = leg1 * leg1forlong + leg2 * leg2forlong + leg3 * leg3forlong;
            m_spreadShort.Value = leg1 * leg1forshort + leg2 * leg2forshort + leg3 * leg3forshort;

            plot1.Set(m_spreadLong.Value);
            plot2.Set(m_spreadShort.Value);
		}
	}
}