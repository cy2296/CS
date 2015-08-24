using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
	public class _gRSI_50_E : SignalObject 
    {
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_mp;   // position info

        private Function.RSI m_rsi;
        private VariableObject<Int32> m_counter1;
        private VariableObject<Int32> m_counter2;

        private XAverage m_averagefc1;
        private XAverage m_averagefc2;
        private VariableSeries<Double> m_spread;

        private IOrderMarket m_rsiCross50_LE_Mkt;
        private IOrderMarket m_rsiCross50_SE_Mkt;

        public _gRSI_50_E(object _ctx):base(_ctx)
        {
            // XLE
            leg1 = 1;
            leg2 = -0.333;
            leg3 = -0.5;
            len_rsi = 5;
            confirmbars = 1;
            fastlength = 20;
            slowlength = 50;

            //// SPY
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //len_rsi = 5;
            //confirmbars = 1;
            //fastlength = 20;  
            //slowlength = 50;

            //// CL
            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //len_rsi = 5;
            //confirmbars = 1;
            //fastlength = 20;
            //slowlength = 50;
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
        public int confirmbars { get; set; }
        [Input]
        public int fastlength { get; set; }
        [Input]
        public int slowlength { get; set; }

		protected override void Create() 
        {
            m_mp = new VariableSeries<int>(this);
            StartTime = "9:30";
            EndTime = "15:30";

            m_rsi = new Function.RSI(this);
            m_counter1 = new VariableObject<int>(this);
            m_counter2 = new VariableObject<int>(this);
            
            m_averagefc1 = new XAverage(this);
            m_averagefc2 = new XAverage(this);
            m_spread = new VariableSeries<double>(this);

            m_rsiCross50_LE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_rsiCross50_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

            m_rsi.price = m_spread;
            m_rsi.length = len_rsi;

            m_averagefc1.Price = m_spread;
            m_averagefc1.Length = fastlength;
            m_averagefc2.Price = m_spread;
            m_averagefc2.Length = slowlength;
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;

            m_spread.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);

            if (PublicFunctions.DoubleGreater(m_rsi[0], 50))
                ++m_counter1.Value;
            else
                m_counter1.Value = 0;
            
            if (m_averagefc1[0] > m_averagefc2[0]
                && m_counter1.Value == confirmbars
                && m_mp[0] <= 0
                && TimeToTrade())
            {
                m_rsiCross50_LE_Mkt.Send();
            }

            if (PublicFunctions.DoubleLess(m_rsi[0], 50))
                ++m_counter2.Value;
            else
                m_counter2.Value = 0;

            if (m_averagefc1[0] < m_averagefc2[0]
                && m_counter2.Value == confirmbars
                && m_mp[0] >= 0
                && TimeToTrade())
            {
                m_rsiCross50_SE_Mkt.Send();
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