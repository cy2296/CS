using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _MK_CurrDLow : FunctionSeries<System.Double>
		{
			public _MK_CurrDLow(CStudyControl _master) : base(_master) { }
			public _MK_CurrDLow(CStudyControl _master, int _ds) : base(_master, _ds) { }

            private bool m_prevBarEndedSession = false;
            private double m_currDLow = 0;

            public ISeries<double> Data;

			protected override void Create()
			{
			}

			protected override void StartCalc()
			{
                m_currDLow = Data.Value;
			}

			protected override System.Double CalcBar()
			{
                // for the new days
                if (m_prevBarEndedSession
                    && !Bars.LastBarInSession  // for duplicate LastBars Issue
                    )
                {
                    m_currDLow = Data.Value;
                    m_prevBarEndedSession = false;
                }

                trackLow();

                if (Bars.LastBarInSession)
                {
                    m_prevBarEndedSession = true;
                }

                return m_currDLow;
			}
            private void trackLow()
            {
                if (PublicFunctions.DoubleLess(Data.Value, m_currDLow))
                {
                    m_currDLow = Data.Value;
                }
            }
		}
	}
}