using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSingleReturnHedge : IndicatorObject
    {
		public _gSingleReturnHedge(object _ctx):base(_ctx)
        {
            sizeFactor = 100; 
        }

        private VariableSeries<Double> m_bbDollarChange;  // buy buy dollar change
		private IPlotObject plot1;

        private ISeries<double> fvES { get; set; }
        private ISeries<double> fvVX { get; set; }

        private _gHedgeRatio m_hedge_indic;

        [Input]
        public int sizeFactor { get; set; }

		protected override void Create() 
        {
            m_bbDollarChange = new VariableSeries<Double>(this);
            plot1 = AddPlot(new PlotAttributes("BuyBuyHedge", EPlotShapes.Histogram, 
                Color.Cyan, Color.Empty, 0, 0, true));

            m_hedge_indic = (_gHedgeRatio)AddIndicator("_gHedgeRatio");
            m_hedge_indic.sizeFactor = 10;
		}

		protected override void StartCalc() 
        {
            fvES = BarsOfData(3).Close;
            fvVX = BarsOfData(4).Close;
		}

		protected override void CalcBar()
        {
            //var ratio = sizeFactor / 100; // 100 is the default sizeFactor for m_hedge_indic

            var dollarChangeES = m_hedge_indic.Plots[0].Values[1] * 50 * (fvES[0] - fvES[1]);
            var dollarChangeVX = m_hedge_indic.Plots[1].Values[1] * 1000 * (fvVX[0] - fvVX[1]);
            m_bbDollarChange.Value = dollarChangeES + dollarChangeVX;
            plot1.Set(0, m_bbDollarChange.Value);
		}
	}
}