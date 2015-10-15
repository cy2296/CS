using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy {
	public class _gRSI_E_X_2 : SignalObject
    {
        // debug
        private System.Text.StringBuilder csv;

        // Time 
        private _gUtility_MarketsOpened m_marketopened;
        private _gUtility_TimeToTrade m_timerange1;         // 9:30 ~ 10:30
        private _gUtility_TimeToTrade m_timerange2;         // 10:31 ~ 15:30
        private bool m_prevBarEndedSession = false;
        private int m_todaybarcount = 0;

        // Price
        private _gBarClose m_close;

        private double m_currPrcSum;
        private double m_currPrcDevSum;


        // Indicator
        private Function.RSI m_rsi;
        private double m_overbot = 70;
        private double m_oversod = 30;
        private VariableSeries<int> m_count1;   // confirm bars for oversold
        private VariableSeries<int> m_count2;   // confirm bars for overbought
        private double m_currRsiSum;
        private double m_currRsiDevSum;

        // Position 
        private _gPnlFC m_pnl;
        private VariableSeries<int> m_mp;
        private int m_entrycount = 0;
        private double m_firstpnl = 0.3;
        private bool m_secondpos = false;

        // Place order
        private VariableSeries<bool> m_waitingforshort;// = false;
        private VariableSeries<bool> m_waitingforlong;// = false;
        private IOrderMarket m_rsiCross_LE1_Mkt;
        private IOrderMarket m_rsiCross_LE2_Mkt;
        private IOrderMarket m_rsiCross_SE1_Mkt;
        private IOrderMarket m_rsiCross_SE2_Mkt;
        private IOrderMarket m_rsiCross_LX1_Mkt;
        private IOrderMarket m_rsiCross_LX2_Mkt;
        private IOrderMarket m_rsiCross_SX1_Mkt;
        private IOrderMarket m_rsiCross_SX2_Mkt;
        private IOrderMarket m_rsiCross_LX_Mkt;
        private IOrderMarket m_rsiCross_SX_Mkt;

		public _gRSI_E_X_2(object _ctx):base(_ctx)
        {
            //// XLE
            Leg1 = 1;
            Leg2 = -0.333;
            Leg3 = -0.5;
            LengthRSI = 14;

            //// SPY
            //Leg1 = 0.333;
            //Leg2 = -1;
            //Leg3 = 0.5;
            //LengthRSI = 14;

            //// USO
            //Leg1 = 0.5;
            //Leg2 = -1;
            //Leg3 = 0.333;
            //LengthRSI = 14; 
        }
        [Input]
        public double Leg1 { get; set; }
        [Input]
        public double Leg2 { get; set; }
        [Input]
        public double Leg3 { get; set; }
        [Input]
        public int LengthRSI { get; set; }

		protected override void Create()
        {
            csv = new StringBuilder();

            m_marketopened = new _gUtility_MarketsOpened(this);
            m_timerange1 = new _gUtility_TimeToTrade(this);
            m_timerange2 = new _gUtility_TimeToTrade(this);

            m_close = new _gBarClose(this);

            m_rsi = new Function.RSI(this);
            m_count1 = new VariableSeries<int>(this);
            m_count2 = new VariableSeries<int>(this);

            m_pnl = new _gPnlFC(this);
            m_mp = new VariableSeries<int>(this);

            m_waitingforshort = new VariableSeries<bool>(this);
            m_waitingforlong = new VariableSeries<bool>(this);
            m_rsiCross_LE1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L1", EOrderAction.Buy));
            m_rsiCross_LE2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L2", EOrderAction.Buy));
            m_rsiCross_SE1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S1", EOrderAction.SellShort));
            m_rsiCross_SE2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S2", EOrderAction.SellShort));
            m_rsiCross_LX1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX1", EOrderAction.Sell));
            m_rsiCross_LX2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX2", EOrderAction.Sell));
            m_rsiCross_SX1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX1", EOrderAction.BuyToCover));
            m_rsiCross_SX2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX2", EOrderAction.BuyToCover));
            m_rsiCross_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_rsiCross_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
        }
		protected override void StartCalc()
        {
            m_marketopened.leg1 = Leg1;
            m_marketopened.leg2 = Leg2;
            m_marketopened.leg3 = Leg3;
            m_timerange1.StartTime = "9:30";
            m_timerange1.EndTime = "10:30";
            m_timerange2.StartTime = "10:31";
            m_timerange2.EndTime = "15:30";

            m_close.leg1 = Leg1;
            m_close.leg2 = Leg2;
            m_close.leg3 = Leg3;

            m_rsi.price = m_close;
            m_rsi.length = LengthRSI;

            m_pnl.price = m_close;
            m_pnl.calltype = "strat";

            m_waitingforshort.Value = false;
            m_waitingforlong.Value = false;
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;

            if (m_marketopened[0])
            {
                if (m_mp[0] == 0)
                {
                    m_entrycount = 0;
                    m_secondpos = false;
                }
                if (!m_timerange1[0] && !m_timerange2[0])
                {
                    m_entrycount = 0;
                    m_secondpos = false;
                }
                ///////////////////////////////////////////
                if (m_prevBarEndedSession)
                {
                    m_todaybarcount = 1;
                    m_prevBarEndedSession = false;
                    m_currPrcSum = m_close[0];
                    m_currPrcDevSum = 0;

                    m_currRsiSum = m_rsi[0];
                    m_currRsiDevSum = 0;
                }
                else
                {
                    ++m_todaybarcount;
                    m_currPrcSum += m_close[0];
                    m_currRsiSum += m_rsi[0];
                }
                if (Bars.LastBarInSession)
                {
                    m_prevBarEndedSession = true;
                    m_currPrcSum = 0;
                    m_currPrcDevSum = 0;
                    m_currRsiSum = 0;
                    m_currRsiDevSum = 0;
                }

                double priceMean = m_currPrcSum / m_todaybarcount;
                m_currPrcDevSum += Math.Pow((m_close[0] - priceMean), 2);
                double priceStdev = Math.Sqrt(m_currPrcDevSum / m_todaybarcount);

                double rsiMean = m_currRsiSum / m_todaybarcount;
                m_currRsiDevSum += Math.Pow((m_rsi[0] - rsiMean), 2);
                double rsiStdev = Math.Sqrt(m_currRsiDevSum / m_todaybarcount);
                double rsiHigh = rsiMean + 2 * rsiStdev;
                double rsiLow = rsiMean - 2 * rsiStdev;
                ///////////////////////////////////////////

                // 9:30 ~ 10:30
                if (m_timerange1[0])
                {
                    if (PublicFunctions.DoubleLess(m_rsi[0], m_overbot))
                        ++m_count1.Value;
                    else
                        m_count1.Value = 0;

                    if (PublicFunctions.DoubleGreater(m_rsi[0], m_oversod))
                        ++m_count2.Value;
                    else
                        m_count2.Value = 0;

                    if (m_count2[0] > 0  // avoid >75 to <25
                        && m_count1[0] == 0  // just abvoe
                        )
                    {
                        if (m_mp[0] >= 0)
                        {
                            m_rsiCross_LX_Mkt.Send();
                            m_rsiCross_SE1_Mkt.Send();
                            m_entrycount = 1;
                        }
                        if (m_mp[0] < 0 && m_entrycount == 1   // add short
                            && m_pnl[0] < -Math.Abs(m_firstpnl))
                        {
                            m_rsiCross_SE2_Mkt.Send();
                            ++m_entrycount;
                        }
                    }
                    if (m_count1[0] > 0  // avoid <25 to >75
                        && m_count2[0] == 0  // just below
                        )
                    {
                        if (m_mp[0] <= 0)
                        {
                            m_rsiCross_SX_Mkt.Send();
                            m_rsiCross_LE1_Mkt.Send();
                            m_entrycount = 1;
                        }
                        if (m_mp[0] > 0 && m_entrycount == 1   // add long
                            && m_pnl[0] < -Math.Abs(m_firstpnl))
                        {
                            m_rsiCross_LE2_Mkt.Send();
                            ++m_entrycount;
                        }
                    }
                }

                // 10:31 ~ 15:30
                if (m_timerange2[0])
                {
                    if (PublicFunctions.DoubleLess(m_rsi[0], rsiHigh))
                        ++m_count1.Value;
                    else
                        m_count1.Value = 0;

                    if (PublicFunctions.DoubleGreater(m_rsi[0], rsiLow))
                        ++m_count2.Value;
                    else
                        m_count2.Value = 0;

                    if (m_count2[0] > 0  // avoid >75 to <25
                        && m_count1[0] == 0  // just above
                        )
                    {
                        if (m_close[0] >= priceMean)
                        {
                            if (m_mp[0] >= 0)
                            {
                                m_rsiCross_LX_Mkt.Send();
                                m_rsiCross_SE1_Mkt.Send();
                                m_entrycount = 1;
                            }

                            if (m_mp[0] < 0 && m_entrycount == 1
                                && m_pnl[0] < -Math.Abs(m_firstpnl)
                                )
                            {
                                m_rsiCross_SE2_Mkt.Send();  // add short
                                ++m_entrycount;
                            }
                        }
                        else
                        {
                            if (m_mp[0] > 0)
                            {
                                m_rsiCross_LX_Mkt.Send();
                                m_entrycount = 0;
                            }
                        }
                    }
                    if (m_count2[0] == 0  // just below
                        && m_count1[0] > 0  // avoid <25 to >75
                        )
                    {
                        if (m_close[0] <= priceMean)
                        {
                            if (m_mp[0] <= 0)
                            {
                                m_rsiCross_SX_Mkt.Send();
                                m_rsiCross_LE1_Mkt.Send();
                                m_entrycount = 1;
                            }
                            if (m_mp[0] > 0 && m_entrycount == 1
                                && m_pnl[0] < -Math.Abs(m_firstpnl))
                            {
                                m_rsiCross_LE2_Mkt.Send();  // add long
                                ++m_entrycount;
                            }
                        }
                        else
                        {
                            if (m_mp[0] < 0)
                            {
                                m_rsiCross_SX_Mkt.Send();
                                m_entrycount = 0;
                            }
                        }
                    }
                }
            }

            //if (Bars.CurrentBar < 2000)
            //{
            //    var x = Bars.TimeValue.Date;
            //    var a = Bars.TimeValue.TimeOfDay;
            //    var b = Bars.CurrentBar;
            //    var c = m_mp[0];
            //    var d = m_pnl[0];
            //    var e = m_entrycount;
            //    var f = m_count1[0];
            //    var g = m_count2[0];

            //    var h = m_localhigh;
            //    var i = m_locallow;
            //    var j = m_close[0];
            //    var k = m_rsi[0];
            //    var l = m_waitingforshort[0];

            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}{13}",
            //    x, a, b, c, d, e, f, g, h, i, j, k, l, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("C:/data/debug1.csv", csv.ToString());
            //}
        }
	}
}