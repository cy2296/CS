using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _MK_CurrDHigh : FunctionSeries<System.Double>
		{
			public _MK_CurrDHigh(CStudyControl _master) : base(_master) { }
			public _MK_CurrDHigh(CStudyControl _master, int _ds) : base(_master, _ds) { }

            private bool m_prevBarEndedSession = false;
            private double m_currDHigh = 0;

            public ISeries<double> Data;

			protected override void Create()
			{
			}

			protected override void StartCalc()
            {
                m_currDHigh = Data.Value;
			}

			protected override System.Double CalcBar()
			{
                // for the new days
                if (m_prevBarEndedSession
                    && !Bars.LastBarInSession  // for duplicate LastBars Issue
                    )
                {
                    m_currDHigh = Data.Value;
                    m_prevBarEndedSession = false;
                }
                
                trackHigh();
                
                if (Bars.LastBarInSession)
                {
                    m_prevBarEndedSession = true;
                }

                return m_currDHigh;

			}
            private void trackHigh()
            {
                if (PublicFunctions.DoubleGreater(Data.Value, m_currDHigh))
                {
                    m_currDHigh = Data.Value;
                }
            }
		}
	}
}