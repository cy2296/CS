using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gManual_1 : SignalObject 
	{
		public _gManual_1(object _ctx):base(_ctx){}
		
		private IOrderMarket m_rsiCross_LE1_Mkt;
		private IOrderMarket m_rsiCross_SE1_Mkt;
		private IOrderMarket m_rsiCross_LX1_Mkt;
		private IOrderMarket m_rsiCross_SX1_Mkt;
	
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
		
		
		protected override void Create() {
            m_rsiCross_LE1_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "L1", EOrderAction.Buy));		
			m_rsiCross_SE1_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "S1", EOrderAction.SellShort));
            m_rsiCross_LX1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX1", EOrderAction.Sell, OrderExit.FromAll));
            m_rsiCross_SX1_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX1", EOrderAction.BuyToCover, OrderExit.FromAll));			
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar(){
	
		}
	}
}