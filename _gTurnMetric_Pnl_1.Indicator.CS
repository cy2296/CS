using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTurnMetric_Pnl_1 : IndicatorObject 
    {
        private VariableSeries<Double> m_bbDollarChange;
        private IPlotObject plot1;
        private _gSingleReturnHedge m_single_hedge_indic;

        public _gTurnMetric_Pnl_1(object _ctx) : base(_ctx) { }
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
		}
		protected override void CalcBar()
        {
            m_bbDollarChange.Value = m_bbDollarChange[1] + m_single_hedge_indic.Plots[0].Values[0];
            plot1.Set(0, m_bbDollarChange.Value);
		}
	}
}