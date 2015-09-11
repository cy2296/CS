using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
    [IOGMode(IOGMode.Enabled)]
	public class _gStop_Hard_X : SignalObject 
    {
        private VariableSeries<int> m_pos;
        private _gBarClose m_close; 
        private _gPnlFC m_pnl;

        private IOrderMarket m_hardLX_Mkt;
        private IOrderMarket m_hardSX_Mkt;
        
        public _gStop_Hard_X(object _ctx):base(_ctx)
        {
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //hardStop = 0.25;

            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //hardStop = 0.25;

            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            hardStop = 0.25;
        }

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public double hardStop {get; set;}

        protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_pnl = new _gPnlFC(this);
            m_close = new _gBarClose(this);
            m_hardLX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "HardLX",
                    EOrderAction.Sell, OrderExit.FromAll));
            m_hardSX_Mkt = OrderCreator.MarketThisBar
               (new SOrderParameters(Contracts.Default, "HardSX",
                    EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;

            m_pnl.price = m_close;
            m_pnl.calltype = "strat";
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            if (m_pnl[0] < -Math.Abs(hardStop))
            { 
                if (m_pos[0] > 0)
                    m_hardLX_Mkt.Send();
                if (m_pos[0] < 0)
                    m_hardSX_Mkt.Send(); 
            }
		}
	}
}