using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator{
	public class _gTest2 : IndicatorObject 
    {
        private VariableSeries<int> m_pos;
        private Function._gTestFC1 m_rsi1;

        private System.Text.StringBuilder csv;  // for debug only


		public _gTest2(object _ctx):base(_ctx)
        {
            spread_leg1 = 0.5;
            spread_leg2 = -1;
            spread_leg3 = 0.333;
            len_RSI = 10;
        }
		private IPlotObject plot1;

        [Input]
        public double spread_leg1 { get; set; }
        [Input]
        public double spread_leg2 { get; set; }
        [Input]
        public double spread_leg3 { get; set; }
        [Input]
        public int len_RSI { get; set; }

		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_rsi1 = new Function._gTestFC1(this);
			plot1 = AddPlot(new PlotAttributes("rsi", EPlotShapes.Line, Color.Aqua));

            csv = new StringBuilder();
		}
		protected override void StartCalc() {
            //m_rsi1.length = len_RSI;
            //m_rsi1.price1 = Bars.Close;
            //m_rsi1.price2 = BarsOfData(2).Close;
            //if (spread_leg3 != 0)
            //    m_rsi1.price3 = BarsOfData(3).Close;
            //else
            //    m_rsi1.price3 = Bars.Close;
            //m_rsi1.leg1 = spread_leg1;
            //m_rsi1.leg2 = spread_leg2;
            //m_rsi1.leg3 = spread_leg3;
		}
		protected override void CalcBar()
        {

            m_pos.Value = StrategyInfo.MarketPosition;
            //plot1.Set(m_pos.Value);
            plot1.Set(0, m_rsi1[0]);

            if (Bars.CurrentBar < 5000)
            {
                var x = Bars.TimeValue.Date;
                var a = Bars.TimeValue.TimeOfDay;
                var b = Bars.CurrentBar;
                var c = Bars.Close[0] * spread_leg1 + BarsOfData(2).Close[0] * spread_leg2 + BarsOfData(3).Close[0] * spread_leg3;
                //var y = StrategyInfo.MarketPosition;
                var d = m_rsi1[0];


                var newLine = string.Format("{0}, {1}, {2}, {3}, {4}{5}",
                x, a, b, c, d, System.Environment.NewLine);
                csv.Append(newLine);
                System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
            }
		}
	}
}