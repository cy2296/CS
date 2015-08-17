using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy 
{
	public class _gATR_SX : SignalObject 
    {
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_pos;
        private _gSpreadPnlFC m_sprdPnl;
        private _gATR m_atrFC;
        private VariableSeries<double> m_atr;
        private double spreadBigPtVal;


        private System.Text.StringBuilder csv;  // for debug only
   
		public _gATR_SX(object _ctx):base(_ctx)
        {
            ////XLE
            //leg1 = 1.0;
            //leg2 = -0.333;
            //leg3 = -0.5;
            //AtrLength = 15;
            //NumAtrs = 2;
            //bigPt1 = 1000;
            //bigPt2 = 500;
            //bigPt3 = 1000;
            //slippage = 0;
            //exitType = 1;

            // SPY  
            leg1 = 0.333;
            leg2 = -1;
            leg3 = 0.5;
            AtrLength = 15;
            NumAtrs = 2;
            bigPt1 = 500;
            bigPt2 = 1000;
            bigPt3 = 1000;
            slippage = 0;
            exitType = 2;

            //CL
            //leg1 = 0.5;
            //leg2 = -1;
            //leg3 = 0.333;
            //AtrLength = 15;
            //NumAtrs = 2;
            //bigPt1 = 1000;
            //bigPt2 = 1000;
            //bigPt3 = 500;
            //slippage = 0;
            //exitType = 2;    
        }

        private IOrderMarket m_atrSX_Mkt;

        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public string StartTime { get; set; }
        [Input]
        public string EndTime { get; set; }
        [Input]
        public int AtrLength { get; set; }
        [Input]
        public int NumAtrs { get; set; }
        [Input]
        public double bigPt1 { get; set; }
        [Input]
        public double bigPt2 { get; set; }
        [Input]
        public double bigPt3 { get; set; }
        [Input]
        public double slippage { get; set; }

        [Input]
        public int exitType { get; set; }

        void assign(_gSpreadFC sprdFunc, ISeries<Double> inst1,
            ISeries<Double> inst2, ISeries<Double> inst3)
        {
            sprdFunc.inst1 = inst1;
            sprdFunc.inst2 = inst2;
            sprdFunc.inst3 = inst3;
            sprdFunc.leg1 = leg1;
            sprdFunc.leg2 = leg2;
            sprdFunc.leg3 = leg3;
        }
		
        protected override void Create() 
        {
            m_atr = new VariableSeries<double>(this);
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_atrFC = new _gATR(this);
            m_pos = new VariableSeries<int>(this);
            m_atrSX_Mkt = OrderCreator.MarketThisBar
                (new SOrderParameters(Contracts.Default, "ATR",
                    EOrderAction.BuyToCover, OrderExit.FromAll));
            StartTime = "9:30";
            EndTime = "15:58";

            csv = new StringBuilder();
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);

            m_sprdPnl.bigPt1 = bigPt1;
            m_sprdPnl.bigPt2 = bigPt2;
            m_sprdPnl.bigPt3 = bigPt3;
            m_sprdPnl.leg1 = leg1;
            m_sprdPnl.leg2 = leg2;
            m_sprdPnl.leg3 = leg3;
            m_sprdPnl.slippage = slippage;
            m_sprdPnl.calltype = "strat";

            m_atrFC.leg1 = leg1;
            m_atrFC.leg2 = leg2;
            m_atrFC.leg3 = leg3;
            m_atrFC.atrlen = AtrLength;
            spreadBigPtVal = Math.Abs(bigPt1 * leg1 + bigPt2 * leg2 + bigPt3 * leg3);
		}
		protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;
            if (m_pos.Value < 0)
            {
                var dollarRatio = Math.Abs(m_pos.Value / leg1) * spreadBigPtVal;
                m_atr.Value = Math.Abs(m_atrFC[0] * NumAtrs * dollarRatio);

                if (exitType == 1)    // stop exit
                {
                    if (m_sprdPnl[0] < 0 && Math.Abs(m_sprdPnl[0]) >= m_atr.Value)
                        m_atrSX_Mkt.Send();
                }
                if (exitType == 2)    // profit exit
                {
                    if (m_sprdPnl[0] > 0 && Math.Abs(m_sprdPnl[0]) >= m_atr.Value)
                        m_atrSX_Mkt.Send();
                }
                if (!TimeToTrade())
                    m_atrSX_Mkt.Send();
                
                //if (Bars.CurrentBar < 5000)
                //{
                //    var x = Bars.TimeValue.Date;
                //    var a = Bars.TimeValue.TimeOfDay;
                //    var b = Bars.CurrentBar;
                //    var c = Bars.Close[0] * leg1 + BarsOfData(2).Close[0] * leg2 + BarsOfData(3).Close[0] * leg3;
                //    var z = m_pos[1];
                //    var y = StrategyInfo.MarketPosition;
                //    var d = m_sprdPnl[0];
                //    var e = m_atr.Value;
                //    var f = (Math.Abs(m_sprdPnl[0]) >= m_atr.Value) ? 1 : 0;

                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}{9}",
                //    x, a, b, c, z, y, d, e, f, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    System.IO.File.WriteAllText("H:/eSignalData/debug.csv", csv.ToString());
                //}
            }
		}
        private TimeSpan ConvertToTime(string timeToConvert)
        {
            DateTime dt = DateTime.ParseExact(timeToConvert, "H:mm",
                System.Globalization.CultureInfo.InvariantCulture);
 
            return dt.TimeOfDay;
        }
 
        // The TimeToTrade() method returns 'true' if the current bar
        // time falls within the trading hours range, or else returns 'false'.
        private bool TimeToTrade()
        {
            TimeSpan barTime = Bars.TimeValue.TimeOfDay;
 
            // If the range is on the same day, both expressions must be true
            if (startTime < endTime)
                return (barTime >= startTime) && (barTime <= endTime);
 
            // Otherwise the range crosses midnight, and only one of
            // the expressions need to be true
            return (barTime > startTime) || (barTime < endTime);
        }
    }
}