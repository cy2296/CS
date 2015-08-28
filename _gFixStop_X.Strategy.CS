using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
    public class _gFixStop_X : SignalObject
    {
        #region Fields
        private _gBarAvg m_avg;
        private VariableSeries<int> m_pos;
        private _gSpreadPnlFC m_sprdPnl;
        private IOrderMarket m_fixLX_Mkt;
        private IOrderMarket m_fixSX_Mkt;
        private double m_profit;
        private double m_stop;
        #endregion

        public _gFixStop_X(object _ctx)
            : base(_ctx)
        {
            //XLE
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            stoploss = 2;
            profittar = 3;

            //// SPY  
            //leg1 = 0.333;
            //leg2 = -1;
            //leg3 = 0.5;
            //stoploss = 2;
            //profittar = 3;

            ////CL
            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //stoploss = 2;
            //profittar = 3;
        }
        #region Properties
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public double stoploss { get; set; }
        [Input]
        public double profittar { get; set; }
        #endregion

        protected override void Create()
        {
            m_avg = new _gBarAvg(this);
            m_pos = new VariableSeries<int>(this);
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_fixLX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "FIX",
                    EOrderAction.Sell, OrderExit.FromAll));
            m_fixSX_Mkt = OrderCreator.MarketThisBar
               (new SOrderParameters(Contracts.Default, "FIX",
                    EOrderAction.BuyToCover, OrderExit.FromAll));
        }
        protected override void StartCalc()
        {
            m_avg.leg1 = leg1;
            m_avg.leg2 = leg2;
            m_avg.leg3 = leg3;
            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.calltype = "strat";
        }
        protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;

            if (m_pos[0] != 0 && m_pos[1] == 0)
            {
                m_profit = m_avg[1] * profittar * 0.01;
                m_stop = m_avg[1] * stoploss * 0.01;

                if (m_sprdPnl[0] > 0 && Math.Abs(m_sprdPnl[0]) >= Math.Abs(m_profit))
                {
                    if (m_pos[0] > 0)
                        m_fixLX_Mkt.Send();
                    if (m_pos[0] < 0)
                        m_fixSX_Mkt.Send();
                }
                if (m_sprdPnl[0] < 0 && Math.Abs(m_sprdPnl[0]) >= Math.Abs(m_stop))
                {
                    if (m_pos[0] > 0)
                        m_fixLX_Mkt.Send();
                    if (m_pos[0] < 0)
                        m_fixSX_Mkt.Send();
                }
            }
        }
    }
}