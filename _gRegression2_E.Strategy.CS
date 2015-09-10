using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
    [IOGMode(IOGMode.Enabled)]
	public class _gRegression2_E : SignalObject 
    {
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_mp;
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _gBarAvg m_avg;
        private _gBarClose m_close;
        private LinearRegValue m_linearregvalue1;      // FunctionSimple : for regression value 
        private VariableSeries<Double> m_linreg;       // linear regression line    
        private XAverage m_linregEma;                  // FuncitonSeries : EMA of the linear regression
        private VariableSeries<Double> m_twoLineGap;   //  
        private VariableSeries<int> m_gapIncreasing;   // (gap[0] > gap[1]) ? 1 : 0
        private VariableSeries<int> m_gapDecreasing;   // (gap[0] < gap[1]) ? 1 : 0
        private VariableSeries<bool> m_upTrend;
        private VariableSeries<bool> m_dnTrend;

        private double m_highBand;
        private double m_lowBand;
        private double m_highPrice;
        private double m_lowPrice;
        private int m_upCount;
        private int m_dnCount;
        private double m_stopLoss;

        private IOrderMarket m_regression2_LE_Mkt;
        private IOrderMarket m_regression2_SE_Mkt;
        private IOrderMarket m_regression2_LX_Mkt;
        private IOrderMarket m_regression2_SX_Mkt;

        private _gSpreadPnlFC m_sprdPnl;

		public _gRegression2_E(object _ctx):base(_ctx)
        {
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //highBand = 0.03;
            //lowBand = 0.03;
            //highLimit = 3;
            //lowLimit = 3;
            //stopLoss = 1;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //highBand = 0.03;
            //lowBand = 0.03;
            //highLimit = 3;
            //lowLimit = 3;
            //stopLoss = 1;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;

            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            highBand = 0.03;
            lowBand = 0.03;
            highLimit = 3;
            lowLimit = 3;
            stopLoss = 1;
            reglength = 20;
            emalength = 4;
            avgLen = 5;         
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
        public double highLimit { get; set; }
        [Input]
        public double lowLimit { get; set; }
        [Input]
        public double stopLoss { get; set; }
        [Input]
        public int reglength { get; set; }
        [Input]
        public int emalength { get; set; }
        [Input]
        public int avgLen { get; set; }
        [Input]
        public string StartTime { get; set; }
        [Input]
        public string EndTime { get; set; }

		protected override void Create() 
        {
            StartTime = "9:30";
            EndTime = "15:30";
            m_mp = new VariableSeries<int>(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_avg = new _gBarAvg(this);
            m_close = new _gBarClose(this);
            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<double>(this);
            m_linregEma = new XAverage(this);
            m_twoLineGap = new VariableSeries<double>(this);
            m_gapIncreasing = new VariableSeries<int>(this);
            m_gapDecreasing = new VariableSeries<int>(this);
            m_upTrend = new VariableSeries<bool>(this);
            m_dnTrend = new VariableSeries<bool>(this);
            m_regression2_LE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_regression2_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_regression2_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LStop", EOrderAction.Sell, OrderExit.FromAll));
            m_regression2_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SStop", EOrderAction.BuyToCover, OrderExit.FromAll));
            m_sprdPnl = new _gSpreadPnlFC(this);
		}

		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
            m_low.leg1 = leg1;
            m_low.leg2 = leg2;
            m_low.leg3 = leg3;
            m_avg.leg1 = leg1;
            m_avg.leg2 = leg2;
            m_avg.leg3 = leg3;
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_linearregvalue1.price = m_avg;
            m_linearregvalue1.length = reglength;

            m_linregEma.Price = m_linreg;
            m_linregEma.Length = emalength;

            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.calltype = "strat";
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;

            m_linreg.Value = m_linearregvalue1[0];
            m_twoLineGap.Value = m_linearregvalue1[0] - m_linregEma[0];
            m_gapIncreasing.Value = (m_twoLineGap[0] > m_twoLineGap[1]) ? 1 : 0;
            m_gapDecreasing.Value = (m_twoLineGap[0] < m_twoLineGap[1]) ? 1 : 0;
            m_upTrend.Value = (getCount(m_gapIncreasing, avgLen) >= avgLen - 1 && m_twoLineGap[0] > m_twoLineGap[1]);
            m_dnTrend.Value = (getCount(m_gapDecreasing, avgLen) >= avgLen - 1 && m_twoLineGap[0] < m_twoLineGap[1]);

            if (Math.Abs(m_twoLineGap[0]) < Math.Abs(highBand))
                m_upCount = 0;
            if (Math.Abs(m_twoLineGap[0]) < Math.Abs(lowBand))
                m_dnCount = 0;


            //if (m_mp[0] >= 0
            //    && (m_upTrend[0] && m_twoLineGap[0] > 0.02)
            //    //&& m_twoLineGap[0] < lowBand
            //    && TimeToTrade())
            //{
            //    //m_regression_LE_Mkt.Send();
            //    m_regression2_SE_Mkt.Send();
            //}
            //if (m_mp[0] <= 0
            //    && (m_dnTrend[0] && m_twoLineGap[0] < -0.03)
            //    //&& m_twoLineGap[0] > highBand
            //    && TimeToTrade())
            //{
            //    //m_regression_SE_Mkt.Send();
            //    m_regression2_LE_Mkt.Send();
            //}

            // find short opportunities
            if (m_twoLineGap[0] > 0 && Math.Abs(m_twoLineGap[0]) > Math.Abs(highBand))
            {
                if (m_upCount == 0)
                {
                    m_highBand = m_twoLineGap[0];
                    m_highPrice = m_high[0];
                    ++m_upCount;
                }
                else
                {
                    m_highPrice = Math.Max(m_highPrice, m_high[0]);
                    if (m_twoLineGap[0] >= m_highBand)
                    {
                        m_highBand = m_twoLineGap[0];
                        ++m_upCount;
                    }
                }
                if (m_mp[0] >= 0 && m_upCount >= highLimit && m_upTrend[0] == false && m_upTrend[1] == true)
                    m_regression2_SE_Mkt.Send();
                    //m_regression2_LX_Mkt.Send();
            }
            // find long opportunities
            if (m_twoLineGap[0] < 0 && Math.Abs(m_twoLineGap[0]) > Math.Abs(lowBand))
            {
                if (m_dnCount == 0)
                {
                    m_lowBand = m_twoLineGap[0];
                    m_lowPrice = m_low[0];
                    ++m_dnCount;
                }
                else
                {
                    m_lowPrice = Math.Min(m_lowPrice, m_low[0]);
                    if (m_twoLineGap[0] <= m_lowBand)
                    {
                        m_lowBand = m_twoLineGap[0];
                        ++m_dnCount;
                    }
                }
                if (m_mp[0] <= 0 && m_dnCount >= lowLimit && m_dnTrend[0] == false && m_dnTrend[1] == true)
                    m_regression2_LE_Mkt.Send();
                   // m_regression2_SX_Mkt.Send();
            }
            //if (m_mp[0] > 0)
            //{
            //    if (m_mp[1] == 0)
            //    {
            //        double stop = Math.Abs(m_close[1] * 2 * 0.01);
            //        double inputStopPrice = m_close[1] - stop;
            //        //m_stopLoss = Math.Max(m_lowPrice, inputStopPrice);
            //        m_stopLoss = m_lowPrice;   // only use low point
            //    }
            //    else
            //    { 
            //        if (m_close[0] <= m_stopLoss)
            //            m_regression2_LX_Mkt.Send();
            //    }
            //}
            //if (m_mp[0] < 0)
            //{
            //    if (m_mp[1] == 0)
            //    {
            //        double stop = Math.Abs(m_close[1] * 2 * 0.01);
            //        double inputStopPrice = m_close[1] + stop;
            //        //m_stopLoss = Math.Min(m_highPrice, inputStopPrice);
            //        m_stopLoss = m_highPrice;   // only use high point               
            //    }
            //    else
            //    {
            //        if (m_close[0] >= m_stopLoss)
            //            m_regression2_SX_Mkt.Send();
            //    }
            //}
        }
        private int getCount(VariableSeries<int> count, int length)
        {
            int res = 0;
            for (int i = 0; i < length; ++i)
                res += count[i];
            return res;
        }

        private TimeSpan ConvertToTime(string timeToConvert)
        {
            DateTime dt = DateTime.ParseExact(timeToConvert, "H:mm",
                System.Globalization.CultureInfo.InvariantCulture);
            return dt.TimeOfDay;
        }

        private bool TimeToTrade()
        {
            TimeSpan barTime = Bars.TimeValue.TimeOfDay;
            if (startTime < endTime)
                return (barTime >= startTime) && (barTime <= endTime);
            return (barTime > startTime) || (barTime < endTime);
        }
	}
}