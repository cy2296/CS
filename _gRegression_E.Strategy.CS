using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gRegression_E : SignalObject
    {
        private VariableSeries<int> m_mp;   // position info
        private LinearRegValue m_linearregvalue1;
        private VariableSeries<Double> m_linreg;

        private VariableSeries<Double> m_spreadClose;

        private XAverage m_averagefc1;

        private VariableSeries<Double> m_twoLineGap;
        private VariableSeries<double> m_gapIsPositive;
        private VariableSeries<double> m_gapIsNegative;
        private SummationFC m_sumUpCount;
        private SummationFC m_sumDnCount;
        private bool upTrend;
        private bool dnTrend;

        private IOrderMarket m_regression_LE_Mkt;
        private IOrderMarket m_regression_SE_Mkt;

		public _gRegression_E(object _ctx):base(_ctx)
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
        public int highBand { get; set; }
        [Input]
        public int lowBand { get; set; }
        [Input]
        public int reglength { get; set; }
        [Input]
        public int emalength { get; set; }
        [Input]
        public int avgLen { get; set; }

		protected override void Create() 
        {
            m_mp = new VariableSeries<int>(this);
            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<Double>(this);
            m_spreadClose = new VariableSeries<Double>(this);
            m_averagefc1 = new XAverage(this);
            m_twoLineGap = new VariableSeries<double>(this);
            m_regression_LE_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_regression_SE_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_gapIsPositive = new VariableSeries<double>(this);
            m_gapIsNegative = new VariableSeries<double>(this);
            m_sumUpCount = new SummationFC(this);
            m_sumDnCount = new SummationFC(this);
		}
		protected override void StartCalc() 
        {
            m_linearregvalue1.price = (leg2 == 0 && leg3 == 0) ? Bars.Close : m_spreadClose;
            m_linearregvalue1.length = reglength;
            
            m_averagefc1.Price =  m_linreg;
            m_averagefc1.Length = emalength;

            m_sumUpCount.price = m_gapIsPositive;
            m_sumUpCount.length = avgLen;

            m_sumDnCount.price = m_gapIsNegative;
            m_sumDnCount.length = avgLen;
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;
            m_spreadClose.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);
            m_linreg.Value = m_linearregvalue1[0];
            m_twoLineGap.Value = m_linearregvalue1[0] - m_averagefc1[0];
            m_gapIsPositive.Value = (m_twoLineGap[0] > 0) ? 1 : 0;
            m_gapIsNegative.Value = (m_twoLineGap[0] < 0) ? 1 : 0;
            upTrend = (m_sumUpCount[0] > avgLen - 1);
            dnTrend = (m_sumDnCount[0] > avgLen - 1);
            if (m_mp[0] == 0 && (upTrend || m_twoLineGap[0] > highBand))
                m_regression_LE_Mkt.Send();
            if (m_mp[0] == 0 && (dnTrend || m_twoLineGap[0] < lowBand))
                m_regression_SE_Mkt.Send();
		}
	}
}