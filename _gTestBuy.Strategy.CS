using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gTestBuy : SignalObject 
    {
        private _gUtility_TimeToTrade m_timetotrade;

		public _gTestBuy(object _ctx):base(_ctx){}

        private IOrderMarket m_TestBuy_E_Mkt;
        private IOrderMarket m_TestBuy_X_Mkt;

        protected override void Create() 
        {
            m_timetotrade = new _gUtility_TimeToTrade(this);
            m_TestBuy_E_Mkt = OrderCreator.MarketThisBar
                                (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_TestBuy_X_Mkt = OrderCreator.MarketThisBar
                                (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            m_timetotrade.StartTime = "14:35";
            m_timetotrade.EndTime = "14:38";
		}
		protected override void CalcBar()
        {
            //m_TestBuy_E_Mkt.Send();

            if (m_timetotrade[0])
            {
                m_TestBuy_E_Mkt.Send();
            }
            else
            {
                m_TestBuy_X_Mkt.Send();
            }
		}
	}
}