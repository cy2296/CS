using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class gSprd_ATR_LX : SignalObject 
    {
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_pos;
        private _gSpreadPnlFC m_sprdPnl;
        private _gATR m_atrFC;
        private double spreadBigPtVal;

        private IOrderMarket m_atrLX_Mkt;
        private IOrderPriced m_atrLX_stop;

		public gSprd_ATR_LX(object _ctx):base(_ctx)
        {
            ////////XLE
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //AtrLength = 15;
            //NumAtrs = 2;
            //bigPt1 = 1;
            //bigPt2 = 1;
            //bigPt3 = 1000;
            //pos1 = 2000;
            //pos2 = -667;
            //pos3 = -1;
            //slippage = 0;
            ////exitType = 1;

            //// SPY  
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //AtrLength = 15;
            //NumAtrs = 2;
            //bigPt1 = 1;
            //bigPt2 = 1;
            //bigPt3 = 1000;
            //pos1 = 667;
            //pos2 = -2000;
            //pos3 = 1;
            //slippage = 0;
            ////exitType = 1;

            //// CL
            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            AtrLength = 15;
            NumAtrs = 2;
            bigPt1 = 1000;
            bigPt2 = 1;
            bigPt3 = 1;
            pos1 = 1;
            pos2 = -2000;
            pos3 = 667;
            slippage = 0;
            //exitType = 1;               
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
        public double bigPt1 { get; set; }
        [Input]
        public double bigPt2 { get; set; }
        [Input]
        public double bigPt3 { get; set; }
        [Input]
        public double pos1 { get; set; }
        [Input]
        public double pos2 { get; set; }
        [Input]
        public double pos3 { get; set; }
        [Input]
        public double slippage { get; set; }
        //[Input]
        //public int exitType { get; set; }
        [Input]
        public int AtrLength { get; set; }
        [Input]
        public int NumAtrs { get; set; }

		protected override void Create() 
        {
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_atrFC = new _gATR(this);
            m_pos = new VariableSeries<int>(this);
            m_atrLX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "ATR", 
                    EOrderAction.Sell, OrderExit.FromAll));
            m_atrLX_stop = OrderCreator.Stop(new SOrderParameters
                (Contracts.Default, "ATR", EOrderAction.Sell, OrderExit.FromAll));
            StartTime = "9:30";
            EndTime = "15:58";
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

            //m_sprdPnl.bigPt1 = bigPt1;
            //m_sprdPnl.bigPt2 = bigPt2;
            //m_sprdPnl.bigPt3 = bigPt3;
            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            //m_sprdPnl.pos1 = pos1;
            //m_sprdPnl.pos2 = pos2;
            //m_sprdPnl.pos3 = pos3;
            //m_sprdPnl.slippage = slippage;
            m_sprdPnl.calltype = "strat";

            m_atrFC.leg1 = leg1;
            m_atrFC.leg2 = leg2;
            m_atrFC.leg3 = leg3;
            m_atrFC.atrlen = AtrLength;
            spreadBigPtVal = Math.Abs(bigPt1 * pos1 + bigPt2 * pos2 + bigPt3 * pos3);
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            if (m_pos.Value > 0)
            {
                //var atrstop = (m_atrFC[0] * NumAtrs + 2 * slippage) * spreadBigPtVal;
                var atrstop = m_atrFC[0] * NumAtrs;
 
                //if (exitType == 1)    // stop exit
                {
                    if (m_sprdPnl[0] < 0 && Math.Abs(m_sprdPnl[0]) >= atrstop)
                        m_atrLX_Mkt.Send();
                        //m_atrLX_stop.Send(Bars.Close[0]);
                }
                //if (exitType == 2)    // profit exit
                //{
                //    if (m_sprdPnl[0] > 0 && Math.Abs(m_sprdPnl[0]) >= atrstop)
                //        m_atrLX_Mkt.Send();
                //        //m_atrLX_stop.Send(Bars.Close[0]);
                //}
                //if (!TimeToTrade())
                //    m_atrLX_Mkt.Send();
                    //m_atrLX_stop.Send(Bars.Close[0]);
            }
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