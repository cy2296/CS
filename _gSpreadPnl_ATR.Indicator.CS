using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using System.IO;
using System.Text;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator{

	public class _gSpreadPnl_ATR : IndicatorObject 
    {
		private _gSpreadPnlFC m_sprdPnl;
        private _gATR m_atrFC;
        private VariableSeries<int> m_pos;
        private double spreadBigPtVal;
        private VariableSeries<double> m_atr;

        private System.Text.StringBuilder csv;  // for debug only

        public _gSpreadPnl_ATR(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            atrlen = 15;
            atrnum = 2;
            bigPt1 = 1000;
            bigPt2 = 500;
            bigPt3 = 1000;
            slippage = 0;
            type = 1;
        }

		private IPlotObject plot1;
        private IPlotObject plot2;

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
 
        protected override void Create() 
        {
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_atrFC = new _gATR(this);
            m_pos = new VariableSeries<int>(this);
            m_atr = new VariableSeries<double>(this);

			plot1 = AddPlot(new PlotAttributes("PNL", EPlotShapes.Histogram, Color.Aqua));
            plot2 = AddPlot(new PlotAttributes("ATR", EPlotShapes.Line, Color.Yellow));

            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
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
			plot1.Set(m_sprdPnl[0]);

            if (type == 1)  // on lossing exit 
            {
                if (m_pos.Value == 0)
                {
                    if (m_pos[1] != 0)
                        m_atr.Value = -1 * m_atrFC[0] * atrnum * Math.Abs(m_pos[1] / leg1) * spreadBigPtVal;
                    else
                        m_atr.Value = 0;
                    //m_atr.Value = -1 * m_atrFC[0] * atrnum * Math.Abs(6 / leg1) * spreadBigPtVal;
                }
                else  // m_pos.Value != 0
                {
                    //if (m_sprdPnl[0] < 0)
                        m_atr.Value = -1 * m_atrFC[0] * atrnum * Math.Abs(m_pos[0] / leg1) * spreadBigPtVal;
                    //else
                    //    m_atr.Value = 0;
                }
                plot2.Set(m_atr.Value);
            }
            if (type == 2)  // on winning exit 
            {
                if (m_pos.Value == 0)
                {
                    if (m_pos[1] != 0)
                        m_atr.Value = m_atrFC[0] * atrnum * Math.Abs(m_pos[1] / leg1) * spreadBigPtVal;
                    else
                        m_atr.Value = 0;
                        //m_atr.Value = m_atrFC[0] * atrnum * Math.Abs(6 / leg1) * spreadBigPtVal;
                }
                else  // m_pos.Value != 0
                {
                    //if (m_sprdPnl[0] < 0)
                    m_atr.Value = m_atrFC[0] * atrnum * Math.Abs(m_pos[0] / leg1) * spreadBigPtVal;
                    //else
                    //    m_atr.Value = 0;
                }
                plot2.Set(m_atr.Value);
            }





            //if (Bars.CurrentBar < 5000)
            //{
            //    var x = Bars.TimeValue.Date;
            //    var a = Bars.TimeValue.TimeOfDay;
            //    var b = Bars.CurrentBar;
            //    var c = Bars.Close[0] * leg1 + BarsOfData(2).Close[0] * leg2 + BarsOfData(3).Close[0] * leg3;
            //    var y = StrategyInfo.MarketPosition;
            //    var d = m_sprdPnl[0];
            //    var e = m_atr.Value;
            //    var f = (Math.Abs(m_sprdPnl[0]) >= m_atr.Value) ? 1 : 0;

            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}{8}",
            //    x, a, b, c, y, d, e, f, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
            //}
		}
	}
}