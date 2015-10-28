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
	public class gRSI_E_X : SignalObject 
    {
        // debug
        private System.Text.StringBuilder csv; 

        // Time 
        private _gUtility_MarketsOpened m_marketopened;
        private _gUtility_TimeToTrade m_timerange1;         // 9:30 ~ 10:30
        private _gUtility_TimeToTrade m_timerange2;         // 10:31 ~ 15:30

        // Price
        private _gBarClose m_close;
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _MK_CurrDHigh m_dhigh;
        private _MK_CurrDLow m_dlow;
        private double m_localhigh;
        private double m_locallow;
        private bool m_updatelocalvalue = false;

        // Indicator
        private Function.RSI m_rsi;
        private VariableSeries<Double> m_upband;
        private VariableSeries<Double> m_dnband;
        private VariableSeries<int> m_count1;   // confirm bars for oversold
        private VariableSeries<int> m_count2;   // confirm bars for overbought
        private XAverage m_rsiXavg;             // xaverage of RSI
       
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
        private int numlots;


        public gRSI_E_X(object _ctx):base(_ctx)
        {
            //// XLE
           // Inst = "XLE";
            Leg1 = 1;
            Leg2 = -0.333;
            Leg3 = -0.5;
            LengthRSI = 14;
            isBigSize = false;

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
        public int numlots2 { get; set; } // XLE, SPY, CL, ES
        [Input]
        public double Leg1 { get; set; }
        [Input]
        public double Leg2 { get; set; }
        [Input]
        public double Leg3 { get; set; }
        [Input]
        public int LengthRSI { get; set; }
        [Input]
        public bool isBigSize { get; set; }

		protected override void Create() 
        {
            csv = new StringBuilder();

            m_marketopened = new _gUtility_MarketsOpened(this);
            m_timerange1 = new _gUtility_TimeToTrade(this);
            m_timerange2 = new _gUtility_TimeToTrade(this);

            m_close = new _gBarClose(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_dhigh = new _MK_CurrDHigh(this);
            m_dlow = new _MK_CurrDLow(this);

            m_rsi = new Function.RSI(this);
            m_upband = new VariableSeries<double>(this);
            m_dnband = new VariableSeries<double>(this);
            m_count1 = new VariableSeries<int>(this);
            m_count2 = new VariableSeries<int>(this);
            m_rsiXavg = new XAverage(this);  

            m_pnl = new _gPnlFC(this);
            m_mp = new VariableSeries<int>(this);

            m_waitingforshort = new VariableSeries<bool>(this);
            m_waitingforlong = new VariableSeries<bool>(this);
            

            
            m_rsiCross_LE1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "L1", EOrderAction.Buy));
            m_rsiCross_LE2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "L2", EOrderAction.Buy));
            m_rsiCross_SE1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "S1", EOrderAction.SellShort));
            m_rsiCross_SE2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "S2", EOrderAction.SellShort));
            m_rsiCross_LX1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "LX1", EOrderAction.Sell));
            m_rsiCross_LX2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "LX2", EOrderAction.Sell));
            m_rsiCross_SX1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "SX1", EOrderAction.BuyToCover));
            m_rsiCross_SX2_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "SX2", EOrderAction.BuyToCover));
            m_rsiCross_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_rsiCross_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.CreateUserSpecified(numlots), "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc()
        {
            numlots = numlots2;
            //if (Inst == "XLE")
            //{
            //    numlots = (isBigSize) ? 2000 : 150;
            //}
            //else if (Inst == "SPY")
            //{
            //    numlots = (isBigSize) ? 166 : 50;
            //}
            //else if (Inst == "USO")
            //{
            //    numlots = 225;
            //}
            //else if (Inst == "CL" || Inst == "ES")
            //{
            //    numlots = (isBigSize) ? 1 : 0;
            //}
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
            m_high.leg1 = Leg1;
            m_high.leg2 = Leg2;
            m_high.leg3 = Leg3;
            m_low.leg1 = Leg1;
            m_low.leg2 = Leg2;
            m_low.leg3 = Leg3;
            m_dhigh.Data = m_high;
            m_dlow.Data = m_low;

            m_rsi.price = m_close;
            m_rsi.length = LengthRSI;
            m_rsiXavg.Price = m_rsi;
            m_rsiXavg.Length = 240;

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
                double diff1 = m_rsiXavg[0] - 50;
                m_upband.Value = 70 - diff1;
                m_dnband.Value = 30 - diff1;

                double mid = 0.5 * (m_dhigh[0] + m_dlow[0]);
                //double midhigh = 0.5 * (m_dhigh[0] + mid);
                //double midlow = 0.5 * (m_dlow[0] + mid);

                if (m_mp[0] == 0)
                {
                    m_entrycount = 0;
                    m_secondpos = false;
                }
                if (!m_timerange1[0] && !m_timerange2[0])
                {
                    m_waitingforlong.Value = false;
                    m_waitingforshort.Value = false;
                    m_entrycount = 0;
                    m_secondpos = false;
                }


                if (PublicFunctions.DoubleLess(m_rsi[0], m_upband[0]))
                    ++m_count1.Value;
                else
                    m_count1.Value = 0;

                if (PublicFunctions.DoubleGreater(m_rsi[0], m_dnband[0]))
                    ++m_count2.Value;
                else
                    m_count2.Value = 0;


                if (m_count1[0] == 0 && m_count1[1] != 0) // just above overbot
                {
                    m_updatelocalvalue = true;
                    m_localhigh = m_high[0];
                }
                if (m_count1[0] > 1)
                {
                    m_updatelocalvalue = false;
                }
                if (m_updatelocalvalue)
                {
                    m_localhigh = Math.Max(m_localhigh, m_high[0]);
                }
                if (m_count1[0] == 0)
                    m_waitingforshort.Value = true;


                // 9:30 ~ 10:30
                if (m_timerange1[0]) 
                {
                    if (m_count2[0] > 0  // avoid >75 to <25
                        && m_count1[0] == 0  // just come back down
                        //&& m_close[0] > m_localhigh
                        //&& m_waitingforshort[0]
                        )
                    {
                        if (m_mp[0] >= 0)
                        {
                            m_rsiCross_LX_Mkt.Send();
                            m_rsiCross_SE1_Mkt.Send();
                            m_entrycount = 1;
                            m_waitingforshort.Value = false;
                        }
                        if (m_mp[0] < 0 && m_entrycount == 1   // add short
                            && m_pnl[0] < -Math.Abs(m_firstpnl))
                        {
                            m_rsiCross_SE2_Mkt.Send();
                            ++m_entrycount;
                        }
                    }
                    if (m_count1[0] > 0  // avoid <25 to >75
                        && m_count2[0] == 0  // just come back up
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
                    //////
                    //if (m_count2[0] > 0)  // avoid >75 to <25
                    //{
                    //    if (m_count1[0] == 0)
                    //    {
                    //        if (m_mp[0] >= 0)
                    //        {
                    //            m_rsiCross_LX_Mkt.Send();
                    //            m_secondpos = false;
                    //            m_rsiCross_SE1_Mkt.Send();
                    //            m_entrycount = 1;
                    //            //m_waitingforshort.Value = false;
                    //        }
                    //        if (m_mp[0] < 0
                    //            && m_entrycount <= 2
                    //            && m_pnl[0] < -Math.Abs(m_firstpnl))
                    //        {
                    //            m_rsiCross_SE2_Mkt.Send();
                    //            m_secondpos = true;
                    //            ++m_entrycount;
                    //        }
                    //    }
                    //    if (m_count1[0] == 1)
                    //    {
                    //        switch (m_entrycount)
                    //        {
                    //            case 1:
                    //                m_rsiCross_SE1_Mkt.Send();
                    //                //++m_entrycount;
                    //                break;
                    //            case 2:
                    //                if (m_secondpos)
                    //                {
                    //                    m_rsiCross_SE1_Mkt.Send();
                    //                    m_rsiCross_SE2_Mkt.Send();
                    //                    // m_entrycount += 2;
                    //                }
                    //                break;
                    //            case 3:
                    //                if (m_secondpos)
                    //                {
                    //                    m_rsiCross_SE2_Mkt.Send();
                    //                    //++m_entrycount;
                    //                }
                    //                break;
                    //        }
                    //        ++m_entrycount;
                    //    }
                    //}

                    //if (m_count1[0] > 0)
                    //{
                    //    if (m_count2[0] == 0)
                    //    {
                    //        if (m_mp[0] <= 0)
                    //        {
                    //            m_rsiCross_SX_Mkt.Send();
                    //            m_secondpos = false;
                    //            m_rsiCross_LE1_Mkt.Send();
                    //            m_entrycount = 1;
                    //        }
                    //        if (m_mp[0] > 0
                    //            && m_entrycount <= 2
                    //            && m_pnl[0] < -Math.Abs(m_firstpnl))
                    //        {
                    //            m_rsiCross_LE2_Mkt.Send();
                    //            m_secondpos = true;
                    //            ++m_entrycount;
                    //        }
                    //    }
                    //    if (m_count2[0] == 1)
                    //    {
                    //        switch (m_entrycount)
                    //        {
                    //            case 1:
                    //                m_rsiCross_LE1_Mkt.Send();
                    //                //++m_entrycount;
                    //                break;
                    //            case 2:
                    //                if (m_secondpos)
                    //                {
                    //                    m_rsiCross_LE1_Mkt.Send();
                    //                    m_rsiCross_LE2_Mkt.Send();
                    //                    //m_entrycount += 2;
                    //                }
                    //                break;
                    //            case 3:
                    //                {
                    //                    m_rsiCross_LE2_Mkt.Send();
                    //                    //++m_entrycount;
                    //                }
                    //                break;
                    //        }
                    //        ++m_entrycount;
                    //    }
                    //}
                    //////
                }

                // 10:31 ~ 15:30
                if (m_timerange2[0])    
                {
                    if (m_count2[0] > 0  // avoid >75 to <25
                        && m_count1[0] == 0  // just come back down
                        //&& m_close[0] >= m_localhigh
                        //&& m_waitingforshort[0]
                        )
                    {
                        if (m_close[0] >= mid)
                        {
                            if (m_mp[0] >= 0)
                            {
                                m_rsiCross_LX_Mkt.Send();
                                m_rsiCross_SE1_Mkt.Send();
                                m_entrycount = 1;
                                m_waitingforshort.Value = false;
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
                    if (m_count2[0] == 0  // just come back up
                        && m_count1[0] > 0  // avoid <25 to >75
                        )
                    {
                        if (m_close[0] <= mid)
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
                    ////
                    //if (m_count2[0] > 0)  // avoid >75 to <25
                    //{
                    //    if (m_count1[0] == 0)
                    //    {
                    //        if (m_close[0] >= mid)
                    //        {
                    //            if (m_mp[0] >= 0)
                    //            {
                    //                m_rsiCross_LX_Mkt.Send();
                    //                m_secondpos = false;
                    //                m_rsiCross_SE1_Mkt.Send();
                    //                m_entrycount = 1;
                    //                //m_waitingforshort.Value = false;
                    //            }

                    //            if (m_mp[0] < 0
                    //                && m_entrycount <= 2
                    //                && m_pnl[0] < -Math.Abs(m_firstpnl)
                    //                )
                    //            {
                    //                m_rsiCross_SE2_Mkt.Send();  // add short
                    //                m_secondpos = true;
                    //                ++m_entrycount;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (m_mp[0] > 0)
                    //            {
                    //                m_rsiCross_LX_Mkt.Send();
                    //                m_secondpos = false;
                    //                m_entrycount = 0;
                    //            }
                    //        }
                    //    }
                    //    if (m_count1[0] == 1)
                    //    {
                    //        switch (m_entrycount)
                    //        {
                    //            case 1:
                    //                m_rsiCross_SE1_Mkt.Send();
                    //                //++m_entrycount;
                    //                break;
                    //            case 2:
                    //                if (m_secondpos)
                    //                {
                    //                    m_rsiCross_SE1_Mkt.Send();
                    //                    m_rsiCross_SE2_Mkt.Send();
                    //                    //m_entrycount += 2;
                    //                }
                    //                break;
                    //            case 3:
                    //                if (m_secondpos)
                    //                {
                    //                    m_rsiCross_SE2_Mkt.Send();
                    //                    //++m_entrycount;
                    //                }
                    //                break;
                    //        }
                    //        ++m_entrycount;
                    //    }
                    //}
                    //if (m_count1[0] > 0)  // avoid <25 to >75
                    //{
                    //    if (m_count2[0] == 0)
                    //    {
                    //        if (m_close[0] <= mid)
                    //        {
                    //            if (m_mp[0] <= 0)
                    //            {
                    //                m_rsiCross_SX_Mkt.Send();
                    //                m_secondpos = false;
                    //                m_rsiCross_LE1_Mkt.Send();
                    //                m_entrycount = 1;
                    //            }
                    //            if (m_mp[0] > 0
                    //                && m_entrycount <= 2
                    //                && m_pnl[0] < -Math.Abs(m_firstpnl))
                    //            {
                    //                m_rsiCross_LE2_Mkt.Send();  // add long
                    //                m_secondpos = true;
                    //                ++m_entrycount;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (m_mp[0] < 0)
                    //            {
                    //                m_rsiCross_SX_Mkt.Send();
                    //                m_secondpos = false;
                    //                m_entrycount = 0;
                    //            }
                    //        }
                    //    }
                    //    if (m_count2[0] == 1)
                    //    {
                    //        switch (m_entrycount)
                    //        {
                    //            case 1:
                    //                m_rsiCross_LE1_Mkt.Send();
                    //                //  ++m_entrycount;
                    //                break;
                    //            case 2:
                    //                if (m_secondpos)
                    //                {
                    //                    m_rsiCross_LE1_Mkt.Send();
                    //                    m_rsiCross_LE2_Mkt.Send();
                    //                    // m_entrycount += 2;
                    //                }
                    //                break;
                    //            case 3:
                    //                if (m_secondpos)
                    //                {
                    //                    m_rsiCross_LE2_Mkt.Send();
                    //                    //++m_entrycount;
                    //                }
                    //                break;
                    //        }
                    //        ++m_entrycount;
                    //    }
                    //}
                    //////
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