using System;
using System.Drawing;
using System.Linq;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _MK_PrevDHigh : FunctionSeries<System.Double>
		{
			public _MK_PrevDHigh(CStudyControl _master) : base(_master) { }
			public _MK_PrevDHigh(CStudyControl _master, int _ds) : base(_master, _ds) { }

			private bool m_prevBarEndedSession = false;
			private double m_currDHigh = 0;
			private double m_prevDHigh = 0;
			
			private ISeries<double> m_data;

            private System.Text.StringBuilder csv;  // for debug only			

			public ISeries<double> Data
            {
                get { return m_data; }
                set { m_data = value; }
            }
			
			protected override void Create()
			{
                csv = new StringBuilder();
			}

			protected override void StartCalc()
			{
				m_currDHigh = m_data.Value;
			}

			protected override System.Double CalcBar()
			{
                // for the new days
                if (m_prevBarEndedSession 
                    && !Bars.LastBarInSession  // for duplicate LastBars Issue
                    )
	            {
	                m_prevDHigh = m_currDHigh;
	                m_currDHigh = m_data.Value;
	                m_prevBarEndedSession = false;
	            }
				
				trackHigh();

	            if (Bars.LastBarInSession)
	            {
	                m_prevBarEndedSession = true;
	            }

                //if (Bars.CurrentBar < 500)
                //{
                //    var x = Bars.TimeValue.Date;
                //    var a = Bars.TimeValue.TimeOfDay;
                //    var b = Bars.CurrentBar;
                //    var c = m_prevBarEndedSession;
                //    var y = m_prevDHigh;
                //    var d = m_currDHigh;
                //    var e = Data.Value;

                //    var newLine = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}{7}",
                //    x, a, b, c, y, d, e, System.Environment.NewLine);
                //    csv.Append(newLine);
                //    System.IO.File.WriteAllText("H:/eSignalData/debug1.csv", csv.ToString());
                //}

				return m_prevDHigh;
			}
			
			private void trackHigh()
	        {
	            if (PublicFunctions.DoubleGreater(m_data.Value, m_currDHigh))
	            {
	                m_currDHigh = m_data.Value;
	            }
	        }
		}
	}
}