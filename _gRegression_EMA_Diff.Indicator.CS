using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gRegression_EMA_Diff : IndicatorObject 
    {
        private _gBarAvg m_avgprice;
        private LinearRegValue m_linearregvalue1;      // FunctionSimple : for regression value 
        private VariableSeries<Double> m_linreg;       // linear regression line    
        private XAverage m_linregEma;                  // FuncitonSeries : EMA of the linear regression
        private VariableSeries<Double> m_twoLineGap;   //  
        private VariableSeries<int> m_gapIncreasing;   // (gap[0] > gap[1]) ? 1 : 0
        private VariableSeries<int> m_gapDecreasing;   // (gap[0] < gap[1]) ? 1 : 0
        //private SummationFC m_sumUpCount;
        //private SummationFC m_sumDnCount;
        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;

        public _gRegression_EMA_Diff(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            highBand = 90;
            lowBand = 10;
            reglength = 20;
            emalength = 4;
            avgLen = 5;

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //highBand = 90;
            //lowBand = 10;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;

            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //highBand = 90;
            //lowBand = 10;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;        
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

        protected override void Create() 
        {
            m_avgprice = new _gBarAvg(this);
            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<double>(this);
            m_linregEma = new XAverage(this);
            m_twoLineGap = new VariableSeries<double>(this);
            m_gapIncreasing = new VariableSeries<int>(this);
            m_gapDecreasing = new VariableSeries<int>(this);
            plot1 = AddPlot(new PlotAttributes("Gap", EPlotShapes.Histogram, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("Gap", EPlotShapes.Line, Color.Yellow));
            plot3 = AddPlot(new PlotAttributes("Gap", EPlotShapes.Line, Color.Yellow));
		}
		
        protected override void StartCalc() 
        {
            m_avgprice.leg1 = leg1;
            m_avgprice.leg2 = leg2;
            m_avgprice.leg3 = leg3;
            m_linearregvalue1.price = m_avgprice;
            m_linearregvalue1.length = reglength;

            m_linregEma.Price = m_linreg;
            m_linregEma.Length = emalength;
		}
		
        protected override void CalcBar()
        {
            m_linreg.Value = m_linearregvalue1[0];
            m_twoLineGap.Value = m_linearregvalue1[0] - m_linregEma[0];
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
		}
        private int getCount(VariableSeries<int> count, int length)
        {
            int res = 0;
            for (int i = 0; i < length; ++i)
                res += count[i];
            return res;
        }
	}
}