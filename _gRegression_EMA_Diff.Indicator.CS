using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator
{
	public class _gRegression_EMA_Diff : IndicatorObject 
    {
        private LinearRegValue m_linearregvalue1;      // FunctionSimple : for regression value 
        private VariableSeries<Double> m_linreg;       // linear regression line    
        private XAverage m_linregEma;                  // FuncitonSeries : EMA of the linear regression
        private VariableSeries<Double> m_twoLineGap;   //  
        private VariableSeries<int> m_gapIncreasing;   // (gap[0] > gap[1]) ? 1 : 0
        private VariableSeries<int> m_gapDecreasing;   // (gap[0] < gap[1]) ? 1 : 0

        private VariableSeries<Double> m_average;


        private VariableSeries<Double> m_gap;
        private AverageFC m_gapAvg;
        
        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
        private IPlotObject plot4;
        private IPlotObject plot5;
        private IPlotObject plot6;
        private System.Text.StringBuilder csv;  // for debug only

        public _gRegression_EMA_Diff(object _ctx):base(_ctx)
        {
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //highBand = 0.03;
            //lowBand = 0.03;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;
            //length = 10;

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //highBand = 0.03;
            //lowBand = 0.03;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;
            //length = 10;

            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            highBand = 0.03;
            lowBand = 0.03;
            reglength = 20;
            emalength = 4;
            avgLen = 5;
            length = 10;
        }

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public double highBand { get; set; }
        [Input]
        public double lowBand { get; set; }
        [Input]
        public int reglength { get; set; }
        [Input]
        public int emalength { get; set; }
        [Input]
        public int avgLen { get; set; }
        [Input]
        public int length { get; set; }

        protected override void Create() 
        {
            m_average = new VariableSeries<double>(this);

            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<double>(this);
            m_linregEma = new XAverage(this);
            m_twoLineGap = new VariableSeries<double>(this);
            m_gapIncreasing = new VariableSeries<int>(this);
            m_gapDecreasing = new VariableSeries<int>(this);
            m_gap = new VariableSeries<double>(this);
            m_gapAvg = new AverageFC(this);
            plot1 = AddPlot(new PlotAttributes("Gap", EPlotShapes.Histogram, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("FixHigh", EPlotShapes.Line, Color.Yellow));
            plot3 = AddPlot(new PlotAttributes("FixLow", EPlotShapes.Line, Color.Yellow));
            plot4 = AddPlot(new PlotAttributes("SigHigh", EPlotShapes.Line, Color.Yellow));
            plot5 = AddPlot(new PlotAttributes("SigLow", EPlotShapes.Line, Color.Yellow));
            plot6 = AddPlot(new PlotAttributes("SigLow", EPlotShapes.Line, Color.Yellow));
            csv = new StringBuilder();
		}
		
        protected override void StartCalc() 
        {
            m_linearregvalue1.price = m_average;
            m_linearregvalue1.length = reglength;
            m_linearregvalue1.tgtbar = 0;

            m_linregEma.Price = m_linreg;
            m_linregEma.Length = emalength;

            m_gapAvg.price = m_gap;
            m_gapAvg.length = length;
		}
        protected override void CalcBar()
        {
            if (MarketsAreOpened())
            {
                if (leg2 == 0 && leg3 == 0)
                    m_average.Value = (Bars.High[0] + Bars.Low[0] + Bars.Close[0]) / 3;
                else
                {
                    double open = Bars.Open[0] * leg1
                                + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                                + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
                    double close = Bars.Close[0] * leg1
                                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);
                    m_average.Value = 0.5 * (open + close);
                }

                m_linreg.Value = m_linearregvalue1[0];
                m_twoLineGap.Value = m_linearregvalue1[0] - m_linregEma[0];
                m_gap.Value = Math.Abs(m_twoLineGap[0]);
                m_gapIncreasing.Value = (m_twoLineGap[0] > m_twoLineGap[1]) ? 1 : 0;
                m_gapDecreasing.Value = (m_twoLineGap[0] < m_twoLineGap[1]) ? 1 : 0;

                if (getCount(m_gapIncreasing, avgLen) >= avgLen - 1
                    && m_twoLineGap[0] > m_twoLineGap[1])
                {
                    plot1.Set(m_twoLineGap[0], Color.LawnGreen);
                }
                else if (getCount(m_gapDecreasing, avgLen) >= avgLen - 1
                    && m_twoLineGap[0] < m_twoLineGap[1])
                {
                    plot1.Set(m_twoLineGap[0], Color.Pink);
                }
                else
                    plot1.Set(m_twoLineGap[0], Color.Gray);

                plot2.Set(Math.Abs(highBand));
                plot3.Set(-1 * Math.Abs(lowBand));

                var m_sdev = m_gap.StandardDeviationCustom(length, 1);
                plot4.Set(m_gapAvg[0] + 2 * m_sdev);
                plot5.Set(-m_gapAvg[0] - 2 * m_sdev);

               // plot6.Set(m_average.Value);

                //if (Bars.CurrentBar < 2000)
                //{
                //    var a = Bars.TimeValue.Date;
                //    var b = Bars.TimeValue.TimeOfDay;
                //    var c = Bars.CurrentBar;
                //    var d = m_linreg.Value;
                //    var e = m_linregEma.Value;
                //    var f = m_twoLineGap[0];
 
                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}{6}",
                //    a, b, c, d, e, f, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
                //}

            }
		}
        private int getCount(VariableSeries<int> count, int length)
        {
            int res = 0;
            for (int i = 0; i < length; ++i)
                res += count[i];
            return res;
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