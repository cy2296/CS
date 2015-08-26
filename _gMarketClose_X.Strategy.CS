using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gMarketClose_X : SignalObject
    {
        #region Fields
        private VariableSeries<int> m_pos;
        private TimeSpan startTime, endTime;
        private IOrderMarket m_closeLX_Mkt;
        private IOrderMarket m_closeSX_Mkt;
        #endregion
        
        public _gMarketClose_X(object _ctx):base(_ctx)
        {
            StartTime = "9:30";
            EndTime = "15:50";
        }

        #region Properties
        [Input]
        public string StartTime { get; set; }
        [Input]
        public string EndTime { get; set; }
        #endregion

        protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_closeLX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "Close",
                    EOrderAction.Sell, OrderExit.FromAll));
            m_closeSX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "Close",
                    EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            if (!TimeToTrade() && m_pos[0] > 0)
                m_closeLX_Mkt.Send();
            if (!TimeToTrade() && m_pos[0] < 0)
                m_closeSX_Mkt.Send();
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