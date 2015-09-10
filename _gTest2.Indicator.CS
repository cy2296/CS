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
        private TimeSpan startTime, endTime;
        private VariableSeries<int> m_pos;
        //private Double m_count = 0;
        private _gSpreadPnlFC m_sprdPnl;

        private VariableSeries<int> m_stopPosition;
        private VariableSeries<Double> m_cumDayPnl;
        private VariableSeries<int> m_stopForTheDay;
        private bool getRealizedPnl = false;
        private System.Text.StringBuilder csv;  // for debug only


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
            m_sprdPnl = new _gSpreadPnlFC(this);
            m_cumDayPnl = new VariableSeries<double>(this);
            m_stopPosition = new VariableSeries<int>(this);
            m_stopForTheDay = new VariableSeries<int>(this);
			plot1 = AddPlot(new PlotAttributes("count", EPlotShapes.Histogram, Color.Aqua));
            csv = new StringBuilder();

            m_stopPos = new _gRisk_StopForPos(this);
		}
		protected override void StartCalc() 
        {
            startTime = ConvertToTime(StartTime);
            endTime = ConvertToTime(EndTime);
            m_sprdPnl.leg1 = 1;
            m_sprdPnl.leg2 = -0.333;
            m_sprdPnl.leg3 = -0.5;
            m_sprdPnl.calltype = "indic";


            m_stopPos.pnl = m_sprdPnl;
            m_stopPos.stop = 0.25;
		}
        protected override void CalcBar()
        {
            m_pos.Value = StrategyInfo.MarketPosition;



            if (TimeToTrade())
            {
                m_stopPosition.Value = (m_sprdPnl[0] < -Math.Abs(stopPos)) ? 1 : 0;

                if (m_pos[1] != 0 && m_pos[0] == 0
                    || m_pos[1] > 0 && m_pos[0] < 0
                    || m_pos[1] < 0 && m_pos[0] > 0)
                    m_cumDayPnl.Value += m_sprdPnl[0];
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

            plot1.Set(m_stopPos[0]);
        }

        private TimeSpan ConvertToTime(string timeToConvert)
        {
            DateTime dt = DateTime.ParseExact(timeToConvert, "H:mm",
                System.Globalization.CultureInfo.InvariantCulture);
            return dt.TimeOfDay;
        }

        private bool TimeToTrade()
        {
            TimeSpan barTime = Bars.TimeValue.TimeOfDay;
            if (startTime < endTime)
                return (barTime >= startTime) && (barTime <= endTime);
            return (barTime > startTime) || (barTime < endTime);
        }
	}
}