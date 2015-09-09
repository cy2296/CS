using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gLinearReg : FunctionSimple<System.Double>
		{
			public _gLinearReg(CStudyControl _master) : base(_master) { }
			public _gLinearReg(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> price { get; set; }

            public Int32 length { get; set; }

            public Int32 tgtbar { get; set; }

            public double olrslope { get; set; }

            public double olrangle { get; set; }

            public double olrintercept { get; set; }

            public double olrvalue { get; set; }

            public double leg1 { get; set; }

            public double leg2 { get; set; }

            public double leg3 { get; set; }

			protected override System.Double CalcBar()
			{
                if (length > 1)
                {
                    var d_length = (double)length;
                    var m_sumx = d_length * (d_length - 1) * 0.5;
                    var m_sumxsqr = d_length * (d_length - 1) * (2 * d_length - 1) * 1.0 / 6.0;
                    var m_divisor = Math.Pow(m_sumx, 2) - d_length * m_sumxsqr;
                    var m_sumxy = .0;
                    for (var i = 0; i < d_length; i++)
                    {
                        m_sumxy += i * price[i];
                    }
                    var m_sumy = price.Summation(length);
                    olrslope = (d_length * m_sumxy - m_sumx * m_sumy) / m_divisor;
                    olrangle = Math.Atan(olrslope);
                    olrintercept = (m_sumy - olrslope * m_sumx) / d_length;
                    olrvalue = olrintercept + olrslope * (d_length - 1 + ExecInfo.ExecOffset - tgtbar);
                    return 1;
                }
                return -1;
			}
            private bool MarketsAreOpened()
            {
                if ((leg2 == 0 && leg3 == 0) ||
                    (leg2 != 0 && leg3 == 0
                     && Bars.TimeValue.Date == BarsOfData(2).TimeValue.Date
                     && Bars.TimeValue.TimeOfDay == BarsOfData(2).TimeValue.TimeOfDay) ||
                    (leg2 != 0 && leg3 != 0
                     && Bars.TimeValue.Date == BarsOfData(2).TimeValue.Date
                     && Bars.TimeValue.Date == BarsOfData(3).TimeValue.Date
                     && Bars.TimeValue.TimeOfDay == BarsOfData(2).TimeValue.TimeOfDay
                     && Bars.TimeValue.TimeOfDay == BarsOfData(3).TimeValue.TimeOfDay))
                {
                    return true;
                }
                else
                    return false;
            }
		}
	}
}