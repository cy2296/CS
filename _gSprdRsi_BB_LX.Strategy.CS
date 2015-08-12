using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gSprdRsi_BB_LX : SignalObject 
    {
        private VariableSeries<int> m_mp;
        private TimeSpan startTime, endTime;

        private _gSpreadRSI m_spreadRsiFC;
        private _gSpreadRsiUpBB m_spreadRsiUpBbFC;
        private _gSpreadRsiDnBB m_spreadRsiDnBbFC;
        private VariableObject<Int32> m_counter;
        private _gSpreadPnlFC m_sprdPnl;

        private IOrderPriced m_rsiBbCrossLX_Prc;
        private IOrderMarket m_rsiBbCrossLX_Mkt;

		public _gSprdRsi_BB_LX(object _ctx):base(_ctx)
        {
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;
            len_rsi = 10;
            numdevsup = 2;
            numdevsdn = 2;
            len_bb = 15;
            confirmbars = 1;
            exitType = 1;
            bigPt1 = 1000;
            bigPt2 = 500;
            bigPt3 = 1000;
            slippage = 20;
        }
        public ISeries<Double> price1 { get; set; }
        public ISeries<Double> price2 { get; set; }
        public ISeries<Double> price3 { get; set; }

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public Int32 len_rsi { get; set; }
        [Input]
        public int numdevsup { get; set; }
        [Input]
        public int numdevsdn { get; set; }
        [Input]
        public Int32 len_bb { get; set; }
        [Input]
        public int confirmbars { get; set; }
        [Input]
        public string StartTime { get; set; }
        [Input]
        public string EndTime { get; set; }
        [Input]
        public int exitType { get; set; }
        [Input]
        public double bigPt1 { get; set; }
        [Input]
        public double bigPt2 { get; set; }
        [Input]
        public double bigPt3 { get; set; }
        [Input]
        public double slippage { get; set; }

		protected override void Create() 
        {
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_spreadRsiFC = new _gSpreadRSI(this);
            m_spreadRsiUpBbFC = new _gSpreadRsiUpBB(this);
            m_spreadRsiDnBbFC = new _gSpreadRsiDnBB(this);
            m_rsiBbCrossLX_Prc = OrderCreator.Limit
                            (new SOrderParameters(Contracts.Default, "LX", 
                             EOrderAction.Sell, OrderExit.FromAll));
            m_rsiBbCrossLX_Mkt = OrderCreator.MarketThisBar
                       (new SOrderParameters(Contracts.Default, "LX",
                             EOrderAction.Sell, OrderExit.FromAll));
            m_counter = new VariableObject<int>(this);
            m_mp = new VariableSeries<int>(this);
            StartTime = "9:30";
            EndTime = "16:00";
        }

		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

            m_spreadRsiFC.price1 = Bars.Close;
            m_spreadRsiFC.price2 = BarsOfData(2).Close;
            m_spreadRsiFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiFC.leg1 = leg1;
            m_spreadRsiFC.leg2 = leg2;
            m_spreadRsiFC.leg3 = leg3;
            m_spreadRsiFC.length = len_rsi;

            m_spreadRsiUpBbFC.price1 = Bars.Close;
            m_spreadRsiUpBbFC.price2 = BarsOfData(2).Close;
            m_spreadRsiUpBbFC.price3 = (leg3 != 0) ? BarsOfData(3).Close : Bars.Close;
            m_spreadRsiUpBbFC.leg1 = leg1;
            m_spreadRsiUpBbFC.leg2 = leg2;
            m_spreadRsiUpBbFC.leg3 = leg3;
            m_spreadRsiUpBbFC.len_rsi = len_rsi;
            m_spreadRsiUpBbFC.numdevsdn = numdevsdn;
            m_spreadRsiUpBbFC.numdevsup = numdevsup;
            m_spreadRsiUpBbFC.len_bb = len_bb;

            m_spreadRsiDnBbFC.price1 = Bars.Close;
            m_spreadRsiDnBbFC.price2 = BarsOfData(2).Close;
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
            m_sprdPnl.slippage = slippage;
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;

            if (exitType == 1)
            {
                if (PublicFunctions.DoubleGreater(m_spreadRsiFC[0], m_spreadRsiUpBbFC[0]))
                    ++m_counter.Value;
                else
                    m_counter.Value = 0;

                if (m_mp.Value > 0)
                {
                    if (Bars.CurrentBar > (len_bb + len_rsi)
                        && m_counter.Value == confirmbars
                        && m_sprdPnl[0] > 0)
                    {
                        m_rsiBbCrossLX_Mkt.Send();
                    }
                }
            }
            if (exitType == 2)
            {
                if (PublicFunctions.DoubleLess(m_spreadRsiFC[0], m_spreadRsiDnBbFC[0]))
                    ++m_counter.Value;
                else
                    m_counter.Value = 0;
                if (m_mp.Value > 0)
                {
                    if (Bars.CurrentBar > (len_bb + len_rsi)
                      && m_counter.Value == confirmbars
                      && m_sprdPnl[0] < 0)
                    {
                        m_rsiBbCrossLX_Mkt.Send();
                    }              
                }
            }
            if (!TimeToTrade() && m_mp.Value > 0)
                m_rsiBbCrossLX_Mkt.Send();
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