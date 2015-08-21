using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gStairs_SX : SignalObject
    {
        private VariableSeries<int> m_pos;
        private _gStairsFC m_stairs;
        private VariableSeries<Double> m_spreadOpen;
        private VariableSeries<Double> m_spreadClos;
        private TimeSpan startTime, endTime;

        private IOrderMarket m_stairSX_Mkt;

		public _gStairs_SX(object _ctx):base(_ctx)
        {
            ////XLE
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            initStop = 0.05;

            //// SPY  
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //initStop = 0.05;

            //// CL
            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //initStop = 0.05;
        }
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public double initStop { get; set; }
        [Input]
        public string StartTime { get; set; }
        [Input]
        public string EndTime { get; set; }

		protected override void Create()
        {
            m_pos = new VariableSeries<int>(this);
            m_stairs = new _gStairsFC(this);
            m_spreadOpen = new VariableSeries<double>(this);
            m_spreadClos = new VariableSeries<double>(this);
            m_stairSX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "SX",
                    EOrderAction.BuyToCover, OrderExit.FromAll));
            StartTime = "9:30";
            EndTime = "15:50";
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);
            m_stairs.o = (leg2 == 0 && leg3 == 0) ? Bars.Open : m_spreadOpen;
            m_stairs.h = (leg2 == 0 && leg3 == 0) ? Bars.High : m_spreadOpen;
            m_stairs.l = (leg2 == 0 && leg3 == 0) ? Bars.Low : m_spreadClos;
            m_stairs.c = (leg2 == 0 && leg3 == 0) ? Bars.Close : m_spreadClos;
            m_stairs.calltype = "strat";
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            m_spreadOpen.Value = Bars.Open[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
            m_spreadClos.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);

            if (m_pos[0] < 0 && m_spreadClos[0] > m_stairs[0])
                m_stairSX_Mkt.Send();
            if (!TimeToTrade())
                m_stairSX_Mkt.Send();
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