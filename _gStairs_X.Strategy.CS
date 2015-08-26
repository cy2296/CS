using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy 
{
	public class _gStairs_X : SignalObject
    {
        #region Fields
        private VariableSeries<int> m_pos;
        private _gStairsFC2 m_stairs;
        private VariableSeries<Double> m_spreadOpen;
        private VariableSeries<Double> m_spreadClos;
        private IOrderMarket m_stairLX_Mkt;
        private IOrderMarket m_stairSX_Mkt;
        private int m_count;
        #endregion

        public _gStairs_X(object _ctx):base(_ctx)
        {
            //////XLE
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //confirmbars = 8;

            ////// SPY  
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //confirmbars = 20;

            // CL
            leg1 = 0.5;
            leg2 = -1;
            leg3 = 0.333;
            confirmbars = 20;        
        }
        
        #region Properties
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int confirmbars { get; set; }
        #endregion
        
        protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            m_stairs = new _gStairsFC2(this);
            m_spreadOpen = new VariableSeries<double>(this);
            m_spreadClos = new VariableSeries<double>(this);
            m_stairLX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "Stair",
                    EOrderAction.Sell, OrderExit.FromAll));
            m_stairSX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "Stair",
                    EOrderAction.BuyToCover, OrderExit.FromAll));
		}
		
        protected override void StartCalc() 
        {
            m_stairs.o = (leg2 == 0 && leg3 == 0) ? Bars.Open : m_spreadOpen;
            m_stairs.h = (leg2 == 0 && leg3 == 0) ? Bars.High : m_spreadOpen;
            m_stairs.l = (leg2 == 0 && leg3 == 0) ? Bars.Low : m_spreadClos;
            m_stairs.c = (leg2 == 0 && leg3 == 0) ? Bars.Close : m_spreadClos;
            m_stairs.calltype = "strat";
            m_count = 0;
		}
		
        protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            m_spreadOpen.Value = Bars.Open[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Open[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Open[0] * leg3 : 0);
            m_spreadClos.Value = Bars.Close[0] * leg1
                + ((leg2 != 0) ? BarsOfData(2).Close[0] * leg2 : 0)
                + ((leg3 != 0) ? BarsOfData(3).Close[0] * leg3 : 0);

            if (m_pos[1] == 0)
                m_count = 1;
            else
            {
                ++m_count;
                if (m_pos[0] > 0 && m_spreadClos[0] < m_stairs[0]  
                    && m_count >= confirmbars)
                {
                    m_count = 0;
                    m_stairLX_Mkt.Send();
                }

                if (m_pos[0] < 0 && m_spreadClos[0] > m_stairs[0] 
                    && m_count >= confirmbars)
                {
                    m_count = 0;
                    m_stairSX_Mkt.Send();
                }
            }
		}
	}
}