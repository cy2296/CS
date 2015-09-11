using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator{
	public class _gTest2 : IndicatorObject 
    {
        //private TimeSpan startTime, endTime;
        private VariableSeries<int> m_pos;
        //private Double m_count = 0;
        //private _gSpreadPnlFC m_sprdPnl;
        private _gPnlFC m_pnl;
        private _gBarClose m_close;

        private VariableSeries<int> m_stopPosition;
        private VariableSeries<Double> m_cumDayPnl;
        private VariableSeries<int> m_stopForTheDay;
        private bool getRealizedPnl = false;
        private System.Text.StringBuilder csv;  // for debug only

        private _gUtility_TimeToTrade m_timeToTrade;
        private _gRisk_StopLongCount m_stoplongcount;
        private _gRisk_StopShortCount m_stopshortcount;
        // Risk functions
        private _gRisk_StopForPos m_stopPos;

		public _gTest2(object _ctx):base(_ctx)
        {
            stopPos = 0.25;
            stopDay = 0.5;
            StartTime = "9:30";
            EndTime = "15:59";
        }
		private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;

        [Input]
        public double stopPos { get; set; }
        [Input]
        public double stopDay { get; set; }
        [Input]
        public string StartTime { get; set; }
        [Input]
        public string EndTime { get; set; }

		protected override void Create() 
        {
            m_pos = new VariableSeries<int>(this);
            //m_sprdPnl = new _gSpreadPnlFC(this);
            m_pnl = new _gPnlFC(this);
            m_close = new _gBarClose(this);
            m_cumDayPnl = new VariableSeries<double>(this);
            m_stopPosition = new VariableSeries<int>(this);
            m_stopForTheDay = new VariableSeries<int>(this);
			plot1 = AddPlot(new PlotAttributes("count", EPlotShapes.Histogram, Color.Aqua));
            plot2 = AddPlot(new PlotAttributes("count", EPlotShapes.Line, Color.Yellow));
            plot3 = AddPlot(new PlotAttributes("count", EPlotShapes.Line, Color.Pink));
            csv = new StringBuilder();

            m_stopPos = new _gRisk_StopForPos(this);

            m_timeToTrade = new _gUtility_TimeToTrade(this);
            m_stoplongcount = new _gRisk_StopLongCount(this);
            m_stopshortcount = new _gRisk_StopShortCount(this);
		}
		protected override void StartCalc() 
        {
            //startTime = ConvertToTime(StartTime);
            //endTime = ConvertToTime(EndTime);
            //m_sprdPnl.leg1 = 1;
            //m_sprdPnl.leg2 = -0.333;
            //m_sprdPnl.leg3 = -0.5;
            //m_sprdPnl.calltype = "indic";
            m_pnl.price = m_close;
            m_pnl.calltype = "indic";
            m_close.leg1 = 1;
            m_close.leg2 = -0.333;
            m_close.leg3 = -0.5;

            m_stopPos.pnl = m_pnl;
            m_stopPos.stop = 0.25;

            m_timeToTrade.StartTime = StartTime;
            m_timeToTrade.EndTime = EndTime;
            
            m_stoplongcount.calltype = "indic";
            m_stoplongcount.startTime = StartTime;
            m_stoplongcount.endTime = EndTime;
            m_stoplongcount.price = m_close;
            m_stoplongcount.stopCriteria = 0.25;

            m_stopshortcount.calltype = "indic";
            m_stopshortcount.startTime = StartTime;
            m_stopshortcount.endTime = EndTime;
            m_stopshortcount.price = m_close;
            m_stopshortcount.stopCriteria = 0.25;
		}
        protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;


            if (m_timeToTrade[0])
            {
                m_stopPosition.Value = (m_pnl[0] < -Math.Abs(stopPos)) ? 1 : 0;

                if (m_pos[1] != 0 && m_pos[0] == 0
                    || m_pos[1] > 0 && m_pos[0] < 0
                    || m_pos[1] < 0 && m_pos[0] > 0)
                    m_cumDayPnl.Value += m_pnl[0];
                else
                    m_cumDayPnl.Value = m_cumDayPnl[1];

                m_stopForTheDay.Value = (m_cumDayPnl[0] < -Math.Abs(stopDay)) ? 1 : 0;
            }
            else
            {
                m_stopPosition.Value = 0;
                m_cumDayPnl.Value = 0;
                m_stopForTheDay.Value = 0;
            }

            //if (Bars.CurrentBar < 2000)
            //{
            //    var a = Bars.TimeValue.Date;
            //    var b = Bars.TimeValue.TimeOfDay;
            //    var c = Bars.CurrentBar;
            //    var d = m_pos[0];
            //    var e = m_pnl[0];
            //    //var f = m_twoLineGap[0];
            //    //var g = m_mp[0];
            //    //var h = m_sprdPnl[0];
            //    //var i = m_stopPos[0];

            //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}{5}",
            //    a, b, c, d, e, System.Environment.NewLine);
            //    csv.Append(newLine);
            //    System.IO.File.WriteAllText("H:/eSignalData/debug13.csv", csv.ToString());
            //}



            plot1.Set(m_stopPosition[0]);
            plot2.Set(m_stoplongcount[0]);
            plot3.Set(m_stopshortcount[0]);
        }



        //private TimeSpan ConvertToTime(string timeToConvert)
        //{
        //    DateTime dt = DateTime.ParseExact(timeToConvert, "H:mm",
        //        System.Globalization.CultureInfo.InvariantCulture);
        //    return dt.TimeOfDay;
        //}

        //private bool TimeToTrade()
        //{
        //    TimeSpan barTime = Bars.TimeValue.TimeOfDay;
        //    if (startTime < endTime)
        //        return (barTime >= startTime) && (barTime <= endTime);
        //    return (barTime > startTime) || (barTime < endTime);
        //}
	}
}