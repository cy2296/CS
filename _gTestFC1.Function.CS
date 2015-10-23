using System;
using System.Drawing;
using System.Linq;

using System.Text;
using System.IO;
using System.Collections.Generic;
namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gTestFC1 : FunctionSeries<System.Double>
		{
            private System.Text.StringBuilder csv;  // for debug only

            private List<Double> m_ohlc;


			public _gTestFC1(CStudyControl _master) : base(_master) { }
			public _gTestFC1(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }
            public double leg4 { get; set; }

			protected override void Create()
			{
                csv = new StringBuilder();
			}
			protected override void StartCalc()
			{
			}
			protected override System.Double CalcBar()
			{
                double open = ((leg1 != 0) ? Bars.Open[0] * leg1 : 0)
                            + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                            + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0)
                            + ((leg4 != 0) ? BarsOfData(4).Open[0] * leg4 : 0);
                double high = ((leg1 != 0) ? Bars.High[0] * leg1 : 0)
                            + ((leg2 != 0) ? BarsOfData(2).High[0] * leg2 : 0)
                            + ((leg3 != 0) ? BarsOfData(3).High[0] * leg3 : 0)
                            + ((leg4 != 0) ? BarsOfData(4).High[0] * leg4 : 0);
                double low = ((leg1 != 0) ? Bars.Low[0] * leg1 : 0)
                           + ((leg2 != 0) ? BarsOfData(2).Low[0] * leg2 : 0)
                           + ((leg3 != 0) ? BarsOfData(3).Low[0] * leg3 : 0)
                           + ((leg4 != 0) ? BarsOfData(4).Low[0] * leg4 : 0);
                double close = ((leg1 != 0) ? Bars.Close[0] * leg1 : 0)
                             + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                             + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0)
                             + ((leg4 != 0) ? BarsOfData(4).Close[0] * leg4 : 0);


                //if (Bars.CurrentBar < 500)
                //{
                //    var x = Bars.TimeValue.Date;
                //    var a = Bars.TimeValue.TimeOfDay;
                //    var b = Bars.CurrentBar;
                //    var z = m_high.Value;
                //    var g = m_low.Value;
                //    var p = c[0];
                //    var d = m_pos[1];
                //    var e = StrategyInfo.MarketPosition;
                //    var f = m_currHigh;
                //    var j = m_currLow;

                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}{10}",
                //    x, a, b, z, g, p, d, e, f, j, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    //System.IO.File.WriteAllText("H:/eSignalData/debug.csv", csv.ToString());
                //    System.IO.File.WriteAllText("C:/data/debug.csv", csv.ToString());
                //}
                return Math.Round(open, 4);
            }
		}
	}
}