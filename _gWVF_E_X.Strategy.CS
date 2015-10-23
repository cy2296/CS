using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gWVF_E_X : SignalObject 
    {
        private HighestFC m_highest;
        private XAverage m_wvfAvg;
        private VariableSeries<double> m_wvf;
        private VariableSeries<int> m_mp;
        private VariableSeries<int> m_count1;   // confirm bars for crossover
        private VariableSeries<int> m_count2;   // confirm bars for crossunder

        public _gWVF_E_X(object _ctx):base(_ctx){}

        private IOrderMarket m_wvfCross_LE_Mkt;
        private IOrderMarket m_wvfCross_SE_Mkt;
        private IOrderMarket m_wvfCross_LX_Mkt;
        private IOrderMarket m_wvfCross_SX_Mkt;

        [Input]
        public int length { get; set; }

		protected override void Create() 
        {
            m_highest = new HighestFC(this);
            m_wvf = new VariableSeries<double>(this);
            m_mp = new VariableSeries<int>(this);
            m_count1 = new VariableSeries<int>(this);
            m_count2 = new VariableSeries<int>(this);

            m_wvfCross_LE_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "LE", EOrderAction.Buy));
            m_wvfCross_SE_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SE", EOrderAction.SellShort));
            m_wvfCross_LX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell, OrderExit.FromAll));
            m_wvfCross_SX_Mkt = OrderCreator.MarketThisBar
                    (new SOrderParameters(Contracts.Default, "SX", EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		protected override void StartCalc() 
        {
            m_highest.pricevalue = Bars.Close;
            m_highest.len = length;
            m_wvfAvg.Price = m_wvf;
            m_wvfAvg.Length = 5;
		}
		protected override void CalcBar()
        {
            m_mp.Value = StrategyInfo.MarketPosition;
            m_wvf.Value = (m_highest[0] - Bars.Low[0]) / m_highest[0] * 100;

            // cross over count
            if (PublicFunctions.DoubleGreater(m_wvf[0], m_wvfAvg[0]))
                ++m_count1.Value;
            else
                m_count1.Value = 0;
            // cross under count
            if (PublicFunctions.DoubleLess(m_wvf[0], m_wvfAvg[0]))
                ++m_count2.Value;
            else 
                m_count2.Value = 0;

            if (m_mp[0] == 0)
            { 
                if (m_count1 == 
            
            }
			buy_order.Send();
		}
	}
}