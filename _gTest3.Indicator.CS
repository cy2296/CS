 using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator
{
	public class _gTest3 : IndicatorObject 
    {
        private Function._gSpreadFC m_spread;
        private _gSpreadRSI m_spreadRsiFC;
        private _gSpreadRsiUpBB m_spreadRsiUpBbFC;
        private _gSpreadRsiDnBB m_spreadRsiDnBbFC;

        private System.Text.StringBuilder csv;  // for debug only

		public _gTest3(object _ctx):base(_ctx)
        {
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;
            len_rsi = 10;
            numdevsup = 1.95;
            numdevsdn = 1.95;
            len_bb = 15;
        }

		private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public Int32 len_rsi { get; set; }
        [Input]
        public double numdevsup { get; set; }
        [Input]
        public double numdevsdn { get; set; }
        [Input]
        public Int32 len_bb { get; set; }

        protected override void Create() 
        {
            m_spread = new Function._gSpreadFC(this);
            m_spreadRsiFC = new _gSpreadRSI(this);
            m_spreadRsiUpBbFC = new _gSpreadRsiUpBB(this);
            m_spreadRsiDnBbFC = new _gSpreadRsiDnBB(this);
            plot1 = AddPlot(new PlotAttributes("RSI", 0, Color.Silver,
                                           Color.Empty, 0, 0, true));
            plot2 =
                AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            plot3 =
                AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));

            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
            m_spread.inst1 = Bars.Close;
            m_spread.inst2 = BarsOfData(2).Close;
            m_spread.inst3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spread.leg1 = leg1;
            m_spread.leg2 = leg2;
            m_spread.leg3 = leg3;

            m_spreadRsiFC.price1 = Bars.Close;
            m_spreadRsiFC.price2 = BarsOfData(2).Close;
            m_spreadRsiFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiFC.leg1 = leg1;
            m_spreadRsiFC.leg2 = leg2;
            m_spreadRsiFC.leg3 = leg3;
            m_spreadRsiFC.length = len_rsi;

            m_spreadRsiUpBbFC.price1 = Bars.Close;
            m_spreadRsiUpBbFC.price2 = BarsOfData(2).Close;
            m_spreadRsiUpBbFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiUpBbFC.leg1 = leg1;
            m_spreadRsiUpBbFC.leg2 = leg2;
            m_spreadRsiUpBbFC.leg3 = leg3;
            m_spreadRsiUpBbFC.len_rsi = len_rsi;
            m_spreadRsiUpBbFC.numdevsdn = numdevsdn;
            m_spreadRsiUpBbFC.numdevsup = numdevsup;
            m_spreadRsiUpBbFC.len_bb = len_bb;

            m_spreadRsiDnBbFC.price1 = Bars.Close;
            m_spreadRsiDnBbFC.price2 = BarsOfData(2).Close;
            m_spreadRsiDnBbFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiDnBbFC.leg1 = leg1;
            m_spreadRsiDnBbFC.leg2 = leg2;
            m_spreadRsiDnBbFC.leg3 = leg3;
            m_spreadRsiDnBbFC.len_rsi = len_rsi;
            m_spreadRsiDnBbFC.numdevsdn = numdevsdn;
            m_spreadRsiDnBbFC.numdevsup = numdevsup;
            m_spreadRsiDnBbFC.len_bb = len_bb;

            var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}{7}",
            "Date", "Time", "CurrBar", "Spread", "RSI", "BB_Up", "BB_Dn", System.Environment.NewLine);
            csv.Append(newLine);
            System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
		}
		protected override void CalcBar()
        {
            plot1.Set(m_spreadRsiFC[0]);
            plot2.Set(m_spreadRsiUpBbFC[0]);
            plot3.Set(m_spreadRsiDnBbFC[0]);


            if (Bars.CurrentBar < 500)
            {
                var x = Bars.TimeValue.Date;
                var a = Bars.TimeValue.TimeOfDay;
                var b = Bars.CurrentBar;
                var c = m_spread[0];
                var d = m_spreadRsiFC[0];
                var e = m_spreadRsiUpBbFC[0];
                var f = m_spreadRsiDnBbFC[0];
            
                var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}{7}",
                x, a, b, c, d, e, f, System.Environment.NewLine);
                csv.Append(newLine);
                System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
            }
            
		}
	}
}