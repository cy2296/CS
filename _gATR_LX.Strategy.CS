using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gATR_LX : SignalObject 
    {
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_pos;
        private _gSpreadPnlFC m_sprdPnl;
        private _gATR m_atr;
        private double spreadBigPtVal;

        private IOrderMarket m_atrLX_Mkt;
        private IOrderPriced m_atrLX_stop;

		public _gATR_LX(object _ctx):base(_ctx)
        {
            ////XLE
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            AtrLength = 15;
            NumAtrs = 2;
            bigPt1 = 1000;
            bigPt2 = 500;
            bigPt3 = 1000;
            slippage = 0;
            exitType = 1;

            ////// SPY  
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //AtrLength = 15;
            //NumAtrs = 2;
            //bigPt1 = 500;
            //bigPt2 = 1000;
            //bigPt3 = 1000;
            //slippage = 0;
            //exitType = 2;

            // CL
            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //AtrLength = 15;
            //NumAtrs = 2;
            //bigPt1 = 1000;
            //bigPt2 = 1000;
            //bigPt3 = 500;
            //slippage = 0;
            //exitType = 2;     
        }
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public string StartTime { get; set; }
        [Input]
        public string EndTime { get; set; }

        [Input]
        public int AtrLength { get; set; }
        [Input]
        public int NumAtrs { get; set; }
        [Input]
        public double bigPt1 { get; set; }
        [Input]
        public double bigPt2 { get; set; }
        [Input]
        public double bigPt3 { get; set; }
        [Input]
        public double slippage { get; set; }

        [Input]
        public int exitType { get; set; }

        protected override void Create() 
        {
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_atr = new _gATR(this);
            m_pos = new VariableSeries<int>(this);
            m_atrLX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "ATR", 
                    EOrderAction.Sell, OrderExit.FromAll));
            m_atrLX_stop = OrderCreator.Stop (new SOrderParameters
                (Contracts.Default, "ATR", EOrderAction.Sell, OrderExit.FromAll));
            StartTime = "9:30";
            EndTime = "15:58";
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

            m_sprdPnl.bigPt1 = bigPt1;
            m_sprdPnl.bigPt2 = bigPt2;
            m_sprdPnl.bigPt3 = bigPt3;
            m_sprdPnl.pos1 = leg1;
            m_sprdPnl.pos2 = leg2;
            m_sprdPnl.pos3 = leg3;
            m_sprdPnl.slippage = slippage;
            m_sprdPnl.calltype = "strat";

            m_atr.leg1 = leg1;
            m_atr.leg2 = leg2;
            m_atr.leg3 = leg3;
            m_atr.atrlen = AtrLength;
            spreadBigPtVal = Math.Abs(bigPt1 * leg1 + bigPt2 * leg2 + bigPt3 * leg3);
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            if (m_pos.Value > 0)
            {
                var dollarRatio = Math.Abs(m_pos.Value / leg1) * spreadBigPtVal;
                var atrstop = m_atr[0] * NumAtrs * dollarRatio;
                if (exitType == 1)    // stop exit
                {
                    if (m_sprdPnl[0] < 0 && Math.Abs(m_sprdPnl[0]) >= atrstop)
                        m_atrLX_Mkt.Send();
                        //m_atrLX_stop.Send(Bars.Close[0]);
                }
                if (exitType == 2)    // profit exit
                { 
                    if (m_sprdPnl[0] > 0 && Math.Abs(m_sprdPnl[0]) >= atrstop)
                        m_atrLX_Mkt.Send();
                        //m_atrLX_stop.Send(Bars.Close[0]);
                }
                if (!TimeToTrade())
                    m_atrLX_Mkt.Send();
                    //m_atrLX_stop.Send(Bars.Close[0]);
            }
		}

        private TimeSpan ConvertToTime(string timeToConvert)
        {
            DateTime dt = DateTime.ParseExact(timeToConvert, "H:mm",
                System.Globalization.CultureInfo.InvariantCulture);

            return dt.TimeOfDay;
        }

        // The TimeToTrade() method returns 'true' if the current bar
        // time falls within the trading hours range, or else returns 'false'.
        private bool TimeToTrade()
        {
            TimeSpan barTime = Bars.TimeValue.TimeOfDay;

            // If the range is on the same day, both expressions must be true
            if (startTime < endTime)
                return (barTime >= startTime) && (barTime <= endTime);

            // Otherwise the range crosses midnight, and only one of
            // the expressions need to be true
            return (barTime > startTime) || (barTime < endTime);
        }
	}
}