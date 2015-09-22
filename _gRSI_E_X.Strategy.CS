using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gRSI_E_X : SignalObject 
    {
        private _gUtility_MarketsOpened m_marketopened;
        
        private _gUtility_TimeToTrade m_timetotrade;
        private _gUtility_TimeToTrade m_morningtime;

        private Function.RSI m_rsi;
        private _gPnlFC m_pnl;

        private VariableSeries<int> m_mp;   // position info
        private VariableSeries<Double> m_close;
        private VariableSeries<int> m_count1;   // confirm bars for oversold
        private VariableSeries<int> m_count2;   // confirm bars for overbought
        // for RSI BB
        private XAverage m_midband;
        private VariableSeries<Double> m_upband;
        private VariableSeries<Double> m_dnband;

        // 
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _MK_CurrDHigh m_dhigh;
        private _MK_CurrDLow m_dlow;



        private IOrderMarket m_rsiCross_LE_Mkt;
        private IOrderMarket m_rsiCross_SE_Mkt;
        private IOrderMarket m_rsiCross_LX_Mkt;
        private IOrderMarket m_rsiCross_SX_Mkt;

        private int m_countbars = 0;    // number of bars a position has been held

		public _gRSI_E_X(object _ctx):base(_ctx)
        {
            //// XLE
            //leg1 = 1;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //len_rsi = 14;
            //len_bb = 15;
            //confirmbars = 1;
            //startTime = "9:30";
            //endTime = "16:00";
            //stop = 0.15;
            //exit = 0.15;

            //// SPY
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;      
            //len_rsi = 14;
            //len_bb = 15;
            //confirmbars = 1;
            //startTime = "9:30";
            //endTime = "16:00";
            //stop = 0.15;
            //exit = 0.15;

            // CL
            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            len_rsi = 14;
            len_bb = 15;
            confirmbars = 1;
            startTime = "9:30";
            endTime = "16:00";
            stop = 0.15;
            exit = 0.15;
        }
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int len_rsi { get; set; }
        [Input]
        public int confirmbars { get; set; }
        [Input]
        public int len_bb { get; set; }
        [Input]
        public string startTime { get; set; }
        [Input]
        public string endTime { get; set; }
        [Input]
        public double stop { get; set; }
        [Input]
        public double exit { get; set; }

		protected override void Create() 
        {
            m_marketopened = new _gUtility_MarketsOpened(this);
            m_timetotrade = new _gUtility_TimeToTrade(this);
            m_morningtime = new _gUtility_TimeToTrade(this);
            m_rsi = new Function.RSI(this);
            m_pnl = new _gPnlFC(this);


            m_mp = new VariableSeries<int>(this);
            m_close = new VariableSeries<double>(this);
            m_count1 = new VariableSeries<int>(this);
            m_count2 = new VariableSeries<int>(this);

            m_midband = new XAverage(this);
            m_upband = new VariableSeries<double>(this);
            m_dnband = new VariableSeries<double>(this);

            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_dhigh = new _MK_CurrDHigh(this);
            m_dlow = new _MK_CurrDLow(this);

            m_rsiCross_LE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_rsiCross_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_rsiCross_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell));
            m_rsiCross_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover));
		}
		protected override void StartCalc() 
        {
            m_marketopened.leg1 = leg1;
            m_marketopened.leg2 = leg2;
            m_marketopened.leg3 = leg3;

            //m_timetotrade.StartTime = startTime;
            //m_timetotrade.EndTime = endTime;

            m_timetotrade.StartTime = "10:31";
            m_timetotrade.EndTime = "15:30";
            m_morningtime.StartTime = "9:30";
            m_morningtime.EndTime = "10:30";

            m_rsi.price = m_close;
            m_rsi.length = len_rsi;

            m_midband.Price = m_rsi;
            m_midband.Length = 180;

            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
            m_low.leg1 = leg1;
            m_low.leg2 = leg2;
            m_low.leg3 = leg3;
            
            m_dhigh.Data = m_high;
            m_dlow.Data = m_low;

            m_pnl.price = m_close;
            m_pnl.calltype = "strat";
        }
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;

            if (m_marketopened[0])
            { 
                // find entries
                m_close.Value = Bars.Close[0] * leg1
                    + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                    + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);

                // RSI Bollinger Band
                //var m_sdev = m_rsi.StandardDeviationCustom(180, 1);
                //m_upband.Value = m_midband[0] + 2 * m_sdev;
                //m_dnband.Value = m_midband[0] - 2 * m_sdev;
                
                // Fix Band
                m_upband.Value = 70;
                m_dnband.Value = 30;

                //  
                double mid = 0.5 * (m_dhigh[0] + m_dlow[0]);
                double midhigh = 0.5 * (m_dhigh[0] + mid);
                double midlow = 0.5 * (m_dlow[0] + mid);

                if (PublicFunctions.DoubleLess(m_rsi[0], m_upband[0]))
                    ++m_count1.Value;
                else
                    m_count1.Value = 0;
                if (m_count1.Value == confirmbars
                    && m_mp[0] >= 0
                    //&& m_timetotrade[0]
                    )
                {
                    if (m_timetotrade[0])
                    {
                        if (m_close[0] >= midhigh)
                        {
                            m_rsiCross_SE_Mkt.Send();
                            m_countbars = 1;
                        }
                        else
                        {
                            if (m_mp[0] > 0)
                            {
                                m_rsiCross_LX_Mkt.Send();
                                m_countbars = 0;
                            }
                        }
                    }
                    if (m_morningtime[0])
                    {
                        m_rsiCross_SE_Mkt.Send();
                        m_countbars = 1;
                    }
                }

                if (PublicFunctions.DoubleGreater(m_rsi[0], m_dnband[0]))
                    ++m_count2.Value;
                else
                    m_count2.Value = 0;

                if (m_count2.Value == confirmbars
                    && m_mp[0] <= 0
                    //&& m_timetotrade[0]
                    )
                {
                    if (m_timetotrade[0])
                    {
                        if (m_close[0] <= midlow)
                        {
                            m_rsiCross_LE_Mkt.Send();
                            m_countbars = 1;
                        }
                        else
                        {
                            if (m_mp[0] < 0)
                            {
                                m_rsiCross_SX_Mkt.Send();
                                m_countbars = 0;
                            }
                        }
                    }
                    if (m_morningtime[0])
                    {
                        m_rsiCross_LE_Mkt.Send();
                        m_countbars = 1;
                    }
                }
            }
            if (m_mp[0] != 0)
                ++m_countbars;

            //if (!m_marketopened[0]
            //    //|| !m_timetotrade[0]
            //    || m_pnl[0] < -Math.Abs(0.5)
            //    //|| m_pnl[0] > Math.Abs(1.0)
            //    //|| (m_countbars >= 50 && m_pnl[0] < 0)
            //    )
            //{
            //    if (m_mp[0] > 0)
            //    {
            //        m_rsiCross_LX_Mkt.Send();
            //        m_countbars = 0;
            //    }
            //    if (m_mp[0] < 0)
            //    {
            //        m_rsiCross_SX_Mkt.Send();
            //        m_countbars = 0;
            //    }
            //}
      

		}
	}
}