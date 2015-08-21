using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
	public class gSprd_RSI_BB_SX : SignalObject 
    {
        private VariableSeries<int> m_mp;   // position info
        private TimeSpan startTime, endTime;

        private _gSpreadRSI m_spreadRsiFC;
        private _gSpreadRsiUpBB m_spreadRsiUpBbFC;

        private _gSpreadRsiDnBB m_spreadRsiDnBbFC;

        private VariableObject<Int32> m_counter;
        private _gSpreadPnlFC m_sprdPnl;

        private IOrderPriced m_rsiBbCrossSX_stop;
        private IOrderMarket m_rsiBbCrossSX_Mkt;

		public gSprd_RSI_BB_SX(object _ctx):base(_ctx)
        {
            //leg1 = 1;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //len_rsi = 10;
            //numdevsup = 1.95;
            //numdevsdn = 1.95;
            //len_bb = 15;
            //confirmbars = 1;
            //exitType = 1;
            //bigPt1 = 1;
            //bigPt2 = 1;
            //bigPt3 = 1000;
            //pos1 = 2000;
            //pos2 = -667;
            //pos3 = -1;
            //slippage = 0;

            // SPY
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //len_rsi = 10;
            //numdevsup = 1.95;
            //numdevsdn = 1.95;
            //len_bb = 15;
            //confirmbars = 1;
            //exitType = 1;
            //bigPt1 = 1;
            //bigPt2 = 1;
            //bigPt3 = 1000;
            //pos1 = 667;
            //pos2 = -2000;
            //pos3 = 1;
            //slippage = 0;

            //// CL
            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            len_rsi = 10;
            numdevsup = 1.95;
            numdevsdn = 1.95;
            len_bb = 15;
            confirmbars = 1;
            exitType = 1;
            bigPt1 = 1000;
            bigPt2 = 1;
            bigPt3 = 1;
            pos1 = 1;
            pos2 = -2000;
            pos3 = 667;
            slippage = 0;        
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
        [Input]
        public int exitType { get; set; }
        [Input]
        public Int32 len_rsi { get; set; }
        [Input]
        public double numdevsup { get; set; }
        [Input]
        public double numdevsdn { get; set; }
        [Input]
        public Int32 len_bb { get; set; }
        [Input]
        public int confirmbars { get; set; }



		protected override void Create() 
        {
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_mp = new VariableSeries<int>(this);
            m_spreadRsiFC = new _gSpreadRSI(this);

            m_spreadRsiUpBbFC = new _gSpreadRsiUpBB(this);
            m_spreadRsiDnBbFC = new _gSpreadRsiDnBB(this);

            m_rsiBbCrossSX_stop = OrderCreator.Stop
                             (new SOrderParameters(Contracts.Default, "SX",
                              EOrderAction.BuyToCover, OrderExit.FromAll));
            m_rsiBbCrossSX_Mkt = OrderCreator.MarketThisBar
                             (new SOrderParameters(Contracts.Default, "SX",
                              EOrderAction.BuyToCover, OrderExit.FromAll));
            m_counter = new VariableObject<int>(this);
            StartTime = "9:30";
            EndTime = "15:58";
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

            m_spreadRsiFC.price1 = Bars.Close;
            m_spreadRsiFC.price2 = (leg2 != 0) ? BarsOfData(2).Close : Bars.Close;
            m_spreadRsiFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiFC.leg1 = leg1;
            m_spreadRsiFC.leg2 = leg2;
            m_spreadRsiFC.leg3 = leg3;
            m_spreadRsiFC.length = len_rsi;

            m_spreadRsiUpBbFC.price1 = Bars.Close;
            m_spreadRsiUpBbFC.price2 = (leg2 != 0) ? BarsOfData(2).Close : Bars.Close;
            m_spreadRsiUpBbFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiUpBbFC.leg1 = leg1;
            m_spreadRsiUpBbFC.leg2 = leg2;
            m_spreadRsiUpBbFC.leg3 = leg3;
            m_spreadRsiUpBbFC.len_rsi = len_rsi;
            m_spreadRsiUpBbFC.numdevsdn = numdevsdn;
            m_spreadRsiUpBbFC.numdevsup = numdevsup;
            m_spreadRsiUpBbFC.len_bb = len_bb;

            m_spreadRsiDnBbFC.price1 = Bars.Close;
            m_spreadRsiDnBbFC.price2 = (leg2 != 0) ? BarsOfData(2).Close : Bars.Close;
            m_spreadRsiDnBbFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiDnBbFC.leg1 = leg1;
            m_spreadRsiDnBbFC.leg2 = leg2;
            m_spreadRsiDnBbFC.leg3 = leg3;
            m_spreadRsiDnBbFC.len_rsi = len_rsi;
            m_spreadRsiDnBbFC.numdevsdn = numdevsdn;
            m_spreadRsiDnBbFC.numdevsup = numdevsup;
            m_spreadRsiDnBbFC.len_bb = len_bb;

            m_sprdPnl.bigPt1 = bigPt1;
            m_sprdPnl.bigPt2 = bigPt2;
            m_sprdPnl.bigPt3 = bigPt3;
            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.pos1 = pos1;
            m_sprdPnl.pos2 = pos2;
            m_sprdPnl.pos3 = pos3;
            m_sprdPnl.slippage = slippage;
            m_sprdPnl.calltype = "strat";
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;

            if (exitType == 1)
            {
                if (PublicFunctions.DoubleLess(m_spreadRsiFC[0], m_spreadRsiDnBbFC[0]))
                    ++m_counter.Value;
                else
                    m_counter.Value = 0;

                if (m_mp.Value < 0)
                {
                    if (Bars.CurrentBar > (len_bb + len_rsi)
                        && m_counter.Value == confirmbars
                        && m_sprdPnl[0] >= 0)
                    {
                        m_rsiBbCrossSX_Mkt.Send();
                        //m_rsiBbCrossSX_stop.Send(Bars.Close[0]);
                    }
                }
            }
            if (exitType == 2)
            {
                if (PublicFunctions.DoubleGreater(m_spreadRsiFC[0], m_spreadRsiUpBbFC[0]))
                    ++m_counter.Value;
                else
                    m_counter.Value = 0;

                if (m_mp.Value < 0)
                {
                    if (Bars.CurrentBar > (len_bb + len_rsi)
                        && m_counter.Value == confirmbars
                        && m_sprdPnl[0] <= 0)
                    {
                        m_rsiBbCrossSX_Mkt.Send();
                        //m_rsiBbCrossSX_stop.Send(Bars.Close[0]);
                    }
                }
            }
            if (!TimeToTrade())
                m_rsiBbCrossSX_Mkt.Send();
                //m_rsiBbCrossSX_stop.Send(Bars.Close[0]);
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