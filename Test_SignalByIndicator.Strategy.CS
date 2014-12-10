using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class Test_SignalByIndicator : SignalObject 
  {
		public Test_SignalByIndicator(object _ctx):base(_ctx){}
		private IOrderMarket buy_order;
    private IOrderMarket Sell_order;

    PowerLanguage.Indicator.ADX m_Adx;

		protected override void Create() 
    {
      m_Adx = (PowerLanguage.Indicator.ADX)AddIndicator("ADX");
			buy_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Buy));
      Sell_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell));
		}

		protected override void StartCalc() 
    {
      m_Adx.length = 10;
		}

		protected override void CalcBar()
    {
      if (m_Adx.Plots[0].Values[0] > m_Adx.Plots[0].Values[1])
        buy_order.Send();
      else
        Sell_order.Send();
		}
	}
}