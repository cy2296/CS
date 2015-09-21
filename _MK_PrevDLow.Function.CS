using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _MK_PrevDLow : FunctionSeries<System.Double>
		{
			public _MK_PrevDLow(CStudyControl _master) : base(_master) { }
			public _MK_PrevDLow(CStudyControl _master, int _ds) : base(_master, _ds) { }

			private bool m_prevBarEndedSession = false;
			private double m_currDLow = 0;
			private double m_prevDLow = 0;
			
			private ISeries<double> m_data;
			
			public ISeries<double> Data
            {
                get { return m_data; }
                set { m_data = value; }
            }
			
			protected override void Create()
			{
				// create variable objects and function objects
			}

			protected override void StartCalc()
			{
				m_currDLow = m_data.Value;
			}

			protected override System.Double CalcBar()
			{
				if (m_prevBarEndedSession
                    && !Bars.LastBarInSession)
	            {
	                m_prevDLow = m_currDLow;
	                m_currDLow = m_data.Value;
	                m_prevBarEndedSession = false;
	            }
				
				trackLow();

	            if (Bars.LastBarInSession)
	            {
	                m_prevBarEndedSession = true;
	            }
				
				return m_prevDLow;
			}
			
			private void trackLow()
	        {
	            if (PublicFunctions.DoubleLess(m_data.Value, m_currDLow))
	            {
	                m_currDLow = m_data.Value;
	            }
	        }
		}
	}
}