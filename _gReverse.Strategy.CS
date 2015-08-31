using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gReverse : SignalObject
    {
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_pos;
        private _gSpreadPnlFC m_sprdPnl;
        private IOrderMarket m_stopReverse_LE_Mkt;
        private IOrderMarket m_stopReverse_SE_Mkt;
        private int m_revCount;

		public _gReverse(object _ctx):base(_ctx)
        {
            //leg1 = 1;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //StartTime = "9:30";
            //EndTime = "15:30";
            //revLimit = 3;

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //StartTime = "9:30";
            //EndTime = "15:30";
            //revLimit = 3;

            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            StartTime = "9:30";
            EndTime = "15:30";
            revLimit = 3;
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
        public int revLimit { get; set; }

		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_stopReverse_LE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "Rev", EOrderAction.Buy));
            m_stopReverse_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "Rev", EOrderAction.SellShort));
		}
		protected override void StartCalc()
        {
            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.calltype = "strat";
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);
            m_revCount = 0;
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            if (m_pos[1] > 0 && m_pos[0] == 0 
                && m_sprdPnl[1] < 0
                && m_revCount < revLimit
                && TimeToTrade())
            {
                ++m_revCount;
                m_stopReverse_SE_Mkt.Send();
            }
            if (m_pos[1] < 0 && m_pos[0] == 0 
                && m_sprdPnl[1] < 0
                && m_revCount < revLimit
                && TimeToTrade())
            {
                ++m_revCount;
                m_stopReverse_LE_Mkt.Send();
            }
            if (!TimeToTrade())
                m_revCount = 0;
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