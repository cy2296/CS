using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy 
{
	public class _gRegression_E : SignalObject
    {
        private System.Text.StringBuilder csv;  // for debug only
        
        private _gUtility_TimeToTrade m_timeToTrade;
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

        //private _gSpreadPnlFC m_sprdPnl;

        // 
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _MK_CurrDHigh m_dhigh;
        private _MK_CurrDLow m_dlow;
      


        // Risk functions
        private VariableSeries<Double> m_close;
        private _gRisk_StopLongCount m_stoplongcount;
        private _gRisk_StopShortCount m_stopshortcount;

        //private _gRisk_StopForPos m_stopPos;
        private IOrderMarket m_regression_LE_Mkt;
        private IOrderMarket m_regression_SE_Mkt;
        private IOrderMarket m_regression_LX_Mkt;
        private IOrderMarket m_regression_SX_Mkt;

        // 


		public _gRegression_E(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            highBand = 0.03;
            lowBand = 0.03;
            reglength = 20;
            emalength = 4;
            avgLen = 5;
            StartTime = "9:30";
            EndTime = "15:30";

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //highBand = 0.03;
            //lowBand = 0.03;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5;
            //StartTime = "9:30";
            //EndTime = "15:30";

            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //highBand = 0.03;
            //lowBand = 0.03;
            //reglength = 20;
            //emalength = 4;
            //avgLen = 5; 
            //StartTime = "9:30";
            //EndTime = "15:30";
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
        [Input]
        public int length { get; set; }

		protected override void Create() 
        {
            // utility .. 
            csv = new StringBuilder();
            m_timeToTrade = new _gUtility_TimeToTrade(this);
            m_mp = new VariableSeries<int>(this);
            // regression ..
            m_averagePrice = new VariableSeries<double>(this);
            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<double>(this);
            m_linregEma = new XAverage(this);
            m_twoLineGap = new VariableSeries<double>(this);
            m_gapIncreasing = new VariableSeries<int>(this);
            m_gapDecreasing = new VariableSeries<int>(this);

            //
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_dhigh = new _MK_CurrDHigh(this);
            m_dlow = new _MK_CurrDLow(this);

            // risk ..
            m_close = new VariableSeries<double>(this);
            m_stoplongcount = new _gRisk_StopLongCount(this);
            m_stopshortcount = new _gRisk_StopShortCount(this);

            m_regression_LE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_regression_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_regression_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_regression_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));

            //m_sprdPnl = new _gSpreadPnlFC(this);
            //m_stopPos = new _gRisk_StopForPos(this);
		}
		protected override void StartCalc() 
        {
            m_timeToTrade.StartTime = StartTime;
            m_timeToTrade.EndTime = EndTime;

            m_linearregvalue1.price = m_averagePrice;
            m_linearregvalue1.length = reglength;
            m_linregEma.Price = m_linreg;
            m_linregEma.Length = emalength;

            m_stoplongcount.calltype = "strat";
            m_stoplongcount.startTime = StartTime;
            m_stoplongcount.endTime = EndTime;
            m_stoplongcount.price = m_close;
            m_stoplongcount.stopCriteria = 0.25;
            m_stopshortcount.calltype = "strat";
            m_stopshortcount.startTime = StartTime;
            m_stopshortcount.endTime = EndTime;
            m_stopshortcount.price = m_close;
            m_stopshortcount.stopCriteria = 0.25;


            m_dhigh.Data = m_high;
            m_dlow.Data = m_low;
            //m_sprdPnl.leg1 = leg1;
            //m_sprdPnl.leg2 = leg2;
            //m_sprdPnl.leg3 = leg3;
            //m_sprdPnl.calltype = "strat";

            //m_stopPos.pnl = m_sprdPnl;
            //m_stopPos.stop = 0.25;
        }
		protected override void CalcBar()
        {
            if (MarketsAreOpened())
            {
                m_mp.Value = StrategyInfo.MarketPosition;

                double mid = 0.5 * (m_dhigh[0] + m_dlow[0]);
                double midhigh = 0.5 * (m_dhigh[0] + mid);
                double midlow = 0.5 * (m_dlow[0] + mid);

                if (leg2 == 0 && leg3 == 0)
                    m_averagePrice.Value = (Bars.High[0] + Bars.Low[0] + Bars.Close[0]) / 3;
                else
                {
                    double open = Bars.Open[0] * leg1
                                + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                                + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
                    m_close.Value = Bars.Close[0] * leg1
                                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);
                    m_averagePrice.Value = 0.5 * (open + m_close[0]);
                }
                m_linreg.Value = m_linearregvalue1[0];
                m_twoLineGap.Value = m_linearregvalue1[0] - m_linregEma[0];
                m_gapIncreasing.Value = (m_twoLineGap[0] > m_twoLineGap[1]) ? 1 : 0;
                m_gapDecreasing.Value = (m_twoLineGap[0] < m_twoLineGap[1]) ? 1 : 0;
                upTrend = (getCount(m_gapIncreasing, avgLen) >= avgLen - 1 && m_twoLineGap[0] > m_twoLineGap[1]);
                dnTrend = (getCount(m_gapDecreasing, avgLen) >= avgLen - 1 && m_twoLineGap[0] < m_twoLineGap[1]);

                if (m_mp[0] >= 0
                    && upTrend
                    && m_twoLineGap[0] > Math.Abs(highBand)
                    //&& m_stopshortcount[0] == 0
                    //&& m_stopshortcount[0] + m_stoplongcount[0] < 2
                    && m_timeToTrade[0]
                    //&& m_twoLineGap[0] < lowBand
                    )
                {
                    //if (m_close[0] >= mid)
                        m_regression_SE_Mkt.Send();
                    //else
                    //    m_regression_LX_Mkt.Send();
                }
                if (m_mp[0] <= 0
                    && dnTrend
                    && m_twoLineGap[0] < -Math.Abs(lowBand)
                    //&& m_stoplongcount[0] == 0
                    //&& m_stopshortcount[0] + m_stoplongcount[0] < 2
                    && m_timeToTrade[0]
                    //&& m_twoLineGap[0] > highBand
                    )
                {
                    //if (m_close[0] <= mid)
                        m_regression_LE_Mkt.Send();
                    //else
                    //    m_regression_SX_Mkt.Send();
                }

                //if (m_stopPos[0] == 1)
                //{
                //    if (m_mp[0] > 0)
                //        m_regression_LX_Mkt.Send();
                //    if (m_mp[0] < 0)
                //        m_regression_SX_Mkt.Send();
                //}

                //if (Bars.CurrentBar < 2000)
                //{
                //    var a = Bars.TimeValue.Date;
                //    var b = Bars.TimeValue.TimeOfDay;
                //    var c = Bars.CurrentBar;
                //    var d = upTrend;
                //    var e = dnTrend;
                //    var f = m_twoLineGap[0];
                //    var g = m_mp[0];
                //    var h = m_stoplongcount[0];
                //    var i = m_stopshortcount[0];

                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}{9}",
                //    a, b, c, d, e, f, g, h, i, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    System.IO.File.WriteAllText("H:/eSignalData/debug0.csv", csv.ToString());
                //}

                
                //if (m_mp[0] == 0
                //    && m_twoLineGap[0] < 0   // down trend
                //    && m_close[0] > m_linregEma[0]
                //    && TimeToTrade())
                //{
                //    //m_regression_LE_Mkt.Send();
                //    m_regression_SE_Mkt.Send();
                //}
                //if (m_mp[0] == 0
                //    && m_twoLineGap[0] > 0   // up trend
                //    && m_close[0] < m_linregEma[0]
                //    && TimeToTrade())
                //{
                //    //m_regression_SE_Mkt.Send();
                //    m_regression_LE_Mkt.Send();
                //}

                //if (m_mp[0] > 0
                //    && m_twoLineGap[0] > 0
                //    && m_close[0] < m_linregEma[0]
                //    && TimeToTrade())
                //{
                //    //m_regression_LE_Mkt.Send();
                //    m_regression_LX_Mkt.Send();
                //}
                //if (m_mp[0] < 0
                //    && m_twoLineGap[0] < 0
                //    && m_close[0] > m_linregEma[0]
                //    && TimeToTrade())
                //{
                //    m_regression_SX_Mkt.Send();
                //    //m_regression_SE_Mkt.Send();
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