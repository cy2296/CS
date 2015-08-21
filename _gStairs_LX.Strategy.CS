using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gStairs_LX : SignalObject 
    {
        private VariableSeries<int> m_pos;
        private _gStairsFC m_stairs;
        private double m_currHigh;

        private IOrderMarket m_atrLX_Mkt;

		public _gStairs_LX(object _ctx):base(_ctx)
        {
        
        }
		private IOrderMarket buy_order;

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }

		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_stairs = new _gStairsFC(this);
			buy_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Buy));
		}
		protected override void StartCalc()
        {
            m_stairs.o = Bars.Open * leg1 
                + (BarsOfData(2) 

		}
		protected override void CalcBar(){
			// strategy logic 
			buy_order.Send();
		}
	}
}