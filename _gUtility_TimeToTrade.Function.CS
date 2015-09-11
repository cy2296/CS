using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gUtility_TimeToTrade : FunctionSeries<System.Boolean>
		{
            private TimeSpan startTime, endTime;

			public _gUtility_TimeToTrade(CStudyControl _master) : base(_master) { }
			public _gUtility_TimeToTrade(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public string StartTime { get; set; }
            public string EndTime { get; set; }

			protected override void Create()
			{
			}

			protected override void StartCalc()
			{
                startTime = ConvertToTime(StartTime);
                endTime = ConvertToTime(EndTime);
			}

			protected override System.Boolean CalcBar()
			{
                if (TimeToTrade())
                    return true;
                else
                    return false;
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
}