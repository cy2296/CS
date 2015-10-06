using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gTestSell : SignalObject
    {
        private _gUtility_TimeToTrade m_timetotrade;

        public _gTestSell(object _ctx):base(_ctx){}
		
        private IOrderMarket m_TestSell_E_Mkt;
        private IOrderMarket m_TestSell_X_Mkt;

		protected override void Create()
        {
            m_timetotrade = new _gUtility_TimeToTrade(this);
            m_TestSell_E_Mkt = OrderCreator.MarketThisBar
                                (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_TestSell_X_Mkt = OrderCreator.MarketThisBar
                                (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
        }
		protected override void StartCalc() 
        {
            m_timetotrade.StartTime = "14:18";
            m_timetotrade.EndTime = "14:20";
		}
		protected override void CalcBar()
        {
            if (m_timetotrade[0])
            {
                m_TestSell_E_Mkt.Send();
            }
            else
            {
                m_TestSell_X_Mkt.Send();
            }
		}
	}
}