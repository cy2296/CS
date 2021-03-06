﻿using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSingleReturn : IndicatorObject 
    {
		public _gSingleReturn(object _ctx):base(_ctx){}
        private VariableSeries<Double> m_bbDollarChange;  // buy buy dollar change
        private IPlotObject plot1;

        private ISeries<double> fvES { get; set; }
        private ISeries<double> fvVX { get; set; }

        [Input]
        public int sizeFactor { get; set; }

        protected override void Create()
        {
            m_bbDollarChange = new VariableSeries<Double>(this);
            plot1 = AddPlot(new PlotAttributes("BuyBuy", EPlotShapes.Histogram, 
                Color.Cyan, Color.Empty, 0, 0, true));
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

            var dollarChangeES = sizeFactor * 50 * (fvES[0] - fvES[1]);
            var dollarChangeVX = sizeFactor * 1000 * (fvVX[0] - fvVX[1]);
            var dollarChangeTot = dollarChangeES + dollarChangeVX;
            m_bbDollarChange.Value = dollarChangeTot;

            plot1.Set(0, m_bbDollarChange.Value);
        }
	}
}