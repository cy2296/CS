using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class gSprd_ATR_X : SignalObject
    {
        #region Fields
        private VariableSeries<int> m_pos;
        private _gSpreadPnlFC m_sprdPnl;
        private _gATR m_atrFC;
        private IOrderMarket m_atrLX_Mkt;
        private IOrderMarket m_atrSX_Mkt;
        #endregion
        
        public gSprd_ATR_X(object _ctx):base(_ctx)
        {
            ////XLE
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //AtrLength = 15;
            //NumAtrs = 2;

            ////// SPY  
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //AtrLength = 15;
            //NumAtrs = 5;

            //CL
            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            AtrLength = 15;
            NumAtrs = 5;
        }

        #region Properties
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int AtrLength { get; set; }
        [Input]
        public int NumAtrs { get; set; }
        #endregion
        
        protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_atrFC = new _gATR(this);
            m_atrLX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "ATR",
                    EOrderAction.Sell, OrderExit.FromAll));
            m_atrSX_Mkt = OrderCreator.MarketThisBar
               (new SOrderParameters(Contracts.Default, "ATR",
                    EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.calltype = "strat";
            m_atrFC.leg1 = leg1;
            m_atrFC.leg2 = leg2;
            m_atrFC.leg3 = leg3;
            m_atrFC.atrlen = AtrLength;
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            var atrstop = m_atrFC[0] * NumAtrs;
            if (m_sprdPnl[0] < 0 && Math.Abs(m_sprdPnl[0]) >= atrstop)
            { 
                if (m_pos.Value > 0)
                    m_atrLX_Mkt.Send();
                if (m_pos.Value < 0)
                    m_atrSX_Mkt.Send();
            }
		}
	}
}