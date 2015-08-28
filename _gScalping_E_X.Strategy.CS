using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gScalping_E_X : SignalObject 
    {

        private _gBarAvg m_avg;
        private _gBarOpen m_open;
        private _gBarHigh m_high;
        private _gBarLow m_low;
        private _gBarClose m_close;
        private HighestFC m_highest1;
        private HighestFC m_highest2;
        private LowestFC m_lowest1;
        private LowestFC m_lowest2;
        private VariableSeries<int> m_mp;
        private VariableSeries<bool> m_cond1;
        private VariableSeries<bool> m_cond2;
        private VariableSeries<bool> m_cond3;
        private VariableSeries<bool> m_cond4;
        private VariableSeries<bool> m_cond5;
        private VariableSeries<bool> m_cond6;
        private IOrderMarket m_scalping_LE_Mkt;
        private IOrderMarket m_scalping_LX_Mkt;

        private IOrderMarket m_scalping_SE_Mkt;
        private IOrderMarket m_scalping_SX_Mkt;

        public _gScalping_E_X(object _ctx):base(_ctx)
        {
            leg1 = 1;
            leg2 = 0;
            leg3 = 0;
            UpRatio = 1;
            DnRatio = 1;
            fastLen = 5;
            slowLen = 15;
        }
        
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public double UpRatio { get; set; }
        [Input]
        public double DnRatio { get; set; }
        [Input]
        public int fastLen { get; set; }
        [Input]
        public int slowLen { get; set; }

        protected override void Create() 
        {
            m_avg = new _gBarAvg(this);
            m_open = new _gBarOpen(this);
            m_high = new _gBarHigh(this);
            m_low = new _gBarLow(this);
            m_close = new _gBarClose(this);
            m_highest1 = new HighestFC(this);
            m_highest2 = new HighestFC(this);
            m_lowest1 = new LowestFC(this);
            m_lowest2 = new LowestFC(this);
            m_mp = new VariableSeries<int>(this);
            m_cond1 = new VariableSeries<bool>(this);
            m_cond2 = new VariableSeries<bool>(this);
            m_cond3 = new VariableSeries<bool>(this);
            m_cond4 = new VariableSeries<bool>(this);
            m_cond5 = new VariableSeries<bool>(this);
            m_cond6 = new VariableSeries<bool>(this);
            m_scalping_LE_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_scalping_LX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_scalping_SE_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "S", EOrderAction.SellShort));
            m_scalping_SX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));

		}
		protected override void StartCalc() 
        {
            m_avg.leg1 = leg1;
            m_avg.leg2 = leg2;
            m_avg.leg3 = leg3;
            m_open.leg1 = leg1;
            m_open.leg2 = leg2;
            m_open.leg3 = leg3;
            m_high.leg1 = leg1;
            m_high.leg2 = leg2;
            m_high.leg3 = leg3;
            m_low.leg1 = leg1;
            m_low.leg2 = leg2;
            m_low.leg3 = leg3;
            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;
            m_highest1.pricevalue = m_high;
            m_highest2.pricevalue = m_high;
            m_lowest1.pricevalue = m_low;
            m_lowest2.pricevalue = m_low;
            m_highest1.len = fastLen;
            m_highest2.len = slowLen;
            m_lowest1.len = fastLen;
            m_lowest2.len = slowLen;
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;
            // going down 
            m_cond1.Value = (m_lowest1[0] <= m_lowest2[0]);
            // up bar criteria
            m_cond2.Value = (m_high[0] - m_low[0] > m_avg[0] * UpRatio / 100);
            m_cond3.Value = (m_close[0] > m_open[0] 
                && (m_close[0] - m_open[0] > m_avg[0] * DnRatio / 100));
            // going up 
            m_cond4.Value = (m_highest1[0] >= m_highest2[0]);
            
            // down bar criteria
            m_cond5.Value = (m_high[0] - m_low[0] > m_avg[0] * DnRatio / 100);  // ???? 
            m_cond6.Value = (m_close[0] < m_open[0] 
                && (m_open[0] - m_close[0] > m_avg[0] * DnRatio / 100));

            // going down and meet an up bar
            if (m_mp[0] == 0 && m_cond1[1] && m_cond2[1] && m_cond3[1])
                //m_scalping_LE_Mkt.Send();
                m_scalping_SE_Mkt.Send();
            
            // going down and meet a down bar 
            if (m_mp[0] < 0 && m_cond1[1] && m_cond5[1] && m_cond6[1])
                m_scalping_SX_Mkt.Send();
                //m_scalping_LE_Mkt.Send();

            // going up and meet a up bar
            if (m_mp[0] > 0 && m_cond4[1] && m_cond2[1] && m_cond3[1])
                m_scalping_LX_Mkt.Send();
                //m_scalping_SE_Mkt.Send();

            // going up and meet a down bar
            if (m_mp[0] == 0 && m_cond4[1] && m_cond5[1] && m_cond6[1])
                //m_scalping_SE_Mkt.Send();
                m_scalping_LE_Mkt.Send();
                //m_scalping_LX_Mkt.Send();
            
		}
	}
}