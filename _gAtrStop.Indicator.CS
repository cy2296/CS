using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gAtrStop : IndicatorObject 
    {
        private VariableSeries<Double> m_atr;
        private VariableSeries<Double> m_atrUp;
        private VariableSeries<Double> m_atrDn;

        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;

        private ISeries<double> price { get; set; }

        public _gAtrStop(object _ctx):base(_ctx)
        {
            atrlength = 14;
        }

        [Input]
        public int atrlength { get; set; }
        [Input]
        public int atrUp { get; set; }
        [Input]
        public int atrDn { get; set; }

		protected override void Create() 
        {
            m_atr = new VariableSeries<Double>(this);
            m_atrUp = new VariableSeries<double>(this);
            m_atrDn = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            plot2 = AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            plot3 =
                AddPlot(new PlotAttributes("Price", 0, Color.Gray,
                                           Color.Empty, 0, 0, true));
        }
		protected override void StartCalc() 
        {
            price = Bars.Close;
		}
		protected override void CalcBar()
        {
            m_atr.Value = this.AverageTrueRange(atrlength);

            m_atrUp.Value = price[1] + 2 * m_atr[1];
            m_atrDn.Value = price[1] - 2 * m_atr[1];
            plot1.Set(0, m_atrUp.Value);
            plot2.Set(0, m_atrDn.Value);
            plot3.Set(0, price.Value);
		}
	}
}