using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gRegression_E : SignalObject
    {
        private VariableSeries<int> m_mp;   // position info
        private LinearRegValue m_linearregvalue1;
        private VariableSeries<Double> m_linreg;



		public _gRegression_E(object _ctx):base(_ctx){}
		private IOrderMarket buy_order;
		protected override void Create() 
        {
            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<Double>(this);
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar(){
			// strategy logic 
			buy_order.Send();
		}
	}
}