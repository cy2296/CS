using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using PowerLanguage.TradeManager;
using ATCenterProxy;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PowerLanguage.Strategy {
	public class _gTest_TradeManager1 : SignalObject 
    {
		public _gTest_TradeManager1(object _ctx):base(_ctx){}
		private IOrderMarket buy_order;

        private String IBProfilename = "Interactive Brokers";
        private String IBAccount = "DU197962";
        private String IBSymbol_ES = "ESU5";
        private String IBSymbol_VX = "VXU5";

        bool ESIsLastTradePositive;
        bool VXIsLastTradePositive;

        bool ESIsOrderPased;
        bool VXIsOrderPased;


		protected override void Create() {
			// create variable objects, function objects, order objects etc.
			buy_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Buy));
		}
		protected override void StartCalc() 
        {
			// assign inputs 
		}
		protected override void CalcBar(){
			// strategy logic 
			buy_order.Send();
		}
	}
}