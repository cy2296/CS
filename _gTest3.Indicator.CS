﻿ using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTest3 : IndicatorObject {
        private _gSpreadPnlFC m_sprdPnl;

        private _gATR m_atrFC;
        private VariableSeries<int> m_pos;
        private double spreadBigPtVal;
        private VariableSeries<double> m_atr;

		public _gTest3(object _ctx):base(_ctx){
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            atrlen = 15;
            atrnum = 2;
            bigPt1 = 1000;
            bigPt2 = 500;
            bigPt3 = 1000;
            slippage = 20;
            type = 1;
        }
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int atrlen { get; set; }
        [Input]
        public int atrnum { get; set; }
        [Input]
        public double bigPt1 { get; set; }
        [Input]
        public double bigPt2 { get; set; }
        [Input]
        public double bigPt3 { get; set; }
        [Input]
        public double slippage { get; set; }
        [Input]
        public int type { get; set; }    // 1: on loosing, 2: on winning 

		private IPlotObject plot1;
		protected override void Create() {
            m_atrFC = new _gATR(this);
            m_pos = new VariableSeries<int>(this);
            m_atr = new VariableSeries<double>(this);
            m_sprdPnl = new _gSpreadPnlFC(this);
			// create variable objects, function objects, plot objects etc.
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Histogram, Color.Aqua));
		}
		protected override void StartCalc() {
            m_sprdPnl.bigPt1 = bigPt1;
            m_sprdPnl.bigPt2 = bigPt2;
            m_sprdPnl.bigPt3 = bigPt3;
            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.slippage = slippage;
            m_sprdPnl.calltype = "indic";
            m_atrFC.leg1 = leg1;
            m_atrFC.leg2 = leg2;
            m_atrFC.leg3 = leg3;
            m_atrFC.atrlen = atrlen;
            spreadBigPtVal = Math.Abs(bigPt1 * leg1 + bigPt2 * leg2 + bigPt3 * leg3);
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            var dollarRatio = Math.Abs(m_pos.Value / leg1) * spreadBigPtVal;
            //plot1.Set(m_sprdPnl[0]);
            if (type == 1)  // on lossing
            {
                if (m_pos.Value == 0)
                {
                    if (m_pos[1] != 0 && m_sprdPnl[0] < 0)   // just close position
                    {
                        m_atr.Value = m_atrFC[0] * atrnum * Math.Abs(m_pos[1] / leg1) * spreadBigPtVal;
                        plot1.Set(m_atr.Value - Math.Abs(m_sprdPnl.Value));
                    }
                    else
                        plot1.Set(0);
                }
                else  // m_pos.Value != 0
                {
                    if (m_sprdPnl[0] < 0)
                    {
                        m_atr.Value = m_atrFC[0] * atrnum * Math.Abs(m_pos[0] / leg1) * spreadBigPtVal;

                        if (Math.Abs(m_sprdPnl[0]) > m_atr.Value)
                            plot1.Set(m_atr.Value - Math.Abs(m_sprdPnl[0]));
                        else
                            plot1.Set(0);
                    }
                    else
                        plot1.Set(0);
                }
            }
            if (type == 2)  // on winning
            {
                if (m_pos.Value == 0)
                {
                    if (m_pos[1] != 0 && m_sprdPnl[0] > 0)   // just close position
                    {
                        m_atr.Value = m_atrFC[0] * atrnum * Math.Abs(m_pos[1] / leg1) * spreadBigPtVal;
                        plot1.Set(Math.Abs(m_sprdPnl.Value) - m_atr.Value);
                    }
                    else
                        plot1.Set(0);
                }
                else  // m_pos.Value != 0
                {
                    if (m_sprdPnl[0] > 0)
                    {
                        m_atr.Value = m_atrFC[0] * atrnum * Math.Abs(m_pos[0] / leg1) * spreadBigPtVal;

                        if (Math.Abs(m_sprdPnl[0]) > m_atr.Value)
                            plot1.Set(Math.Abs(m_sprdPnl[0]) - m_atr.Value);
                        else
                            plot1.Set(0);
                    }
                    else
                        plot1.Set(0);
                }
            }        

		}
	}
}