using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSingleReturnHedge : IndicatorObject
    {
		public _gSingleReturnHedge(object _ctx):base(_ctx)
        {
            delta_length = 100;
            delta_vxWeight = 0.6;
            delta_upRatio = 0.5;
            sizeFactor = 100; 
        }

        private VariableSeries<Double> m_bbDollarChange;  // buy buy dollar change
		private IPlotObject plot1;

        private ISeries<double> fvES { get; set; }
        private ISeries<double> fvVX { get; set; }

        private _gHedgeRatio m_hedge_indic;
        [Input]
        public int delta_length { get; set; }
        [Input]
        public double delta_vxWeight { get; set; }
        [Input]
        public double delta_upRatio { get; set; }
        [Input]
        public int sizeFactor { get; set; }

		protected override void Create() 
        {
            m_bbDollarChange = new VariableSeries<Double>(this);
            plot1 = AddPlot(new PlotAttributes("BuyBuyHedge", EPlotShapes.Histogram, 
                Color.Cyan, Color.Empty, 0, 0, true));

            m_hedge_indic = (_gHedgeRatio)AddIndicator("_gHedgeRatio");
            m_hedge_indic.delta_length = delta_length;
            m_hedge_indic.delta_vxWeight = delta_vxWeight;
            m_hedge_indic.delta_upRatio = delta_upRatio;
            m_hedge_indic.sizeFactor = sizeFactor;
		}

		protected override void StartCalc() 
        {
            fvES = BarsOfData(3).Close;
            fvVX = BarsOfData(4).Close;
		}

		protected override void CalcBar()
        {
            if (1 == Bars.CurrentBar)
                m_bbDollarChange.Value = 0;

            var dollarChangeES = m_hedge_indic.Plots[0].Values[1] * 50 * (fvES[0] - fvES[1]);
            var dollarChangeVX = m_hedge_indic.Plots[1].Values[1] * 1000 * (fvVX[0] - fvVX[1]);
            m_bbDollarChange.Value = dollarChangeES + dollarChangeVX;
            plot1.Set(0, m_bbDollarChange.Value);
		}
	}
}