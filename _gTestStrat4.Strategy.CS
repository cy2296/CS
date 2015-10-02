using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy {
	public class _gTestStrat4 : SignalObject 
    {
        private System.Text.StringBuilder csv;  // for debug only

        private _gUtility_TimeToTrade m_timetotrade;
        private _gBarClose m_close;
        private Function.RSI m_rsi;
        private bool m_prevBarEndedSession = false;

		public _gTestStrat4(object _ctx):base(_ctx)
        {
        
        
        }
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int len_rsi { get; set; }
        [Input]
        public int confirmbars { get; set; }
		
		protected override void Create() 
        {
            csv = new StringBuilder();
            m_close = new _gBarClose(this);
            m_rsi = new Function.RSI(this);


		}
		protected override void StartCalc() {
            m_timetotrade.StartTime = "9:30";
            m_timetotrade.EndTime = "9:50";

            m_close.leg1 = leg1;
            m_close.leg2 = leg2;
            m_close.leg3 = leg3;

            m_rsi.price = m_close;
            m_rsi.length = len_rsi;
		}
		protected override void CalcBar()
        {
            

		}
	}
}