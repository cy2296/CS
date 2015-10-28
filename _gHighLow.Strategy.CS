using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gHighLow : SignalObject 
    {
        VariableSeries<int> m_pos;

        private _gBarHigh m_high;
        private _gBarLow m_low;

        private _MK_PrevDHigh m_prevDHigh;
        private _MK_PrevDLow m_prevDLow;
        private _MK_CurrDHigh m_CurrDHigh;
        private _MK_CurrDLow m_CurrDLow;

        private IOrderMarket m_rsiCross_LE1_Mkt;
        private IOrderMarket m_rsiCross_SE1_Mkt;
        private IOrderMarket m_rsiCross_LX_Mkt;
        private IOrderMarket m_rsiCross_SX_Mkt;

		public _gHighLow(object _ctx):base(_ctx){}
		
		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);

            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);

            m_prevDHigh = new _MK_PrevDHigh(this);
            m_prevDLow = new _MK_PrevDLow(this);
            m_CurrDHigh = new _MK_CurrDHigh(this);
            m_CurrDLow = new _MK_CurrDLow(this);

            m_rsiCross_LE1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "L1", EOrderAction.Buy));
            m_rsiCross_SE1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "S1", EOrderAction.SellShort));
            m_rsiCross_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_rsiCross_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            m_prevDHigh.Data = m_high;
            m_prevDLow.Data = m_low;
            m_CurrDHigh.Data = m_high;
            m_CurrDLow.Data = m_low;
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;



			buy_order.Send();
		}
	}
}