using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _test : IndicatorObject 
	{
        private VariableSeries<Double> m_bbDollarChange;
		private IPlotObject plot1;

        public _test(object _ctx) : base(_ctx) { }


        private _gSingleReturnHedge m_single_hedge_indic;
        //private ISeries<double> fvES { get; set; }
        //private ISeries<double> fvVX { get; set; }
        //[Input]
        //public int sizeFactor { get; set; }
        protected override void Create()
        {
            m_bbDollarChange = new VariableSeries<Double>(this);
            m_single_hedge_indic = (_gSingleReturnHedge)AddIndicator("_gSingleReturnHedge");
            plot1 = AddPlot(new PlotAttributes("BuyBuyPnl", 0, Color.Cyan, Color.Empty, 0, 0, true));

        }
        protected override void StartCalc()
		{
            //fvES = BarsOfData(3).Close;
            //fvVX = BarsOfData(4).Close;
		}
		protected override void CalcBar()
		{
            //if (1 == Bars.CurrentBar)
            //    m_bbDollarChange.Value = 0;

            //var dollarChangeES = 50 * (fvES[0] - fvES[1]);
            //var dollarChangeVX = 1000 * (fvVX[0] - fvVX[1]);
            //var dollarChangeTot = dollarChangeES + dollarChangeVX;
            m_bbDollarChange.Value = m_bbDollarChange[1] + m_single_hedge_indic.Plots[0].Values[0];

            plot1.Set(0, m_bbDollarChange.Value);
		}
	}
}