using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class gRegression_E : SignalObject
    {
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_mp;   // position info
        private VariableSeries<Double> m_averagePrice;  // ISeries for linear Regression Line
        private LinearRegValue m_linearregvalue1;      // FunctionSimple : for regression value 
        private VariableSeries<Double> m_linreg;       // linear regression line    
        private XAverage m_linregEma;                  // FuncitonSeries : EMA of the linear regression
        private VariableSeries<Double> m_twoLineGap;   //  
        private VariableSeries<int> m_gapIncreasing;   // (gap[0] > gap[1]) ? 1 : 0
        private VariableSeries<int> m_gapDecreasing;   // (gap[0] < gap[1]) ? 1 : 0
        private bool upTrend;
        private bool dnTrend;
        private IOrderMarket m_regression_LE_Mkt;
        private IOrderMarket m_regression_SE_Mkt;
        private IOrderMarket m_regression_LX_Mkt;
        private IOrderMarket m_regression_SX_Mkt;

        private _gSpreadPnlFC m_sprdPnl;
       
        //test
        private XAverage m_longTermAvg;

        public gRegression_E(object _ctx):base(_ctx)
        {
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //highBand = 0.02;
            //lowBand = 0.03;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //highBand = 0.03;
            //lowBand = 0.02;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;

            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            highBand = 0.03;
            lowBand = 0.02;
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
            m_averagePrice = new VariableSeries<double>(this);
            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<double>(this);
            m_linregEma = new XAverage(this);
            m_twoLineGap = new VariableSeries<double>(this);
            m_gapIncreasing = new VariableSeries<int>(this);
            m_gapDecreasing = new VariableSeries<int>(this);
            m_regression_LE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_regression_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_regression_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_regression_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
            m_sprdPnl = new _gSpreadPnlFC(this);

            m_longTermAvg = new XAverage(this);

		}
        protected override void StartCalc()
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

            m_linearregvalue1.price = m_averagePrice;
            m_linearregvalue1.length = reglength;

            m_linregEma.Price = m_linreg;
            m_linregEma.Length = emalength;

            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.calltype = "strat";

            m_longTermAvg.Price = m_averagePrice;
            m_longTermAvg.Length = 160;
		}
		protected override void CalcBar()
        {
            if (MarketsAreOpened())
            {
                m_mp.Value = StrategyInfo.MarketPosition;
                if (leg2 == 0 && leg3 == 0)
                    m_averagePrice.Value = (Bars.High[0] + Bars.Low[0] + Bars.Close[0]) / 3;
                else
                {
                    double open = Bars.Open[0] * leg1
                                + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                                + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
                    double close = Bars.Close[0] * leg1
                                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);
                    m_averagePrice.Value = 0.5 * (open + close);
                }
                m_linreg.Value = m_linearregvalue1[0];
                m_twoLineGap.Value = m_linearregvalue1[0] - m_linregEma[0];
                m_gapIncreasing.Value = (m_twoLineGap[0] > m_twoLineGap[1]) ? 1 : 0;
                m_gapDecreasing.Value = (m_twoLineGap[0] < m_twoLineGap[1]) ? 1 : 0;

                upTrend = (getCount(m_gapIncreasing, avgLen) >= avgLen - 1 && m_twoLineGap[0] > m_twoLineGap[1]);
                dnTrend = (getCount(m_gapDecreasing, avgLen) >= avgLen - 1 && m_twoLineGap[0] < m_twoLineGap[1]);

                if (m_mp[0] >= 0
                    && (upTrend && m_twoLineGap[0] > Math.Abs(highBand))
                    //&& m_twoLineGap[0] < lowBand
                    //&& m_longTermAvg[0] < m_longTermAvg[1]
                    && TimeToTrade())
                {
                    //m_regression_LE_Mkt.Send();
                    m_regression_SE_Mkt.Send();
                }
                if (m_mp[0] <= 0
                    && (dnTrend && m_twoLineGap[0] < -Math.Abs(lowBand))
                    //&& m_longTermAvg[0] > m_longTermAvg[1]
                    //&& m_twoLineGap[0] > highBand
                    && TimeToTrade())
                {
                    //m_regression_SE_Mkt.Send();
                    m_regression_LE_Mkt.Send();
                }

                //if (m_mp[0] >= 0
                //    && (upTrend && m_twoLineGap[0] > Math.Abs(highBand))
                //    && TimeToTrade())
                //{
                //    m_regression_LX_Mkt.Send();
                //}
                //if (m_mp[0] <= 0
                //    && (dnTrend && m_twoLineGap[0] < -Math.Abs(lowBand))
                //    && TimeToTrade())
                //{
                //    m_regression_SX_Mkt.Send();
                //}

                //if (m_sprdPnl[0] >= 0.2|| m_sprdPnl[0] <= -0.2)
                //{
                //    if (m_mp[0] > 0)
                //        m_regression_LX_Mkt.Send();
                //    if (m_mp[0] < 0)
                //        m_regression_SX_Mkt.Send();
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