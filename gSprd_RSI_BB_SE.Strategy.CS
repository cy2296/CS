using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class gSprd_RSI_BB_SE : SignalObject 
    {
        private VariableSeries<int> m_mp;   // position info
        private TimeSpan startTime, endTime;

        private _gSpreadRSI m_spreadRsiFC;
        private _gSpreadRsiUpBB m_spreadRsiUpBbFC;

        private VariableObject<Int32> m_counter;

        private IOrderPriced m_rsiBbCrossSE_Prc;
        private IOrderMarket m_rsiBbCrossSE_Mkt;

		public gSprd_RSI_BB_SE(object _ctx):base(_ctx)
        {
            //// XLE
            //leg1 = 1;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //len_rsi = 10;
            //numdevsup = 1.95;
            //numdevsdn = 1.95;
            //len_bb = 15;
            //confirmbars = 1;

            //// SPY
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //len_rsi = 10;
            //numdevsup = 1.95;
            //numdevsdn = 1.95;
            //len_bb = 15;
            //confirmbars = 1;

            //// CL
            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            len_rsi = 10;
            numdevsup = 1.95;
            numdevsdn = 1.95;
            len_bb = 15;
            confirmbars = 1;
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
            m_mp = new VariableSeries<int>(this);
            m_spreadRsiFC = new _gSpreadRSI(this);
            m_spreadRsiUpBbFC = new _gSpreadRsiUpBB(this);
            m_rsiBbCrossSE_Prc = OrderCreator.Limit
                (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_rsiBbCrossSE_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));

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
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;
            if (PublicFunctions.DoubleLess(m_spreadRsiFC[0], m_spreadRsiUpBbFC[0]))
            {
                ++m_counter.Value;
            }
            else
            {
                m_counter.Value = 0;
            }
            if (Bars.CurrentBar > (len_bb + len_rsi)
                && m_counter.Value == confirmbars
                && m_mp[0] >= 0
                && TimeToTrade())
            {
                //m_rsiBbCrossSE_Prc.Send(Bars.Close[0]);
                m_rsiBbCrossSE_Mkt.Send();
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